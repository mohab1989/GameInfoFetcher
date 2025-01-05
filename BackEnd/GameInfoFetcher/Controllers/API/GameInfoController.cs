using GameInfoFetcher.Mappers;
using GameInfoFetcher.Models;
using GameInfoFetcher.Responses;
using GameInfoFetcher.Services;
using GameInfoFetcher.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace GameInfoFetcher.Controllers.Api;

[ApiController]
[Route("[controller]")]
public
class GameInfoController : ControllerBase
{
    readonly ICache<string, GameInfo> _cache;
    readonly IGamesApiClient _apiClient;
    readonly IServerlessDbService _db;
    readonly ILogger<GameInfoController> _logger;

    public GameInfoController(
        ICache<string,
        GameInfo> cache,
        IGamesApiClient apiClient,
        IServerlessDbService db,
        ILogger<GameInfoController> logger)
    {
        _cache = cache;
        _apiClient = apiClient;
        _logger = logger;
        _db = db;
    }

    // Example: /api/controller/games?gameNames=game1&gameNames=game2
    [HttpGet("games")]
    public async
    Task<ActionResult<IEnumerable<ResponseModels.GameInfo>>> GetGamesInfo([FromQuery] string[] gameNames)
    {
        if (gameNames == null || gameNames.Length == 0)
            return BadRequest("At least one game name must be provided.");

        var result = new List<ResponseModels.GameInfo>();

        foreach (var name in gameNames)
        {
            try
            {
                var gi = await GetGameDetails(name);
                result.Add(gi.MapToResponse());
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ResponseModels.GameInfo>>(ex, name);
            }
        }

        return Ok(result);
    }

    // checkout https://rawg.io/@mohab1989/apikey and https://api.rawg.io/docs/#operation/games_read
    // $response = Invoke-WebRequest -Uri "https://api.rawg.io/api/games?key=4c2c99d975d84ef9a95aec8fad6611a1&search={Witcher 3}" -Method Get
    [HttpGet("{gameName}")]
    public async
    Task<ActionResult<ResponseModels.GameInfo>> GetGameInfo(string gameName)
    {
        try
        {
            return Ok((await GetGameDetails(gameName)).MapToResponse());
        }

        catch (Exception ex)
        {
            return HandleException<ResponseModels.GameInfo>(ex, gameName);
        }
    }

    async
    Task<GameInfo> GetGameDetails(string gameName)
    {
        _logger.LogInformation($"Fetching game info for {gameName}");

        var slug = Slugify(gameName);
        var id = await _db.GetIdBySlug(slug);

        if (id is not null)
        {
            GameInfo gi = await _cache.GetGameInfo(id);
            if (gi.IsNotDefault())
                return gi;
        }

        using IResponse response = id is null ?
            await _apiClient.GetGamesInfoListFromName(slug) : await _apiClient.GetGameInfoFromId(id);

        response.HttpResponseMessage.EnsureSuccessStatusCode();

        string contentType = response.HttpResponseMessage.Content?.Headers?.ContentType?.MediaType ??
            throw new InvalidOperationException("Headers is missing in the response.");

        var gameInfoModel = contentType switch
        {
            "application/json" => await JsonResponseMapper.ToModel(response, slug),
            _ => throw new NotSupportedException($"Content type {contentType} is not supported.")
        };

        if (gameInfoModel.IsDefault())
            throw new InvalidOperationException($"Mapping json to Model failed for game {gameName}");

        if (id is null)
        {
            if (await _cache.SetGameInfo($"{gameInfoModel.Id}", gameInfoModel, TimeSpan.FromDays(30)))
            {
                if (slug != gameInfoModel.Slug)
                    await _db.SetSlugIdMapping(slug, $"{gameInfoModel.Id}");
                await _db.SetSlugIdMapping(gameInfoModel.Slug, $"{gameInfoModel.Id}");
            }
            else
                _logger.LogWarning($"Failed to cache game info for {gameName}");
        }

        return gameInfoModel;
    }

    static string Slugify(string input)
    {
        input = input.ToLowerInvariant();
        input = Regex.Replace(input, @"[^a-z0-9\s-]", ""); // Remove invalid characters
        input = Regex.Replace(input, @"\s+", " ").Trim(); // Replace multiple spaces with a single space
        input = input.Replace(" ", "-"); return input;
    }

    ActionResult<T> HandleException<T>(Exception ex, string gameName)
    => ex switch
    {
        HttpRequestException httpRequestEx => StatusCode((int)httpRequestEx.StatusCode, new
        {
            Message = "An error occurred while fetching game info",
        }),

        JsonReaderException jsonReaderEx => Problem(
            statusCode: 500,
            title: "An error occurred while parsing JSON",
            detail: jsonReaderEx.Message),

        InvalidOperationException invalidOperationEx => Problem(
            statusCode: 500,
            title: "Response component missing",
            detail: invalidOperationEx.Message),

        NotSupportedException notSupportedEx => Problem(
            statusCode: 500,
            title: "Unexpected contentType",
            detail: notSupportedEx.Message),

        _ => Problem(
            statusCode: 500,
            title: "An unexpected error occurred",
            detail: ex.Message)
    };
}
