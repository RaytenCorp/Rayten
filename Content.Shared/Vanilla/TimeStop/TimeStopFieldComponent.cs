using Robust.Shared.GameStates;
namespace Content.Shared.Vanilla.TimeStop;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class TimeStopFieldComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public EntityUid? TimeStopOwner;
}
