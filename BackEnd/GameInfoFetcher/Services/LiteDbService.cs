using GameInfoFetcher.Services.Interfaces;
using LiteDB;
using Nito.AsyncEx;

namespace GameInfoFetcher.Services;

public 
class LiteDbService : IServerlessDbService
{
    class SlugIdMapping
    {
        public string GameSlug { get; set; }
        public string GameId { get; set; }
    }

    readonly LiteDatabase _db;
    readonly ILiteCollection<SlugIdMapping> _collection;
    readonly AsyncLock _lock = new();

    public LiteDbService(string connectionString)
    {
        _db = new LiteDatabase(connectionString);
        _collection = _db.GetCollection<SlugIdMapping>("slugIdMap");
    }

    public async
    Task<string> GetIdBySlug(string slug)
    {
        using var _ = await _lock.LockAsync();
        return _collection.FindOne(x => x.GameSlug == slug)?.GameId;
    }

    public async
    Task<bool> SetSlugIdMapping(string slug, string id)
    {
        using var _ = await _lock.LockAsync();
        SlugIdMapping existingMapping = _collection.FindOne(x => x.GameSlug == slug);

        if (existingMapping is null)
        {
            var newMapping = new SlugIdMapping { GameSlug = slug, GameId = id };
            _collection.Insert(newMapping);
            return true;
        }
        else
        {
            existingMapping.GameId = id;
            return _collection.Update(existingMapping);
        }
    }

    public void Dispose() => _db.Dispose();
}
