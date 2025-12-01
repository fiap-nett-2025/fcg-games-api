# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=49fea977ae8cb8915399b2c027e24687FFFFNRAL \
NEW_RELIC_APP_NAME="fcg-game-api"

# ✅ Troca para root para conseguir instalar pacotes
USER root

RUN apt-get update && \
    apt-get install -y wget ca-certificates gnupg && \
    mkdir -p /usr/share/keyrings && \
    wget -O- https://download.newrelic.com/548C16BF.gpg | gpg --dearmor -o /usr/share/keyrings/newrelic-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/newrelic-archive-keyring.gpg] http://apt.newrelic.com/debian/ newrelic non-free" > /etc/apt/sources.list.d/newrelic.list && \
    apt-get update && \
    apt-get install -y newrelic-dotnet-agent && \
    rm -rf /var/lib/apt/lists/*

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FCG.Games.API/FCG.Games.API.csproj", "src/FCG.Games.API/"]
COPY ["src/FCG.Games.Application/FCG.Games.Application.csproj", "src/FCG.Games.Application/"]
COPY ["src/FCG.Games.Domain/FCG.Games.Domain.csproj", "src/FCG.Games.Domain/"]
COPY ["src/FCG.Games.Infra/FCG.Games.Infra.csproj", "src/FCG.Games.Infra/"]
RUN dotnet restore "./src/FCG.Games.API/FCG.Games.API.csproj"
COPY . .
WORKDIR "/src/src/FCG.Games.API"
RUN dotnet build "./FCG.Games.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FCG.Games.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Games.API.dll"]