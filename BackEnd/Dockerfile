FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY "GameInfoFetcher/GameInfoFetcher.csproj" .

RUN dotnet restore "GameInfoFetcher.csproj"

COPY "GameInfoFetcher" .

ARG BUILD_CONFIG=Release

RUN dotnet build "GameInfoFetcher.csproj" -c $BUILD_CONFIG -o /app/bin/build/$BUILD_CONFIG

RUN dotnet publish "GameInfoFetcher.csproj" -p:UseAppHost=false -c $BUILD_CONFIG -o "/app/bin/publish/$BUILD_CONFIG"


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS deploy

USER app

WORKDIR /app

ARG BUILD_CONFIG=Release

COPY --from=build "/app/bin/publish/$BUILD_CONFIG" .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "GameInfoFetcher.dll"]
