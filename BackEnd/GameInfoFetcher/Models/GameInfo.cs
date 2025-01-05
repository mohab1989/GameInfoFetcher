namespace GameInfoFetcher.Models;

public
struct GameInfo
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public int Playtime { get; set; }
    public List<PlatformWrapper> Platforms { get; set; }
    public List<StoreWrapper> Stores { get; set; }
    public string Released { get; set; }
    public bool Tba { get; set; }
    public string BackgroundImage { get; set; }
    public double Rating { get; set; }
    public int RatingTop { get; set; }
    public List<Rating> Ratings { get; set; }
    public int RatingsCount { get; set; }
    public int ReviewsTextCount { get; set; }
    public int Added { get; set; }
    public AddedByStatus AddedByStatus { get; set; }
    public object Metacritic { get; set; }
    public int SuggestionsCount { get; set; }
    public string Updated { get; set; }
    public int Id { get; set; }
    public string Score { get; set; }
    public object Clip { get; set; }
    public List<Tag> Tags { get; set; }
}

public struct PlatformWrapper
{
    public Platform Platform { get; set; }
}

public struct Platform
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public struct StoreWrapper
{
    public Store Store { get; set; }
}

public struct Store
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public struct Rating
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Count { get; set; }
    public double Percent { get; set; }
}

public struct AddedByStatus
{
    public int Owned { get; set; }
    public int Beaten { get; set; }
    public int Playing { get; set; }
    public int? Yet { get; set; } // Nullable as it's missing in the JSON
    public int? Dropped { get; set; }
    public int? Wishlist { get; set; }
}

public struct Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Language { get; set; }
    public int GamesCount { get; set; }
    public string ImageBackground { get; set; }
}