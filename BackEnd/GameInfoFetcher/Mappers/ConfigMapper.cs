using GameInfoFetcher.Models;
using Newtonsoft.Json;

namespace GameInfoFetcher.Mappers;

public static
class ConfigMapper
{
    public static CacheConfig MapToCacheConfig(this IConfigurationRoot config, string sectionName = "Memcached")
    {
        var memcachedSection = config.GetSection(sectionName) ??
            throw new InvalidOperationException($"Section: '{sectionName}' is missing in config.");

        var memcachedConfig = memcachedSection.Get<CacheConfig>();

        // if config is a string instead of json (env var set by github) we deserialize value
        return memcachedConfig == default?
            memcachedConfig = JsonConvert.DeserializeObject<CacheConfig>(memcachedSection.Value ??
                throw new InvalidOperationException("Section: '{sectionName}' could not be deserilized."))
            : memcachedConfig;
    }
}
