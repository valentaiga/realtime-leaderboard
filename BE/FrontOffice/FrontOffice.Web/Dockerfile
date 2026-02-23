FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5110

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install NativeAOT build prerequisites
RUN apt-get update
RUN apt-get install -y --no-install-recommends clang zlib1g-dev

COPY ["FrontOffice/FrontOffice.Web/", "FrontOffice/FrontOffice.Web/"]
COPY ["Common/Common.Grpc.Client/", "Common/Common.Grpc.Client/"]
COPY ["BackOffice/ProtoFiles/", "BackOffice/ProtoFiles/"]

RUN dotnet restore "FrontOffice/FrontOffice.Web/FrontOffice.Web.csproj"

RUN dotnet build "FrontOffice/FrontOffice.Web/FrontOffice.Web.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FrontOffice/FrontOffice.Web/FrontOffice.Web.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false --self-contained true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./FrontOffice.Web"]
