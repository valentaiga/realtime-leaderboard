using BackOffice.Chronicle.Grpc;
using Common.Filtering;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using FilterOperator = BackOffice.Chronicle.Grpc.FilterOperator;

namespace FrontOffice.Web.Api.Player;

public static class ChronicleGrpcExtensions
{
    public static GrpcFilterDescriptor_UInt64 ToGrpcFilterDescriptor(this FilterDescriptor<ulong> descriptor) =>
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

    public static IEnumerable<Match> FromRepeatedField(this RepeatedField<MatchInfo> matches) =>
        matches.Select(x => new Match
        {
            StartedAt = x.StartedAt.ToDateTime(),
            FinishedAt = x.FinishedAt.ToDateTime(),
            Losers = x.Losers.ToArray(),
            Winners = x.Winners.ToArray()
        });

}