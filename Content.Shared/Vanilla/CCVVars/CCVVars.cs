using Robust.Shared.Configuration;
using Robust.Shared;

namespace Content.Shared.Vanilla.CCVars;

[CVarDefs]
public sealed class CCVVars
{

    /// <summary>
    /// URL вебхука, используемого для отправки сообщений о банах на Discord сервер.
    /// </summary>
    public static readonly CVarDef<string> DiscordServerBansWebhook = CVarDef.Create("discord.server_bans_webhook", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
    public static readonly CVarDef<string> DiscordBridgeWebhook = CVarDef.Create("discord.bridge_webhook_url", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
    public static readonly CVarDef<string> DiscordAntiCheatWebhook = CVarDef.Create("discord.anticheat_webhook_url", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
    /// <summary>
    ///     Controls if the connections queue is enabled. If enabled stop kicking new players after `SoftMaxPlayers` cap and instead add them to queue.
    /// </summary>
    public static readonly CVarDef<bool> QueueEnabled = CVarDef.Create("queue.enabled", false, CVar.SERVERONLY);

    /// <summary>
    ///     Enabled Discord linking, show linking button and modal window
    /// </summary>
    public static readonly CVarDef<bool> DiscordAuthEnabled = CVarDef.Create("discord_auth.enabled", false, CVar.SERVERONLY);

    /// <summary>
    ///     URL of the Discord auth server API
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthApiUrl = CVarDef.Create("discord_auth.api_url", "", CVar.SERVERONLY);

    /// <summary>
    ///     Secret key of the Discord auth server API
    /// </summary>
    public static readonly CVarDef<string> DiscordAuthApiKey = CVarDef.Create("discord_auth.api_key", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<int> GameAlertLevelDownDelay = CVarDef.Create("game.alert_level_down_delay", 900, CVar.SERVERONLY);
    /// <summary>
    ///  Спонсорка
    /// </summary>
    public static readonly CVarDef<bool> SponsorEnabled = CVarDef.Create("Sponsor.enabled", false, CVar.SERVERONLY);
    public static readonly CVarDef<string> SponsorApiUrl = CVarDef.Create("Sponsor.api_url", "", CVar.SERVERONLY | CVar.CONFIDENTIAL);
    /// <summary>
    /// Включен ли тим дезматч в конце раунда
    /// </summary>
    public static readonly CVarDef<bool> TDMRoundEndEnabled = CVarDef.Create("game.tdmroundend_enabled", false, CVar.SERVERONLY);

}
