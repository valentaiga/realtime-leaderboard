using BackOffice.Matchmaker.Fake;
using BackOffice.Matchmaker.Services;
using Common.MQ;
using Common.MQ.Messages;
using Common.Primitives;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<FakePlayerActivityOptions>().Services
    .AddHostedService<FakePlayerActivityService>();

builder.Services
    .AddSingleton<MatchService>()
    .AddSingleton(_ => new FixedSizeArrayPool<ulong>(5))
    .AddSingleton(typeof(ObjectRingBuffer<>))
    .AddUnboundedChannel<FinishedMatchMessage>()
    .AddMessageSender<FinishedMatchNotifier, FinishedMatchMessage>();

var app = builder.Build();

app.Run();