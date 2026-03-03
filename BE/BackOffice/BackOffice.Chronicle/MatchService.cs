using BackOffice.MQ.Messages.MatchStatus;

namespace BackOffice.Chronicle;

public class MatchService
{
    public async Task SaveFinishedMatchAsync(Guid matchId, MatchFinishedEvent @event, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}