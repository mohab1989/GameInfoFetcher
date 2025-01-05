using GameInfoFetcher.Mappers;
using GameInfoFetcher.Models;
using GameInfoFetcher.Services;
using GameInfoFetcher.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<CorsSettings>() ??
    throw new InvalidOperationException("CorsSettings must be defined");

builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCorsPolicy", policy =>
    {
        if (corsSettings.AllowedOrigins.Any())
            policy.WithOrigins(corsSettings.AllowedOrigins.ToArray());

        if (corsSettings.AllowAnyHeader)
            policy.AllowAnyHeader();

        else if (corsSettings.AllowedHeaders.Any())
            policy.WithHeaders(corsSettings.AllowedHeaders.ToArray());

        if (corsSettings.AllowAnyMethod)
            policy.AllowAnyMethod();

        else if (corsSettings.AllowedMethods.Any())
            policy.WithMethods(corsSettings.AllowedMethods.ToArray());
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

builder.Configuration.AddUserSecrets<Program>();

var cachedConfig = builder.Configuration.MapToCacheConfig("Memcached");

builder.Services.AddEnyimMemcached(options =>
{
    options.AddServer(cachedConfig.ServerAddress, cachedConfig.ServerPort);
    options.AddPlainTextAuthenticator("", cachedConfig.UserName, cachedConfig.Password);
});

builder.Services.AddSingleton<ICache<string, GameInfo>, Cache>();

builder.Services.AddHttpClient<IGamesApiClient, RawgClient>(httpClient =>
{
    var configuration = GetSection("RawgConfig", "BaseUrl");
    httpClient.BaseAddress = new Uri(configuration.GetValue<string>("BaseUrl")!);
    return new(httpClient, GetEnvironmentVariable("RawgApiKey"));
});

builder.Services.AddSingleton<IServerlessDbService, LiteDbService>(_
    => new(GetEnvironmentVariable("LiteDbConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

// Use CORS policy
app.UseCors("DynamicCorsPolicy");

app.UseHttpsRedirection();

app.UseEnyimMemcached();

app.UseAuthorization();

app.MapControllers();

app.Run();


string GetEnvironmentVariable(string variableName)
{
    var value = builder.Configuration[variableName];
    return string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"Environment Variable: '{variableName}' was not defined.")
        : value;
}

IConfigurationSection GetSection(string sectionName, params string[] variableNames)
{
    var configSection = builder.Configuration.GetSection(sectionName);

    if (configSection is null)
        throw new InvalidOperationException($"Configuration Section '{sectionName}' is missing.");

    foreach (var variableName in variableNames)
    {
        if (string.IsNullOrWhiteSpace(configSection[variableName]))
            throw new InvalidOperationException(
                $"Environment Variable '{variableName}' in Section '{sectionName}' was not defined.");
    }

    return configSection;
}
