using BackOffice.Chronicle.Data.Models;
using Common.Filtering;

namespace BackOffice.Chronicle.Database;

public interface IMatchRepository
{
    Task AddAsync(MatchDto dto, CancellationToken ct);

    Task<FilterResult<MatchDto>> GetByFilterAsync(
        FilterDescriptor<ulong>? playerFilter,
        FilterDescriptor<DateTime>? startedAtFilter,
        FilterDescriptor<DateTime>? finishedAtFilter,
        uint limit,
        uint offset,
        CancellationToken ct);
}