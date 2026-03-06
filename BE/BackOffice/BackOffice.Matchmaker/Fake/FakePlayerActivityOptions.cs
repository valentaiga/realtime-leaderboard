namespace BackOffice.Matchmaker.Fake;

public class FakePlayerActivityOptions
{
    public bool IsEnabled { get; set; }
    public uint PlayersConnectedPerMinute { get; set; } = 1000 * 10; // 1000 matches per minute
    public long[] AvailablePlayerIds { get; set; } = Enumerable.Range(0, 100_000).Select(x => (long)x).ToArray();
}