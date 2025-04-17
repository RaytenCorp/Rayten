using System.Threading.Tasks;
using Content.Shared.Vanilla.CCVars;
using Robust.Shared.Configuration;

namespace Content.Server.Discord.Webhooks;

public sealed class WebhookBans : IPostInjectInit
{
    [Dependency] private readonly ILogManager _log = default!;

    private ISawmill _sawmill = default!;

    [Dependency] private readonly DiscordWebhook _discord = default!;

    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private WebhookIdentifier? _webhookIdentifier;


    public async Task SendBanMessage(string admin, string timeOfBan, string expires, string username, string reason, string banId)
    {
        try
        {
            if (_webhookIdentifier == null)
                return;

            var embed = new WebhookEmbed
            {
                Title = Loc.GetString("discord-serverBan-notifications-title", ("banId", banId)),
                Description = Loc.GetString("discord-ban-notifications-description",
                    ("user", username),
                    ("admin", admin),
                    ("timeOfBan", timeOfBan),
                    ("expires", expires),
                    ("reason", reason)
                ),
                Color = 15079705,
                Footer = new()
                {
                    Text = Loc.GetString("discord-ban-notifications-footer")
                }
            };

            var webhookPayload = new WebhookPayload()
            {
                Content = null,
                Embeds = new List<WebhookEmbed> {
                    embed
                    },
            };

            await _discord.CreateMessage(_webhookIdentifier.Value, webhookPayload);

        }
        catch (Exception e)
        {
            _sawmill.Error(e.Message);
        }
    }

    public async Task SendBanRoleMessage(string admin, string timeOfBan, string expires, string roleOrDepartment, string username, string reason)
    {
        try
        {
            if (_webhookIdentifier == null)
                return;

            var embed = new WebhookEmbed
            {
                Title = Loc.GetString("discord-roleBan-notifications-title"),
                Description = Loc.GetString("discord-roleBan-notifications-description",
                    ("user", username),
                    ("admin", admin),
                    ("timeOfBan", timeOfBan),
                    ("expires", expires),
                    ("reason", reason),
                    ("roles", roleOrDepartment)
                ),
                Color = 15079705,
                Footer = new()
                {
                    Text = Loc.GetString("discord-ban-notifications-footer")
                }
            };

            var webhookPayload = new WebhookPayload()
            {
                Content = null,
                Embeds = new List<WebhookEmbed> {
                    embed
                    }
            };

            await _discord.CreateMessage(_webhookIdentifier.Value, webhookPayload);
        }
        catch (Exception e)
        {
            _sawmill.Error(e.Message);
        }
    }

    public void Initialize()
    {
        _cfg.OnValueChanged(CCVVars.DiscordServerBansWebhook, url =>
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                _discord.GetWebhook(url, data => _webhookIdentifier = data.ToIdentifier());
            }
        }, true);
    }


    public void PostInject()
    {
        _sawmill = Logger.GetSawmill("WebhookBans");
    }

}