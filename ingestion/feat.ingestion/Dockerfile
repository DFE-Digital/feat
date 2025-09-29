FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ingestion/feat.ingestion/feat.ingestion.csproj", "ingestion/feat.ingestion/"]
RUN dotnet restore "ingestion/feat.ingestion/feat.ingestion.csproj"
COPY . .
WORKDIR "/src/ingestion/feat.ingestion"
RUN dotnet build "./feat.ingestion.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./feat.ingestion.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "feat.ingestion.dll"]
