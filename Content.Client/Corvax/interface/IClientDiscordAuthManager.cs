using Content.Shared.Corvax.Interface;

namespace Content.Client.Corvax.Interface;

public interface IClientDiscordAuthManager : ISharedDiscordAuthManager
{
    public string AuthUrl { get; }
}
