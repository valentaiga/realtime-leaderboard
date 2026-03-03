using Common.Filtering;

namespace FrontOffice.Web.Api.Player;

[FilterSpecification]
public sealed class MatchFilterRequest : FilterRequest
{
    [FilterCriteria(FilterOperator.Equals)]
    public ulong PlayerId { get; set; }

    [FilterCriteria(FilterOperator.GreaterThan, FilterOperator.LessThan)]
    public DateTime StartedAt { get; set; }

    [FilterCriteria(FilterOperator.GreaterThan, FilterOperator.LessThan)]
    public DateTime FinishedAt { get; set; }
}