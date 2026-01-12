using Content.Shared.Corvax.Interface;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System.Threading;
using System.Threading.Tasks;

namespace Content.Server.Corvax.Interface;

public interface IServerDiscordAuthManager : ISharedDiscordAuthManager
{
    public event EventHandler<ICommonSession>? PlayerVerified;
    public Task<string> GenerateAuthLink(NetUserId userId, CancellationToken cancel);
    public Task<bool> IsVerified(NetUserId userId, CancellationToken cancel);
}
