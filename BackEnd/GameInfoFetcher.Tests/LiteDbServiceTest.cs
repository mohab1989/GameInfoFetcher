using GameInfoFetcher.Services;
using GameInfoFetcher.Services.Interfaces;

namespace GameInfoFetcher.Tests;

[TestClass]
public
class LiteDbServiceTest
{
    IServerlessDbService _db;
    string _slug = "slug-test", _id = "12345", _dbPath = ".\\GameInfoFetcherTest.db";

    [TestInitialize]
    public void Setup()
    {
        Cleanup();
        _db = new LiteDbService($"Filename={_dbPath}");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_db is not null) _db.Dispose();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
    }


    [TestMethod]
    public async
    Task TestUpdate()
    {
        Assert.IsTrue(await _db.SetSlugIdMapping(_slug, _id)); // insert
        Assert.IsTrue(await _db.SetSlugIdMapping(_slug, _id)); // update same value
        Assert.IsTrue(await _db.SetSlugIdMapping(_slug, "12346")); // update different value
    }

    [TestMethod]
    public async
    Task TestGetIdBySlug()
    {
        Assert.IsTrue(await _db.SetSlugIdMapping(_slug, _id));
        Assert.AreEqual(_id, await _db.GetIdBySlug(_slug));
    }
}
