FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ServerStatusBot/ServerStatusBot.csproj", "ServerStatusBot/"]
RUN dotnet restore "ServerStatusBot/ServerStatusBot.csproj"
COPY . .
WORKDIR "/src/ServerStatusBot"
RUN dotnet build "ServerStatusBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerStatusBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerStatusBot.dll"]
