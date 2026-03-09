using BackOffice.Chronicle.Data.Models;
using BackOffice.Chronicle.Grpc;
using Common.Filtering;
using Google.Protobuf.WellKnownTypes;
using FilterOperator = Common.Filtering.FilterOperator;

namespace BackOffice.Chronicle;

public static class ChronicleGrpcExtensions
{
    public static FilterDescriptor<long> FromGrpcFilterDescriptor(this GrpcFilterDescriptor_Int64 descriptor) =>
        new(descriptor.Value, (FilterOperator)(byte)descriptor.Operator);

    public static FilterDescriptor<DateTime> FromGrpcFilterDescriptor(this GrpcFilterDescriptor_DateTime descriptor) =>
        new(descriptor.Value.ToDateTime(), (FilterOperator)(byte)descriptor.Operator);

    public static IEnumerable<GrpcMatchInfo> ToGrpc(this IEnumerable<MatchDto> matches) =>
        matches.Select(x => new GrpcMatchInfo
        {
            MatchId = x.MatchId,
            StartedAt = x.StartedAt.ToTimestamp(),
            FinishedAt = x.FinishedAt.ToTimestamp(),
            Players =
            {
                x.Players.Select(ToGrpc)
            }
        });

    private static GrpcMatchPlayer ToGrpc(this MatchPlayerDto player) =>
        new()
        {
            PlayerId = player.PlayerId,
            IsWin = player.IsWin
        };
}