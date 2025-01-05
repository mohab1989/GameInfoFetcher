using GameInfoFetcher.Responses;

namespace GameInfoFetcher.Services;

public interface IGamesApiClient : IDisposable
{
    Task<GamesListResponse> GetGamesInfoListFromName(string gameName);
    Task<GameByIdResponse> GetGameInfoFromId(string gameName);
}
