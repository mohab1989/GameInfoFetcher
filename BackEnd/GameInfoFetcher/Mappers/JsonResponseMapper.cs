using Fastenshtein;
using GameInfoFetcher.Responses;
using Newtonsoft.Json.Linq;

namespace GameInfoFetcher.Mappers;

public
class JsonResponseMapper
{
    public static async
    Task<Models.GameInfo> ToModel(IResponse response, string slug)
    {
        return response switch
        {
            GamesListResponse r => await ToModel(r, slug),
            GameByIdResponse r => await ToModel(r),
            _ => throw new InvalidOperationException("Unknown response type.")
        };
    }

    static async
    Task<Models.GameInfo> ToModel(GamesListResponse response, string slug)
    {
        string responseBody = await (response.HttpResponseMessage.Content?.ReadAsStringAsync() ??
          throw new InvalidOperationException("Content type is missing in the response."));

        JObject json = JObject.Parse(responseBody);

        JArray results = json["results"] as JArray ??
            throw new InvalidOperationException($"couldn't find results");

        var sortedArray = new JArray(results.OrderBy(i => Levenshtein.Distance(i["slug"].ToString(), slug)));

        return sortedArray.First().ToObject<Models.GameInfo>();
    }

    static async
    Task<Models.GameInfo> ToModel(GameByIdResponse response)
    {
        string responseBody = await (response.HttpResponseMessage.Content?.ReadAsStringAsync() ??
          throw new InvalidOperationException("Content type is missing in the response."));

        return JObject.Parse(responseBody).ToObject<Models.GameInfo>();
    }
}
