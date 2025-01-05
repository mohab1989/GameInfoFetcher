
namespace GameInfoFetcher.Responses;

public class GamesListResponse : IResponse
{
    public HttpResponseMessage HttpResponseMessage { get; }
    public GamesListResponse(HttpResponseMessage response)
        => HttpResponseMessage = response ?? throw new ArgumentNullException(nameof(response));
    public void Dispose() =>
        HttpResponseMessage?.Dispose();
}
