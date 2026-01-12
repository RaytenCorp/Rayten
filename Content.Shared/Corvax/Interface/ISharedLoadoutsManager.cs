using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Network;

namespace Content.Shared.Corvax.Interface;

public interface ISharedLoadoutsManager
{
    public void Initialize();
    public bool TryGetServerPrototypes(NetUserId userId, [NotNullWhen(true)] out List<string>? prototypes);
    public List<string> GetClientPrototypes();
}
