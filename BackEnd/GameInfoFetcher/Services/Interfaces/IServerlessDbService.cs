namespace GameInfoFetcher.Services.Interfaces;

public
interface IServerlessDbService : IDisposable
{
    Task<string> GetIdBySlug(string slug);
    Task<bool> SetSlugIdMapping(string slug, string id);
}
