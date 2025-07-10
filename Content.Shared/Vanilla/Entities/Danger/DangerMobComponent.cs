using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
namespace Content.Shared.Vanilla.Dominator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DangerMobComponent : Component
{
    [DataField]
    public int Danger = 0;

    [DataField]
    public int DeepDanger = 0;

    [DataField, AutoNetworkedField]
    public bool MaxDanger = true;
}
