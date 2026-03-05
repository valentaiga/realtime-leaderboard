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
            dto.Id = await conn.ExecuteScalarAsync<ulong>(
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
                }).WaitAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding match");
            throw;
        }
    }

    public async Task<FilterResult<MatchDto>> GetByFilterAsync(
        FilterDescriptor<ulong>? playerFilter,
        FilterDescriptor<DateTime>? startedAtFilter,
        FilterDescriptor<DateTime>? finishedAtFilter,
        uint limit,
        uint offset,
        CancellationToken ct)
    {
        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine("SELECT id, match_id, started_at, finished_at, COUNT(*) OVER() AS total_count");
        queryBuilder.AppendLine("FROM matches");

        var whereClauseUsed = false;

        if (startedAtFilter is not null)
        {
            queryBuilder.Append(whereClauseUsed ? "AND " : "WHERE ");
            whereClauseUsed = true;
            queryBuilder.Append("started_at ").Append(InterpolateCondition(startedAtFilter)).AppendLine(" @startedAt");
        }

        if (finishedAtFilter is not null)
        {
            queryBuilder.Append(whereClauseUsed ? "AND " : "WHERE ");
            whereClauseUsed = true;
            queryBuilder.Append("finished_at ").Append(InterpolateCondition(finishedAtFilter)).AppendLine(" @finishedAt");
        }

        queryBuilder.AppendLine("ORDER BY started_at DESC");
        queryBuilder.Append("LIMIT ").AppendLine(limit.ToString());
        queryBuilder.Append("OFFSET ").AppendLine(offset.ToString());

        var query = queryBuilder.ToString();
        
        var result = new FilterResult<MatchDto>();

        try
        {
            await using var conn = dbConnectionFactory.GetConnection("ChronicleDb");
            await conn.OpenAsync(ct);

            var data = (await conn.QueryAsync<MatchDtoExtended>(
                query,
                new
                {
                    PlayerId = playerFilter?.Value,
                    StartedAt = startedAtFilter?.Value,
                    FinishedAt = finishedAtFilter?.Value
                }).WaitAsync(ct)).ToArray();
            result.Data = data;
            result.Total = data.Length > 0 ? data[0].TotalCount : 0;
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
        _ => throw new ArgumentOutOfRangeException(nameof(filterDescriptor.Operator), filterDescriptor.Operator, "Probably operator shouldnt be interpolated")
    };

    public class MatchDtoExtended : MatchDto
    {
        public ulong TotalCount { get; set; }
    }
}
