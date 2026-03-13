FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install NativeAOT build prerequisites
RUN apt-get update
RUN apt-get install -y --no-install-recommends clang zlib1g-dev

COPY ["BackOffice/BackOffice.Identity/", "BackOffice/BackOffice.Identity/"]
COPY ["BackOffice/BackOffice.Identity.Data/", "BackOffice/BackOffice.Identity.Data/"]
COPY ["Common/Common.Grpc.Server/", "Common/Common.Grpc.Server/"]
COPY ["Common/Common.Primitives/", "Common/Common.Primitives/"]
COPY ["Common/Common.OpenTelemetry/", "Common/Common.OpenTelemetry/"]
COPY ["Common/Common.MQ.Kafka.Configurator/", "Common/Common.MQ.Kafka.Configurator/"]
COPY ["Common/Common.MQ.Kafka.Serializer.MessagePack/", "Common/Common.MQ.Kafka.Serializer.MessagePack/"]
COPY ["Common/Common.MQ.Kafka/", "Common/Common.MQ.Kafka/"]
COPY ["Common/Common.MQ.Messages/", "Common/Common.MQ.Messages/"]
COPY ["BackOffice/ProtoFiles/", "BackOffice/ProtoFiles/"]

# docker migrations
COPY ["BackOffice/BackOffice.Identity.Migrations/appsettings.Docker.json", "/app/migrations/appsettings.json"]
COPY ["BackOffice/BackOffice.Identity.Migrations/", "BackOffice/BackOffice.Identity.Migrations/"]
COPY ["Common/Common.Migrations.Postgres/", "Common/Common.Migrations.Postgres/"]
RUN dotnet restore "BackOffice/BackOffice.Identity.Migrations/BackOffice.Identity.Migrations.csproj"
RUN dotnet build "BackOffice/BackOffice.Identity.Migrations/BackOffice.Identity.Migrations.csproj" -c $BUILD_CONFIGURATION

RUN dotnet restore "BackOffice/BackOffice.Identity/BackOffice.Identity.csproj"

RUN dotnet build "BackOffice/BackOffice.Identity/BackOffice.Identity.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BackOffice/BackOffice.Identity/BackOffice.Identity.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false --self-contained true -o /app/publish

# create migrations bundle
RUN dotnet tool install -g dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --self-contained --project BackOffice/BackOffice.Identity.Data/ --startup-project BackOffice/BackOffice.Identity.Migrations/ -r linux-x64 --configuration $BUILD_CONFIGURATION -o /app/migrations/efbundle

FROM base AS final
WORKDIR /app/migrations
COPY --from=publish /app/migrations .
RUN ["./efbundle"]

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./BackOffice.Identity"]
