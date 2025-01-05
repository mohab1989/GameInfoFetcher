using Newtonsoft.Json;

namespace GameInfoFetcher.Mappers;

public static
class GameInfoMapper
{
    public static ResponseModels.GameInfo MapToResponse(this Models.GameInfo gameInfo)
        => new ResponseModels.GameInfo(gameInfo.Id, gameInfo.Name, gameInfo.Rating, gameInfo.Playtime);

    public static string MapToJsonString(this Models.GameInfo gameInfo)
        => JsonConvert.SerializeObject(gameInfo, Formatting.Indented);
}
