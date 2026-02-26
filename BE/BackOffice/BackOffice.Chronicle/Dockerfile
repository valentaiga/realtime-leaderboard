FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5020

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install NativeAOT build prerequisites
RUN apt-get update
RUN apt-get install -y --no-install-recommends clang zlib1g-dev

COPY ["BackOffice/BackOffice.Chronicle/", "BackOffice/BackOffice.Chronicle/"]
COPY ["Common/Common.Grpc.Server/", "Common/Common.Grpc.Server/"]
COPY ["BackOffice/ProtoFiles/", "BackOffice/ProtoFiles/"]

RUN dotnet restore "BackOffice/BackOffice.Chronicle/BackOffice.Chronicle.csproj"

RUN dotnet build "BackOffice/BackOffice.Chronicle/BackOffice.Chronicle.csproj" -c $BUILD_CONFIGURATION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BackOffice/BackOffice.Chronicle/BackOffice.Chronicle.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false /p:TrimMode=partial --self-contained true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./BackOffice.Chronicle"]
