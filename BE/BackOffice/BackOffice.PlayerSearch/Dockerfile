FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5040

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install NativeAOT build prerequisites
RUN apt-get update
RUN apt-get install -y --no-install-recommends clang zlib1g-dev

COPY ["BackOffice/BackOffice.PlayerSearch/", "BackOffice/BackOffice.PlayerSearch/"]
COPY ["Common/Common.Grpc.Server/", "Common/Common.Grpc.Server/"]
COPY ["Common/Common.Primitives/", "Common/Common.Primitives/"]
COPY ["Common/Common.OpenTelemetry/", "Common/Common.OpenTelemetry/"]
COPY ["BackOffice/ProtoFiles/", "BackOffice/ProtoFiles/"]

RUN dotnet restore "BackOffice/BackOffice.PlayerSearch/BackOffice.PlayerSearch.csproj"

RUN dotnet build "BackOffice/BackOffice.PlayerSearch/BackOffice.PlayerSearch.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BackOffice/BackOffice.PlayerSearch/BackOffice.PlayerSearch.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false --self-contained true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./BackOffice.PlayerSearch"]
