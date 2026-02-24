namespace BackOffice.Matchmaker.Fake;

public class FakePlayerActivityOptions
{
    public uint PlayersConnectedPerMinute { get; set; } = 1000 * 10; // 1000 matches per minute
    public ulong[] AvailablePlayerIds { get; set; } = Enumerable.Range(0, 1_000_000).Select(x => (ulong)x).ToArray();
}