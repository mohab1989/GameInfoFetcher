namespace GameInfoFetcher.Responses;

public class GameByIdResponse : IResponse
{
    public HttpResponseMessage HttpResponseMessage { get; }

    public GameByIdResponse(HttpResponseMessage response)
        => HttpResponseMessage = response ?? throw new ArgumentNullException(nameof(response));

    public void Dispose() =>
        HttpResponseMessage?.Dispose();
}
