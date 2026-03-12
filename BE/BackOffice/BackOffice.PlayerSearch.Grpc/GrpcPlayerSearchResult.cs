using System.Runtime.Serialization;

namespace BackOffice.PlayerSearch.Grpc;

[DataContract]
public class GrpcPlayerSearchResult
{
    [DataMember(Order = 1)]
    public IEnumerable<GrpcPlayer> Top { get; set; } = [];
}