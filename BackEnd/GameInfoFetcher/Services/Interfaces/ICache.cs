namespace GameInfoFetcher.Services;

public interface ICache<Key, Value> : IDisposable
{
    Task<Value> GetGameInfo(Key key);
    Task<bool> SetGameInfo(Key key, Value Value, TimeSpan ttl);
}
