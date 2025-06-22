using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
namespace Content.Shared.Vanilla.Dominator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DominatorComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? AuthorizedID = null;

    [DataField]
    public DominatorState CurrentState = DominatorState.Disabled;
}

[Serializable, NetSerializable]
public enum DominatorState : byte
{
    Disabled = 1,
    NonLethal = 2,
    Lethal = 3
}
