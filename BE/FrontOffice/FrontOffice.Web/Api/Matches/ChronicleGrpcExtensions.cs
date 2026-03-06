using BackOffice.Chronicle.Grpc;
using Common.Filtering;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using FilterOperator = BackOffice.Chronicle.Grpc.FilterOperator;

namespace FrontOffice.Web.Api.Matches;

public static class ChronicleGrpcExtensions
{
    public static GrpcFilterDescriptor_Int64 ToGrpcFilterDescriptor(this FilterDescriptor<long> descriptor) =>
        new()
        {
            Value = descriptor.Value,
            Operator = (FilterOperator)(byte)descriptor.Operator,
        };

    public static GrpcFilterDescriptor_DateTime ToGrpcFilterDescriptor(this FilterDescriptor<DateTime> descriptor) =>
        new()
        {
            Value = Timestamp.FromDateTime(descriptor.Value),
            Operator = (FilterOperator)(byte)descriptor.Operator,
        };

    public static GrpcFilterDescriptor_Boolean ToGrpcFilterDescriptor(this bool value) =>
        new()
        {
            Value = value,
            Operator = FilterOperator.Equals,
        };

    public static IEnumerable<Match> FromRepeatedField(this RepeatedField<GrpcMatchInfo> matches) =>
        matches.Select(x => new Match
        {
            MatchId = x.MatchId,
            StartedAt = x.StartedAt.ToDateTime(),
            FinishedAt = x.FinishedAt.ToDateTime(),
            Players = x.Players.Select(FromGrpc)
        });

    private static MatchPlayer FromGrpc(this GrpcMatchPlayer player) =>
        new()
    {
        PlayerId = player.PlayerId,
        IsWin = player.IsWin
    };
}