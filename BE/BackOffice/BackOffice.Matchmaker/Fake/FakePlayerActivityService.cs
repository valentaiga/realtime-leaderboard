using System.Diagnostics;
using BackOffice.Matchmaker.Services;
using Microsoft.Extensions.Options;

namespace BackOffice.Matchmaker.Fake;

public class FakePlayerActivityService(MatchService matchService, IOptions<FakePlayerActivityOptions> options, ILogger<FakePlayerActivityService> logger) : BackgroundService
{
    private const int MatchPlayersCount = 10;
    private readonly TimeSpan _second = TimeSpan.FromSeconds(1);

    /// <summary> Simulates player activity in matchmaker. </summary>
    /// <remarks>
    /// In real life player connects to matchmaker, matchmaker matches players in match based on individual ELO. <br/>
    /// Since project is created for education purpose only, we're assuming players activity based on requirement numbers. 
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (!options.Value.IsEnabled)
        {
            logger.LogInformation($"{nameof(FakePlayerActivityService)} is disabled");
            return;
        }

        var matchesToCreate = 0d;
        var matchesPerSecond = (double)options.Value.PlayersConnectedPerMinute / 60 / MatchPlayersCount;
        var matchPlayersBuffer = new long[MatchPlayersCount];

        while (!ct.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Debug) && matchesToCreate > 0)
                logger.LogDebug("Starting {MatchesCount} matches", Math.Ceiling(matchesToCreate));
            var ts = Stopwatch.GetTimestamp();
            for (;matchesToCreate > 0;matchesToCreate--)
            {
                ct.ThrowIfCancellationRequested();
                GetPlayersForMatch(options.Value, ref matchPlayersBuffer);
                await matchService.StartMatchAsync(matchPlayersBuffer, ct);
            }

            var spentTime = Stopwatch.GetElapsedTime(ts);
            await Task.Delay(_second, ct);
            matchesToCreate += matchesPerSecond * (_second + spentTime).TotalSeconds;
        }
    }

    private static void GetPlayersForMatch(FakePlayerActivityOptions options, ref long[] playersBuffer)
    {
        var availablePlayerIds = options.AvailablePlayerIds.AsSpan();
        var startIndex = Random.Shared.Next(0, availablePlayerIds.Length - MatchPlayersCount);
        availablePlayerIds.Slice(startIndex, MatchPlayersCount).CopyTo(playersBuffer);
        Random.Shared.Shuffle(playersBuffer);
    }
}