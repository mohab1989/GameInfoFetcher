using GameInfoFetcher.Responses;

namespace GameInfoFetcher.Services;

public class RawgClient : IGamesApiClient
{
    readonly HttpClient _httpClient;
    readonly string _apiKey;
    record struct RawgConfig(string BaseUrl, string ApiKey);
    public RawgClient(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

#pragma warning disable CS8603 // Possible null reference return.
    public async
    Task<GamesListResponse> GetGamesInfoListFromName(string gameName)
    { 
        UriBuilder uriBuilder = new(_httpClient.BaseAddress?? throw BaseAddressIsNullException);

        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["key"] = _apiKey;
        query["search"] = $"{{{gameName}}}";
        query["search_precise"] = "true";
        uriBuilder.Query = query.ToString();

        return new(await _httpClient.GetAsync(uriBuilder.Uri));
    }

    public async

    Task<GameByIdResponse> GetGameInfoFromId(string id) =>
        new(await _httpClient.GetAsync(new Uri(_httpClient.BaseAddress ??
            throw BaseAddressIsNullException,
            id)));

#pragma warning restore CS8603 // Possible null reference return.

    Exception BaseAddressIsNullException { get; }
        = new InvalidOperationException(
                "The base address must have been set at the construction of this class.");

    public void Dispose() => _httpClient.Dispose();
}
