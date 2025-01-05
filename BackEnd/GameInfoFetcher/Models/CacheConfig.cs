namespace GameInfoFetcher.Models;

public record struct CacheConfig(string ServerAddress, int ServerPort, string UserName, string Password);
