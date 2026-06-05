using Content.Server.DeviceLinking.Components;
using Content.Shared.Interaction;
using Robust.Shared.Audio.Systems;
using Content.Shared.Timing;
using Content.Server.Radio.EntitySystems;



namespace Content.Server.DeviceLinking.Systems;

public sealed partial class SignalAlertSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private RadioSystem _radio = default!;

    [Dependency] private UseDelaySystem _delay = default!;

    [Dependency] private ILocalizationManager _loc = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SignalAlertComponent, ActivateInWorldEvent>(OnActivated);
    }


    private void OnActivated(EntityUid uid, SignalAlertComponent comp, ActivateInWorldEvent args)
    {

        if (!TryComp(uid, out UseDelayComponent? useDelay) || _delay.IsDelayed((uid, useDelay)))
            return;

        string nameRoom = _loc.GetString(comp.NameRoom);

        _radio.SendRadioMessage(uid, _loc.GetString("ent-AlertButtonMessage", ("nameRoom", nameRoom)), comp.RadioChannel, uid);

        _audio.PlayPvs(comp.ClickSound, uid);

        _delay.TryResetDelay((uid, useDelay));
    }
}
