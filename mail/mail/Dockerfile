FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

# in GCP should be set to "Production" in cloud run, can not be set in dockerfile
# ENV ASPNETCORE_ENVIRONMENT="Production"

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["mail/mail.csproj", "mail/"]
RUN dotnet restore "mail/mail.csproj"
COPY . .
WORKDIR "/src/mail"
RUN dotnet build "mail.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "mail.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mail.dll"]
