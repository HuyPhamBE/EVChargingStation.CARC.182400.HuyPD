# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["EVChargingStation.CARC.WebAPI.HuyPD/EVChargingStation.CARC.WebAPI.HuyPD.csproj", "EVChargingStation.CARC.WebAPI.HuyPD/"]
COPY ["EVChargingStation.CARC.Application.HuyPD/EVChargingStation.CARC.Application.HuyPD.csproj", "EVChargingStation.CARC.Application.HuyPD/"]
COPY ["EVChargingStation.CARC.Infrastructure.HuyPD/EVChargingStation.CARC.Infrastructure.HuyPD.csproj", "EVChargingStation.CARC.Infrastructure.HuyPD/"]
COPY ["EVChargingStation.CARC.Domain.HuyPD/EVChargingStation.CARC.Domain.HuyPD.csproj", "EVChargingStation.CARC.Domain.HuyPD/"]
RUN dotnet restore "./EVChargingStation.CARC.WebAPI.HuyPD/EVChargingStation.CARC.WebAPI.HuyPD.csproj"
COPY . .
WORKDIR "/src/EVChargingStation.CARC.WebAPI.HuyPD"
RUN dotnet build "./EVChargingStation.CARC.WebAPI.HuyPD.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EVChargingStation.CARC.WebAPI.HuyPD.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EVChargingStation.CARC.WebAPI.HuyPD.dll"]