using BackOffice.Chronicle.Data.Models;
using Common.Filtering;

namespace BackOffice.Chronicle.Database;

public interface IMatchRepository
{
    Task AddAsync(MatchDto dto, CancellationToken ct);
    
    Task UpdatePlayerEloChange(string matchId, long playerId, int eloChange, CancellationToken ct);

    Task<FilterResult<MatchDto>> GetByFilterAsync(
        FilterDescriptor<long>? playerFilter,
        bool? playerWonFilter,
        FilterDescriptor<DateTime>? startedAtFilter,
        FilterDescriptor<DateTime>? finishedAtFilter,
        long limit,
        long offset,
        CancellationToken ct);
}