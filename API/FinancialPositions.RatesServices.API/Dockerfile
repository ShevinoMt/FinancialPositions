# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/FinancialPositions.RatesServices.API/FinancialPositions.RatesServices.API.csproj", "API/FinancialPositions.RatesServices.API/"]
COPY ["Models/FinancialPositions.RatesServices.Core/FinancialPositions.RatesServices.Core.csproj", "Models/FinancialPositions.RatesServices.Core/"]
COPY ["Repositories/FinancialPositions.RatesService.Repositories.EF/FinancialPositions.RatesService.Repositories.EF.csproj", "Repositories/FinancialPositions.RatesService.Repositories.EF/"]
COPY ["Services/FinancialPositions.RatesServices.Services.CoinMarketCap/FinancialPositions.RatesServices.Services.CoinMarketCap.csproj", "Services/FinancialPositions.RatesServices.Services.CoinMarketCap/"]
COPY ["Models/FinancialPositions.Shared.Core/FinancialPositions.Shared.Core.csproj", "Models/FinancialPositions.Shared.Core/"]
COPY ["Services/FinancialPositions.RatesServices.Services.Core/FinancialPositions.RatesServices.Services.Core.csproj", "Services/FinancialPositions.RatesServices.Services.Core/"]
RUN dotnet restore "./API/FinancialPositions.RatesServices.API/FinancialPositions.RatesServices.API.csproj"
COPY . .
WORKDIR "/src/API/FinancialPositions.RatesServices.API"
RUN dotnet build "./FinancialPositions.RatesServices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinancialPositions.RatesServices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialPositions.RatesServices.API.dll"]