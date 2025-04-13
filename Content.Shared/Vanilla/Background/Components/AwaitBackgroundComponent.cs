using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Background;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AwaitBackgroundComponent : Component
{

    [DataField("backgroundGroup"), ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public ProtoId<BackgroundGroupPrototype>? BackgroundGroup;

    [DataField]
    public bool RoleAwaitBackground = false;
}