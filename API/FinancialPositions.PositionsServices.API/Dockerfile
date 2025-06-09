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
COPY ["API/FinancialPositions.PositionsServices.API/FinancialPositions.PositionsServices.API.csproj", "API/FinancialPositions.PositionsServices.API/"]
COPY ["Models/FinancialPositions.PositionsServices.Core/FinancialPositions.PositionsServices.Core.csproj", "Models/FinancialPositions.PositionsServices.Core/"]
COPY ["Models/FinancialPositions.Shared.Core/FinancialPositions.Shared.Core.csproj", "Models/FinancialPositions.Shared.Core/"]
COPY ["Repositories/FinancialPositions.PositionsServices.Repositories.EF/FinancialPositions.PositionsServices.Repositories.EF.csproj", "Repositories/FinancialPositions.PositionsServices.Repositories.EF/"]
COPY ["Services/FinancialPositions.PositionsServices.Services.Core/FinancialPositions.PositionsServices.Services.Core.csproj", "Services/FinancialPositions.PositionsServices.Services.Core/"]
COPY ["Services/FinancialPositions.PositionsServices.Services/FinancialPositions.PositionsServices.Services.csproj", "Services/FinancialPositions.PositionsServices.Services/"]
RUN dotnet restore "./API/FinancialPositions.PositionsServices.API/FinancialPositions.PositionsServices.API.csproj"
COPY . .
WORKDIR "/src/API/FinancialPositions.PositionsServices.API"
RUN dotnet build "./FinancialPositions.PositionsServices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinancialPositions.PositionsServices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialPositions.PositionsServices.API.dll"]