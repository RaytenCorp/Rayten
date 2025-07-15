using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Content.Shared.Weapons.Ranged.Components;
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
    public TimeSpan NextSpeechTime = TimeSpan.FromSeconds(0);

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
