using System.Diagnostics;
using BackOffice.Matchmaker.Services;
using Microsoft.Extensions.Options;

namespace BackOffice.Matchmaker.Fake;

public class FakePlayerActivityService(MatchService matchService, IOptions<FakePlayerActivityOptions> options) : BackgroundService
{
    private const int MatchPlayersCount = 10;
    private readonly TimeSpan _second = TimeSpan.FromSeconds(1);

    /// <summary> Simulates player activity in matchmaker. </summary>
    /// <remarks>
    /// In real life player connects to matchmaker, matchmaker matches players in match based on individual ELO. <br/>
    /// Since project is created for education purpose only, we're assuming players activity based on requirement numbers. 
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var matchesToCreate = 0d;
        var matchesPerSecond = (double)options.Value.PlayersConnectedPerMinute / 60 / MatchPlayersCount;
        var matchPlayersBuffer = new ulong[MatchPlayersCount];

        while (!stoppingToken.IsCancellationRequested)
        {
            var ts = Stopwatch.GetTimestamp();
            for (;matchesToCreate > 0;matchesToCreate--)
            {
                GetPlayersForMatch(options.Value, ref matchPlayersBuffer);
                await matchService.StartMatchAsync(matchPlayersBuffer, stoppingToken);
            }

            var spentTime = Stopwatch.GetElapsedTime(ts);
            await Task.Delay(_second, stoppingToken);
            matchesToCreate += matchesPerSecond * (_second + spentTime).TotalSeconds;
        }
    }

    private static void GetPlayersForMatch(FakePlayerActivityOptions options, ref ulong[] playersBuffer)
    {
        var availablePlayerIds = options.AvailablePlayerIds.AsSpan();
        var startIndex = Random.Shared.Next(0, availablePlayerIds.Length - MatchPlayersCount);
        availablePlayerIds.Slice(startIndex, MatchPlayersCount).CopyTo(playersBuffer);
    }
}