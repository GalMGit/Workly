FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/backend/Hosts/Workly.Api/Workly.Api.csproj", "src/backend/Hosts/Workly.Api/"]
RUN dotnet restore "src/backend/Hosts/Workly.Api/Workly.Api.csproj"
COPY . .
WORKDIR "/src/src/backend/Hosts/Workly.Api"
RUN dotnet build "./Workly.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Workly.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Workly.Api.dll"]
