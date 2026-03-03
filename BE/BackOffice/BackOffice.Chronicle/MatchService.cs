using BackOffice.Chronicle.Data.Models;
using BackOffice.Chronicle.Database;
using BackOffice.MQ.Messages.MatchStatus;
using Common.Filtering;

namespace BackOffice.Chronicle;

public class MatchService(IMatchRepository matchRepository)
{
    public Task SaveFinishedMatchAsync(Guid matchId, MatchFinishedEvent @event, CancellationToken ct)
    {
        var dto = new MatchDto
        {
            MatchId = matchId,
            StartedAt = @event.StartedAt,
            FinishedAt = @event.FinishedAt
        };

        return matchRepository.AddAsync(dto, ct);
    }

    public Task<FilterResult<MatchDto>> GetByFilterAsync(
        FilterDescriptor<ulong>? playerFilter,
        FilterDescriptor<DateTime>? startedAtFilter,
        FilterDescriptor<DateTime>? finishedAtFilter,
        uint limit,
        uint offset,
        CancellationToken ct) => matchRepository.GetByFilterAsync(playerFilter, startedAtFilter, finishedAtFilter, limit, offset, ct);
}