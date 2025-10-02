using Content.Server.Chat.Systems;
using Content.Shared.Vanilla.Eye.BlindPredator;


namespace Content.Server.Vanilla.Eye.BlindPredator;

public sealed class BlindPredatorSystem : SharedBlindPredatorSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;

    protected override void Say(EntityUid uid, string msg, string? name)
    {
        _chat.TrySendInGameICMessage(uid, msg, InGameICChatType.Speak, false,
            nameOverride: name, ignoreActionBlocker: true);
    }

    protected override void ChangeVictimVisablity(EntityUid target, bool visible)
    {
        if (visible)
        {
            EnsureComp<PredatorVisibleMarkComponent>(target);
        }
        else
        {
            RemComp<PredatorVisibleMarkComponent>(target);
        }
    }
}
