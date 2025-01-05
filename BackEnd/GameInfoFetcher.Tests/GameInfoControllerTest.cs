using Enyim.Caching;
using GameInfoFetcher.Controllers.Api;
using GameInfoFetcher.Models;
using GameInfoFetcher.Responses;
using GameInfoFetcher.Services;
using GameInfoFetcher.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace GameInfoFetcher.Tests;

[TestClass]
public sealed class GameInfoControllerTest
{
    GameInfoController _controller;
    Cache _cache;
    Mock<ICache<string, GameInfo>> _mockCache = new();
    Mock<IGamesApiClient> _mockApiClient = new();
    Mock<ILogger<GameInfoController>> _mockLogger = new();
    Mock<IServerlessDbService> _mockDb = new();

    [TestInitialize]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<GameInfoController>(optional: true) // Secrets.json for dev
            .AddEnvironmentVariables() // env var for prod
            .Build();

        var memcachedConfig = GetSection(configuration,
            "Memcached",
            "ServerAddress", "ServerPort", "UserName", "Password");

        var serviceProvider = new ServiceCollection()
            .AddEnyimMemcached(options =>
            {
                options.AddServer(
                    memcachedConfig["ServerAddress"],
                    int.Parse(memcachedConfig["ServerPort"]!)
                    );

                options.AddPlainTextAuthenticator(
                    "",
                    memcachedConfig["UserName"],
                    memcachedConfig["Password"]
                    );

                configuration.GetSection("enyimMemcached").Bind(options);
            })
            .AddLogging()
            .BuildServiceProvider();

        _cache = new(serviceProvider.GetRequiredService<IMemcachedClient>());

        _controller = new GameInfoController(
            _mockCache.Object,
            _mockApiClient.Object,
            _mockDb.Object,
            _mockLogger.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _cache.Dispose();
        _mockDb.Object.Dispose();
        _mockApiClient.Object.Dispose();
    }


    [TestMethod]
    public async Task TestCacheConnection()
    {
        var game = new GameInfo() { Name = "test" };
        var id = "1";
        Assert.IsTrue(await Task.Run(async () => await _cache.SetGameInfo(id, game, TimeSpan.FromMinutes(5))));
        var v = await Task.Run(async () => await _cache.GetGameInfo(id));
        Assert.AreEqual(game, v);
    }

    [TestMethod]
    public async Task Get_ReturnsOkResult_WithGameInfo()
    {
        // Arrange
        var gameName = "Mind Over Magnet";
        var slug = "mind-over-magnet";

        _mockCache.Setup(a => a.GetGameInfo(slug))
                  .ReturnsAsync(new GameInfo()); // Simulate a cache miss

        _mockDb.Setup(a => a.GetIdBySlug(slug))
               .ReturnsAsync((string?)null);

        _mockApiClient.Setup(a => a.GetGamesInfoListFromName(slug))
            .ReturnsAsync(() => 
                new GamesListResponse(
                    new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                            ReadEmbeddedResource("GameInfoFetcher.Tests.Resources.RawgResponse.json"))
                        {
                            Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                        }
                    }));

        for (int i = 0; i < 10; ++i)
        {
            try
            {
                var response = await _controller.GetGameInfo(gameName);
                Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception was thrown during deserialization: {ex.Message}");
            }
        }
    }

    static string ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
                          ?? throw new FileNotFoundException($"Resource {resourceName} not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    IConfigurationSection GetSection(IConfigurationRoot config, string sectionName, params string[] variableNames)
    {
        var configSection = config.GetSection(sectionName);

        if (configSection is null)
            throw new InvalidOperationException($"Configuration Section '{sectionName}' is missing.");

        foreach (var variableName in variableNames)
        {
            if (string.IsNullOrWhiteSpace(configSection[variableName]))
                throw new InvalidOperationException(
                    $"Environment Variable '{variableName}' in Section '{sectionName}' was not defined.");
        }

        return configSection;
    }
}
