FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web/feat.web/feat.web.csproj", "web/feat.web/"]
COPY ["common/feat.common/feat.common.csproj", "common/feat.common/"]
RUN dotnet restore "web/feat.web/feat.web.csproj"
COPY . .
WORKDIR "/src/web/feat.web"
RUN dotnet build "./feat.web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM node:24 AS node
WORKDIR /src
COPY "web/feat.web/package.json" "web/feat.web/package-lock.json" "web/feat.web/Gulpfile.js" ./
COPY "web/feat.web/AssetSrc/" "./AssetSrc/"
RUN npm install --ignore-scripts
RUN npm install --ignore-scripts -g gulp
RUN npm install --ignore-scripts gulp
RUN gulp dev

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./feat.web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=node /src/wwwroot ./wwwroot
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "feat.web.dll"]
