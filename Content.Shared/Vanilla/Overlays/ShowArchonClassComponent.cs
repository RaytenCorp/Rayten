using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Overlays;

[RegisterComponent, AutoGenerateComponentState(true), NetworkedComponent]
public sealed partial class ShowArchonClassComponent : Component
{

    [DataField]
    [AutoNetworkedField]
    public bool ShowRealClass = false;

}

