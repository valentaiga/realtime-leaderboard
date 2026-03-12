using System.Runtime.Serialization;

namespace BackOffice.PlayerSearch.Grpc;

[DataContract]
public class FindPlayersByUsernameRequest
{
    [DataMember(Order = 1)]
    public string Username { get; set; } = null!;
}