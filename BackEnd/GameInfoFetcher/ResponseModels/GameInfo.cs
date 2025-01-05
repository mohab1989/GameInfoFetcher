namespace GameInfoFetcher.ResponseModels;

public record struct
GameInfo(
    int Id,
    string Name,
    double Rating,
    int Playtime);
