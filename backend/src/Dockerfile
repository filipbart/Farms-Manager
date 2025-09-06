FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FarmsManager.sln", "."]
COPY ["FarmsManager.Api/FarmsManager.Api.csproj", "FarmsManager.Api/"]
COPY ["FarmsManager.Application/FarmsManager.Application.csproj", "FarmsManager.Application/"]
COPY ["FarmsManager.Domain/FarmsManager.Domain.csproj", "FarmsManager.Domain/"]
COPY ["FarmsManager.HostBuilder/FarmsManager.HostBuilder.csproj", "FarmsManager.HostBuilder/"]
COPY ["FarmsManager.Infrastructure/FarmsManager.Infrastructure.csproj", "FarmsManager.Infrastructure/"]
COPY ["FarmsManager.Shared/FarmsManager.Shared.csproj", "FarmsManager.Shared/"]

RUN dotnet restore "FarmsManager.sln"

COPY . .

WORKDIR "/src/FarmsManager.Api"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FarmsManager.Api.dll"]
