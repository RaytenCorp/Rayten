using Robust.Shared.Serialization;
using Robust.Shared.GameStates;
namespace Content.Shared.Vanilla.Entities.ArsenalAuthorizator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ArsenalAuthorizatorComponent : Component
{
    /// <summary>
    /// текущее состояние
    /// </summary>
    [DataField, AutoNetworkedField]
    public ArsenalAuthorizatorState State = ArsenalAuthorizatorState.Green;

    [DataField]
    public string NukeDiscAlertReason = "gamma1";

    [ViewVariables, AutoNetworkedField]
    public HashSet<string> AllowedFingerprints = [];
}


[Serializable, NetSerializable]
public sealed class ArsenalAuthorizatorOpenMessage : BoundUserInterfaceMessage
{
    public string ReasonId;

    public ArsenalAuthorizatorOpenMessage(string reasonId)
    {
        ReasonId = reasonId;
    }
}


[Serializable, NetSerializable]
public sealed class ArsenalAuthorizatorBoundInterfaceState : BoundUserInterfaceState
{
    public ArsenalAuthorizatorBoundInterfaceState()
    {
    }
}
[Serializable, NetSerializable]
public enum ArsenalAuthorizatorState : byte
{
    Green = 1,  //зелёный код, оружейка закрыта для всех
    Red = 2,    //красный код открыто для сб
    Gamma = 3,  //гамма код, открыто для всех
}

[Serializable, NetSerializable]
public enum ArsenalAuthorizatorVisuals : byte
{
    State,
}

[Serializable, NetSerializable]
public enum ArsenalAuthorizatorUiKey : byte
{
    Key,
}
