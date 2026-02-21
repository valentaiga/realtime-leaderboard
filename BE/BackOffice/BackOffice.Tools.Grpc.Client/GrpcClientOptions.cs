using Grpc.Core;

namespace BackOffice.Tools.Grpc.Client;

public class GrpcClientOptions<TGrpcService> where TGrpcService : ClientBase
{
    public string Endpoint { get; set; } = null!;
}
