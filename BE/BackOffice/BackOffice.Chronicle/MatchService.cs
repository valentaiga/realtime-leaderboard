using BackOffice.Chronicle.Data.Models;
using BackOffice.Chronicle.Database;
using BackOffice.MQ.Messages.MatchStatus;
using BackOffice.MQ.Messages.PlayerUpdate;
using Common.Filtering;

namespace BackOffice.Chronicle;

public class MatchService(IMatchRepository matchRepository)
{
    public Task SaveFinishedMatchAsync(string matchId, MatchFinishedEvent @event, CancellationToken ct)
    {
        var dto = new MatchDto
        {
            MatchId = matchId,
            StartedAt = @event.StartedAt,
            FinishedAt = @event.FinishedAt,
            Players = new List<MatchPlayerDto>(11)
        };

        // fill players section
        foreach (var playerId in @event.Winners.AsSpan())
            dto.Players.Add(new MatchPlayerDto { PlayerId =  playerId, IsWin = true });
        foreach (var playerId in @event.Losers.AsSpan())
            dto.Players.Add(new MatchPlayerDto { PlayerId =  playerId, IsWin = false });

        return matchRepository.AddAsync(dto, ct);
    }

    public Task UpdatePlayerEloChangeAsync(long playerId, PlayerEloChangedEvent @event, CancellationToken ct) =>
        matchRepository.UpdatePlayerEloChange(@event.MatchId, playerId, @event.EloChange, ct);

    public Task<FilterResult<MatchDto>> GetByFilterAsync(
        FilterDescriptor<long>? playerFilter,
        bool? playerWonFilter,
        FilterDescriptor<DateTime>? startedAtFilter,
        FilterDescriptor<DateTime>? finishedAtFilter,
        long limit,
        long offset,
        CancellationToken ct) => matchRepository.GetByFilterAsync(playerFilter, playerWonFilter, startedAtFilter, finishedAtFilter, limit, offset, ct);
}