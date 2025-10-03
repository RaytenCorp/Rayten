using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Teleportation.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class PortalMapComponent : Component
{
    [DataField]
    public bool Enabled = true;

    [DataField]
    public float UpdateRate = 60f;

    [ViewVariables]
    public TimeSpan NextCheckTime;
}
