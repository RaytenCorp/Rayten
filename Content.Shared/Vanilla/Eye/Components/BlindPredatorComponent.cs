using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

using Content.Shared.Actions;

namespace Content.Shared.Vanilla.Eye.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class BlindPredatorComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    [DataField("visibleDistanceStand"), AutoNetworkedField]
    public float VisibleDistanceStand = 1.5f;

    [DataField("visibleDistanceWalk"), AutoNetworkedField]
    public float VisibleDistanceWalk = 2.5f;

    [DataField("visibleDistanceRun"), AutoNetworkedField]
    public float VisibleDistanceRun = 6.5f;

    [DataField("userRunModifier"), AutoNetworkedField]
    public float UserRunModifier = 1f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan EnableTime = TimeSpan.Zero;

    [DataField, AutoNetworkedField]
    public bool DelayEnabled = false;
}

public sealed partial class DisableBlindlessEvent : InstantActionEvent
{

    [DataField]
    public TimeSpan DisableDelay = TimeSpan.FromSeconds(2);

    [DataField("sound"), AutoNetworkedField]
    public SoundSpecifier? Sound { get; set; } = new SoundCollectionSpecifier("Alarm939");

}
