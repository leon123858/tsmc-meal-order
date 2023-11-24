FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
ENV ASPNETCORE_ENVIRONMENT="Production"
EXPOSE ${PORT}

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["order/order.csproj", "order/"]
RUN dotnet restore "order/order.csproj"
COPY . .
WORKDIR "/src/order"
RUN dotnet build "order.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "order.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "order.dll"]
