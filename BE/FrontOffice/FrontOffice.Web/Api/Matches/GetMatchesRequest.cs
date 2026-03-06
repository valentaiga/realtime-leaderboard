using Common.Filtering;

namespace FrontOffice.Web.Api.Matches;

public class GetMatchesRequest
{
    public FilterDescriptor<long>? PlayerId { get; set; }
    public FilterDescriptor<DateTime>? StartedAt { get; set; }
    public FilterDescriptor<DateTime>? FinishedAt { get; set; }
    public bool? PlayerWon { get; set; }
    public long Limit { get; set; } = 50;
    public long Offset { get; set; }
}