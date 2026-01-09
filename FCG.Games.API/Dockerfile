# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG AWS_ACCOUNT_ID
ARG AWS_REGION
ARG BASE_RUNTIME_IMAGE=${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com/aspnet-newrelic-runtime:latest

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FCG.Games.API/FCG.Games.API.csproj", "FCG.Games.API/"]
COPY ["FCG.Games.Application/FCG.Games.Application.csproj", "FCG.Games.Application/"]
COPY ["FCG.Games.Domain/FCG.Games.Domain.csproj", "FCG.Games.Domain/"]
COPY ["FCG.Games.Infra/FCG.Games.Infra.csproj", "FCG.Games.Infra/"]
RUN dotnet restore "./FCG.Games.API/FCG.Games.API.csproj"
COPY . .
WORKDIR "/src/FCG.Games.API"
RUN dotnet build "./FCG.Games.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FCG.Games.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Stage final: usa a imagem do ECR com aspnet + new relic
FROM ${BASE_RUNTIME_IMAGE} AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Games.API.dll"]