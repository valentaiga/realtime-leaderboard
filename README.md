# leaderboard-system-design

## What is a leaderboard?
The leaderboard is a dashboard for displaying the ranking of the players in a competitive event such as an online multiplayer game.
In the internet-connected world, leaderboards for popular games can be shared by millions of players.
The players are assigned a score for the completion of tasks and the player with the highest score appears at the top of the leaderboard.  

The following are the benefits of having a leaderboard:
- enhance social aspects of the game
- increase game activity

Leaderboards are also useful in gamification in fitness, education, loyalty programs, or community participation. The following are the broad categories of leaderboards2:
- **Absolute leaderboard**: ranks all players by a global measure. The top-ranked players such as the top 10 players are typically displayed by the absolute leaderboards.
- **Relative leaderboard**: ranks players in such a way that players are grouped according to certain criteria. The surrounding ranked players of a particular player is displayed by the relative leaderboards.

## How does the Leaderboard work?

The Redis sorted set is the data type for the use cases and access patterns in the leaderboard requirements.
The sorted set is an in-memory data type that makes it trivial to generate the leaderboard in real-time for millions of players.
The current rank of the players can be fetched in logarithmic time.
In simple words, the leaderboard is a set sorted by the score.
The score and leaderboard records are persisted on the relational database as well to support complex queries.

## Requirements
### Functional Requirements
- [ ] The client (player) can view the top 10 players on the leaderboard in real-time (absolute leaderboard)
- [ ] The client can view a specific player’s rank and score
- [ ] The client can view the surrounding ranked players to a particular player (relative leaderboard)
- [ ] The client can receive score updates through push notifications
- [ ] The leaderboard can be configured for global, regional, and friend circles
- [ ] The client can view the historical game scores and historical leaderboards
- [ ] The leaderboards can rank players based on gameplay on a daily, weekly, or monthly basis
- [ ] The clients can update the leaderboard in a fully distributed manner across the globe
- [x] The leaderboard should support thousands of concurrent players
- [ ] The client can view games history with rating changes
- [ ] The client can login to view own statistics 

### Non-Functional Requirements
- [x] High availability
- [x] Low latency
```markdown
- Websocket connection from client to web for realtime-observing any leaderboard changes
- Distributed cache for idempotent requests
- Internal services communication via gRPC through HTTP2
- Database queries are well optimized
```
- [x] Scalability
```markdown
- BackOffice services are ready to scale horizontally
- FrontOffice is ready to scale for high clients count
```
- [x] Reliability
```markdown
- Kafka cluster as MQ, topics with replication factor
```
- [x] Minimal operational overhead
```markdown
- All .NET services are made with NativeAOT
- Distributed cache allows to return cache directly ignoring db calls
```
- [x] Observability
```markdown
- OpenTelemetry tracing and logging integrated into services
```

## System Design

[Link](https://excalidraw.com/#room=313c94cf4dcf72625202,4gJ_fFXYJwT50aGtW2X7KA)

todo vm: insert some information

### .NET Stack
- .NET 10 with NativeAOT
- Dapper for database queries
- EF for migrations
- gRPC for internal services communication
- xUnit for tests (Unit tests, Integration tests)
- OpenTelemetry for logging and tracing

### Technical Solutions

- Postgres as consistent data storage
```markdown
Why:
- Many-to-many support via third table
- Indexes allow to instantly search through player_id/match_id params
- High insert speed without table locks
- Maturity, excellent documentation and a large community
- Sharding opportunity for scaling
```

- Kafka for inter-service message communication
```markdown
Why:
- Services will be able to work if consumers are unavailable
- Topic ReplicationFactor with NumPartitions guarantee important messages distribution
- Easy scaling and high availability via clustering and partitioning
- Allows to increase number of consumers(services) to provide faster messages processing
```

- NativeAOT support
```markdown
Why:
- Faster startup time (on horizontal scaling)
- Reduced execution overhead
- Self-contained executables (Simplified distribution via standalone application)
- Resource efficiency (smaller memory footprint)
```

- Automatic proto files generation based on c# interface
```markdown
Why:
- Faster development time in code-first paradigm
```

## Hosting

### Environment Variables
`.env` file in repository root should contain next values:
```markdown
OTEL_EXPORTER_OTLP_PROTOCOL=<change>
OTEL_EXPORTER_OTLP_ENDPOINT=<change>
OTEL_EXPORTER_OTLP_HEADERS=<change>
ASPNETCORE_ENVIRONMENT=Development
```

### Docker Compose
Everything but Grafana Stack is hosting in docker via compose:  
`docker compose up -d` for local testing  
[Grafana Cloud Stack](https://grafana.com/products/cloud/) can be used testing 