using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.DoAfter;
using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Dominator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DominatorComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? AuthorizedID = null;

    [DataField]
    public DominatorState CurrentState = DominatorState.Lethal;

    [DataField(required: true)]
    [AutoNetworkedField]
    public List<BatteryWeaponFireMode> FireModes = new();

    [DataField]
    public float ScanRange = 14.0f;

    [DataField]
    public float CheckDelay = 0.5f;

    public float Timer;
    public ProtoId<LocalizedDatasetPrototype> Dataset = "DominatorPhrases";
    public TimeSpan NextSpeechTime = TimeSpan.FromSeconds(0);
    public SoundSpecifier? CompleteSound = new SoundPathSpecifier("/Audio/Items/beep.ogg");
    [AutoNetworkedField]
    public bool AllowGhostTakeover = true;
}

[Serializable, NetSerializable]
public enum DominatorState : byte
{
    Disabled = 0,
    NonLethal = 1,
    Lethal = 2
}
[Serializable, NetSerializable]
public enum DominatorVisuals : byte
{
    firemod
}
[Serializable, NetSerializable]
public sealed partial class DominatorDoAfterEvent : SimpleDoAfterEvent
{
}
