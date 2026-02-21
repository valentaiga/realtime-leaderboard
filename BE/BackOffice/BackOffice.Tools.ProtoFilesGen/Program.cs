using BackOffice.Identity.Grpc;
using BackOffice.Tools.ProtoFilesGen;

Console.WriteLine("ProtoFilesGen: Args [" + string.Join(",", args) + "]");

// ===================== update only this part =====================

var grpcInterfaces = new Dictionary<string, Type[]>
{
    { "Identity.Grpc", [typeof(IIdentityApi)] }
};

// ====================== do not update below ======================

foreach (var interfaceName in args.Select(x => x[2..]))
{
    if (!grpcInterfaces.TryGetValue(interfaceName, out var types))
        throw new KeyNotFoundException("Project reference not found for proto file generation");

    await Generator.GenerateAsync(interfaceName, types);
}
