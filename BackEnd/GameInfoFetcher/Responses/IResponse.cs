namespace GameInfoFetcher.Responses;

public interface IResponse : IDisposable
{
    HttpResponseMessage HttpResponseMessage { get; }
}
