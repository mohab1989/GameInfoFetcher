using Enyim.Caching;
using GameInfoFetcher.Models;
using Nito.AsyncEx;

namespace GameInfoFetcher.Services;

public class Cache : ICache<string, GameInfo>
{
    readonly IMemcachedClient _memcachedClient;
    readonly AsyncLock _lock = new();

    public Cache(IMemcachedClient memcachedClient) => _memcachedClient = memcachedClient;

    public async
    Task<GameInfo> GetGameInfo(string key)
    {
        using var _ = await _lock.LockAsync();
        return await _memcachedClient.GetValueAsync<GameInfo>(key);
    }

    public async
    Task<bool> SetGameInfo(string key, GameInfo value, TimeSpan ttl)
    {
        using var _ = await _lock.LockAsync();
        return await _memcachedClient.SetAsync(key, value, ttl);
    }

    public void Dispose() => _memcachedClient.Dispose();
}
