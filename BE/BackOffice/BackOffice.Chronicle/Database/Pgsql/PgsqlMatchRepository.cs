using System.Data;
using System.Text;
using BackOffice.Chronicle.Data.Models;
using Common.Filtering;
using Dapper;

namespace BackOffice.Chronicle.Database.Pgsql;
[DapperAot]
public class PgsqlMatchRepository(DbConnectionFactory dbConnectionFactory, ILogger<PgsqlMatchRepository> logger) : IMatchRepository
{
    public async Task AddAsync(MatchDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        try
        {
            await using var conn = dbConnectionFactory.GetConnection("ChronicleDb");
            await conn.OpenAsync(ct);
            await using var transaction = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            try
            {
                dto.Id = await conn.ExecuteScalarAsync<long>(
                    """
                    INSERT INTO matches (id, match_id, started_at, finished_at)
                    VALUES (nextval('MatchSeq'), @matchId, @startedAt, @finishedAt)
                    RETURNING id
                    """,
                    new
                    {
                        dto.MatchId,
                        dto.StartedAt,
                        dto.FinishedAt
                    }, transaction).WaitAsync(ct);

                var playersParams = dto.Players.Select(x => new
                {
                    matchId = dto.Id,
                    playerId = x.PlayerId,
                    Win = x.IsWin
                });
                await conn.ExecuteAsync(
                    """
                    INSERT INTO match_players (match_id, player_id, is_win)
                    VALUES (@matchId, @playerId, @win)
                    """,
                    playersParams, transaction);

                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding match");
            throw;
        }
    }

    public async Task<FilterResult<MatchDto>> GetByFilterAsync(
        FilterDescriptor<long>? playerFilter,
        bool? playerWonFilter,
        FilterDescriptor<DateTime>? startedAtFilter,
        FilterDescriptor<DateTime>? finishedAtFilter,
        long limit,
        long offset,
        CancellationToken ct)
    {
        var queryBuilder = new StringBuilder();

        queryBuilder.AppendLine(@"with mq AS (
    SELECT distinct m.id, m.match_id, m.started_at, m.finished_at
    FROM matches AS m
             JOIN match_players AS mp ON mp.match_id = m.id");

        var whereClauseUsed = false;

        if (playerFilter is not null)
        {
            queryBuilder.Append(whereClauseUsed ? "AND " : "WHERE ");
            whereClauseUsed = true;
            queryBuilder.AppendLine("mp.player_id = @playerId");
            if (playerWonFilter.HasValue)
                queryBuilder.AppendLine(" AND mp.is_win = @isWin");
        }

        if (startedAtFilter is not null)
        {
            queryBuilder.Append(whereClauseUsed ? "AND " : "WHERE ");
            whereClauseUsed = true;
            queryBuilder.Append("m.started_at ").Append(InterpolateCondition(startedAtFilter)).AppendLine(" @startedAt");
        }

        if (finishedAtFilter is not null)
        {
            queryBuilder.Append(whereClauseUsed ? "AND " : "WHERE ");
            whereClauseUsed = true;
            queryBuilder.Append("m.finished_at ").Append(InterpolateCondition(finishedAtFilter)).AppendLine(" @finishedAt");
        }

        queryBuilder.AppendLine(")");

        queryBuilder.AppendLine("SELECT id, match_id, started_at, finished_at, (SELECT count(*) FROM mq) AS total_count");
        queryBuilder.AppendLine("FROM mq");

        queryBuilder.AppendLine("ORDER BY started_at DESC");
        queryBuilder.AppendLine("LIMIT @limit");
        queryBuilder.Append("OFFSET @offset");

        var query = queryBuilder.ToString();
        
        var result = new FilterResult<MatchDto>();

        try
        {
            await using var conn = dbConnectionFactory.GetConnection("ChronicleDb");
            await conn.OpenAsync(ct);

            var matches = (await conn.QueryAsync<MatchDtoExtended>(
                query,
                new
                {
                    PlayerId = playerFilter?.Value,
                    StartedAt = startedAtFilter?.Value,
                    FinishedAt = finishedAtFilter?.Value,
                    IsWin = playerWonFilter,
                    Limit = limit,
                    Offset = offset,
                }).WaitAsync(ct)).ToDictionary(x => x.Id);

            // two requests since dapper.AOT doesnt allow to match 2 or more entities in single query
            var matchPlayers = matches.Count == 0
                ? []
                : await conn.QueryAsync<MatchPlayerDto>(
                    "select match_id, player_id, is_win from match_players where match_id = ANY(@matchIds)", 
                new
                {
                    MatchIds = matches.Keys.Select(x => x).ToArray()
                });
            foreach (var mp in matchPlayers)
                matches[mp.MatchId].Players.Add(mp);
            result.Data = matches.Values;
            result.Total = matches.Values.FirstOrDefault()?.TotalCount ?? 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding match");
            throw;
        }

        return result;
    }

    private static string InterpolateCondition<T>(FilterDescriptor<T> filterDescriptor) => filterDescriptor.Operator switch
    {
        FilterOperator.Equals => "=",
        FilterOperator.NotEquals => "<>",
        FilterOperator.GreaterThan => ">",
        FilterOperator.GreaterThanOrEqual => ">=",
        FilterOperator.LessThan => "<",
        FilterOperator.LessThanOrEqual => "<=",
        _ => throw new ArgumentOutOfRangeException(nameof(filterDescriptor.Operator), filterDescriptor.Operator, "Probably operator shouldn't be interpolated")
    };

    public class MatchDtoExtended : MatchDto
    {
        public long TotalCount { get; set; }
    }
}
