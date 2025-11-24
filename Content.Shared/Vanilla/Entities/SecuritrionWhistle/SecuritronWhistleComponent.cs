
using Robust.Shared.GameStates;
using Content.Shared.Humanoid;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Entities.SecuritronWhistle;

[RegisterComponent, NetworkedComponent]
public sealed partial class SecuritronWhistleComponent : Component
{
    [DataField]
    public EntProtoId Effect = "WhistleExclamation";

    /// <summary>
    /// Range value.
    /// </summary>
    [DataField]
    public float Distance = 5;
}
