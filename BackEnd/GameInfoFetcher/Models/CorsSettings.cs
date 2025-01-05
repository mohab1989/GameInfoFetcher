
namespace GameInfoFetcher.Models;

class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public bool AllowAnyHeader { get; set; } = false;
    public bool AllowAnyMethod { get; set; } = false;
    public string[] AllowedHeaders { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();
};
