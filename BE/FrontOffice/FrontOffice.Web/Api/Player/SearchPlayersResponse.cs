namespace FrontOffice.Web.Api.Player;

public class SearchPlayersResponse
{
    public IEnumerable<KeyValuePair<long, string>> Players { get; set; } = null!;
}