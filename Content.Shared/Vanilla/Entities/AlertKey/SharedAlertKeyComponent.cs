using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.AlertKey;

[Virtual]
public partial class SharedAlertKeyComponent : Component
{
}

[Serializable, NetSerializable]
public sealed class AlertKeyInterfaceState : BoundUserInterfaceState
{
    public List<(string Level, bool IsSubcode, bool blocked)>? AlertLevels;
    public HashSet<string> ActiveSubLevels;
    public string CurrentAlert;
    public float CurrentAlertDelay;

    public AlertKeyInterfaceState(List<(string Level, bool IsSubcode, bool blocked)>? alertLevels, string currentAlert, HashSet<string> activeSubLevels, float currentAlertDelay)
    {
        AlertLevels = alertLevels;
        CurrentAlert = currentAlert;
        ActiveSubLevels = activeSubLevels;
        CurrentAlertDelay = currentAlertDelay;
    }
}

[Serializable, NetSerializable]
public sealed class AlertKeyApplyMessage : BoundUserInterfaceMessage
{
    public readonly string Level;
    public readonly string Reason;
    public AlertKeyApplyMessage(string level, string reason)
    {
        Level = level;
        Reason = reason;
    }
}

[Serializable, NetSerializable]
public enum AlertKeyUiKey
{
    Key
}
