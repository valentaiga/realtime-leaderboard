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
COPY ["BackOffice/BackOffice.Chronicle.Data/", "BackOffice/BackOffice.Chronicle.Data/"]
COPY ["Common/Common.Filtering/", "Common/Common.Filtering/"]
COPY ["Common/Common.Grpc.Server/", "Common/Common.Grpc.Server/"]
COPY ["Common/Common.MQ/", "Common/Common.MQ/"]
COPY ["Common/Common.MQ.Kafka.Serializer.MessagePack/", "Common/Common.MQ.Kafka.Serializer.MessagePack/"]
COPY ["Common/Common.MQ.Kafka/", "Common/Common.MQ.Kafka/"]
COPY ["Common/Common.Primitives/", "Common/Common.Primitives/"]
COPY ["BackOffice/BackOffice.MQ.Messages/", "BackOffice/BackOffice.MQ.Messages/"]
COPY ["BackOffice/ProtoFiles/", "BackOffice/ProtoFiles/"]

# docker migrations
COPY ["BackOffice/BackOffice.Chronicle.Migrations/appsettings.Docker.json", "/app/migrations/appsettings.json"]
COPY ["BackOffice/BackOffice.Chronicle.Migrations/", "BackOffice/BackOffice.Chronicle.Migrations/"]
COPY ["Common/Common.Migrations.Postgres/", "Common/Common.Migrations.Postgres/"]

RUN dotnet restore "BackOffice/BackOffice.Chronicle/BackOffice.Chronicle.csproj"
RUN dotnet build "BackOffice/BackOffice.Chronicle/BackOffice.Chronicle.csproj" -c $BUILD_CONFIGURATION

RUN dotnet restore "BackOffice/BackOffice.Chronicle.Migrations/BackOffice.Chronicle.Migrations.csproj"
RUN dotnet build "BackOffice/BackOffice.Chronicle.Migrations/BackOffice.Chronicle.Migrations.csproj" -c $BUILD_CONFIGURATION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BackOffice/BackOffice.Chronicle/BackOffice.Chronicle.csproj" -c $BUILD_CONFIGURATION /p:UseAppHost=false /p:TrimMode=partial --self-contained true -o /app/publish

# create migrations bundle
RUN dotnet tool install -g dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --self-contained --project BackOffice/BackOffice.Chronicle.Data/ --startup-project BackOffice/BackOffice.Chronicle.Migrations/ -r linux-x64 --configuration $BUILD_CONFIGURATION -o /app/migrations/efbundle

FROM base AS final
WORKDIR /app/migrations
COPY --from=publish /app/migrations .
RUN ["./efbundle"]

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./BackOffice.Chronicle"]
