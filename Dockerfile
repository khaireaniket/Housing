FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Housing.API/Housing.API.csproj", "src/Housing.API/"]
RUN dotnet restore "src/Housing.API/Housing.API.csproj"
COPY . .
WORKDIR "/src/src/Housing.API"
RUN dotnet build "Housing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Housing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Housing.API.dll"]