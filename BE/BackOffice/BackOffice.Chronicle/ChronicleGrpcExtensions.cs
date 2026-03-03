using BackOffice.Chronicle.Data.Models;
using BackOffice.Chronicle.Grpc;
using Common.Filtering;
using Google.Protobuf.WellKnownTypes;
using FilterOperator = Common.Filtering.FilterOperator;

namespace BackOffice.Chronicle;

public static class ChronicleGrpcExtensions
{
    public static FilterDescriptor<ulong> FromGrpcFilterDescriptor(this GrpcFilterDescriptor_UInt64 descriptor) =>
        new(descriptor.Value, (FilterOperator)(byte)descriptor.Operator);

    public static FilterDescriptor<DateTime> FromGrpcFilterDescriptor(this GrpcFilterDescriptor_DateTime descriptor) =>
        new(descriptor.Value.ToDateTime(), (FilterOperator)(byte)descriptor.Operator);

    public static IEnumerable<MatchInfo> ToGrpc(this IEnumerable<MatchDto> dtos) =>
        dtos.Select(x => new MatchInfo
        {
            MatchId = x.MatchId.ToString(),
            StartedAt = x.StartedAt.ToTimestamp(),
            FinishedAt = x.FinishedAt.ToTimestamp()
        });
}