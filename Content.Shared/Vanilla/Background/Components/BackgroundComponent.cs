using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Background;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BackgroundComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public ProtoId<BackgroundPrototype>? BabyBackground { get; set; } = null;
    
    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public ProtoId<BackgroundPrototype>? AdultBackground { get; set; } = null;

    [DataField, ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public ProtoId<BackgroundPrototype>? GeneralBackground { get; set; } = null;
}