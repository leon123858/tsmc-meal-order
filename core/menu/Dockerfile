FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
ENV ASPNETCORE_ENVIRONMENT="Production"
EXPOSE ${PORT}

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /core
COPY ["core/core.csproj", "core/"]
COPY ["menu/menu.csproj", "menu/"]
RUN dotnet restore "menu/menu.csproj"
COPY . .
WORKDIR "/core/menu"
RUN dotnet build "menu.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "menu.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "menu.dll"]
