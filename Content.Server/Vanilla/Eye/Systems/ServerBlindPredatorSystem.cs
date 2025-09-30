using Content.Shared.Animals.Components;
using Content.Shared.Vanilla.Eye.Components;
using Content.Shared.Popups;
using Robust.Shared.Timing;
using Robust.Shared.Random;
using Content.Server.Chat.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Server.Player;

namespace Content.Server.Eye.Systems;

public sealed class ServerBlindPredatorSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DisableBlindlessEvent>(OnDisableAction);
    }

    private void OnDisableAction(DisableBlindlessEvent args)
    {
        if (args.Handled)
            return;

        var uid = args.Performer;

        if (!TryComp<BlindPredatorComponent>(uid, out var blindComp))
            return;

        if (TryComp<ParrotMemoryComponent>(uid, out var parrotComp) &&
            parrotComp.SpeechMemories.Count > 0)
        {
            var memory = _random.Pick(parrotComp.SpeechMemories);

            _chat.TrySendInGameICMessage(uid, memory.Message, InGameICChatType.Speak, false,
                nameOverride: memory.Name, ignoreActionBlocker: true);

            blindComp.Enabled = false;
            blindComp.DelayEnabled = true;
            blindComp.EnableTime = _gameTiming.CurTime + args.DisableDelay;

            _audio.PlayPvs(args.Sound, uid);

            Dirty(uid, blindComp);

            args.Handled = true;
        }
        else
           _popup.PopupClient("Вы ещё не скопировали ничей голос", uid, uid, PopupType.SmallCaution);
    }
}
