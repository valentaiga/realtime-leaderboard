using BackOffice.PlayerSearch.Grpc;
using Google.Protobuf.Collections;

namespace FrontOffice.Web.Api.Player;

public static class AutocompleteGrpcExtensions
{
    public static IEnumerable<KeyValuePair<long, string>> FromGrpc(this RepeatedField<GrpcPlayer> players) =>
        players.Select(x => new KeyValuePair<long, string>(x.PlayerId, x.Username));
}