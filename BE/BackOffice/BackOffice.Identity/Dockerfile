FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BackOffice/BackOffice.Identity/BackOffice.Identity.csproj", "BackOffice/BackOffice.Identity/"]
RUN dotnet restore "BackOffice/BackOffice.Identity/BackOffice.Identity.csproj"
COPY . .
WORKDIR "/src/BackOffice/BackOffice.Identity"
RUN dotnet build "./BackOffice.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BackOffice.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackOffice.Identity.dll"]
