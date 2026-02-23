using BackOffice.Identity.Grpc;
using Common.Tools.ProtoFilesGenerator;

Console.WriteLine("ProtoFilesGen: Args [" + string.Join(",", args) + "]");

// ===================== update only this part =====================

var grpcInterfaces = new Dictionary<string, Type>
{
    { "Identity.Grpc", typeof(IIdentityApi) }
};

// ====================== do not update below ======================

foreach (var interfaceName in args.Select(x => x[2..]))
{
    if (!grpcInterfaces.TryGetValue(interfaceName, out var type))
        throw new KeyNotFoundException("Project reference not found for proto file generation");

    await Generator.GenerateAsync(interfaceName, type);
}
