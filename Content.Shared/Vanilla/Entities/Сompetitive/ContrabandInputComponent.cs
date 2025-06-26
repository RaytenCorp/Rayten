using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Competitive;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ContrabandInputComponent : Component
{
    [DataField("slotId", required: true)]
    public string SlotId = string.Empty;

    [DataField]
    public float ScanTime = 3f;

    [AutoNetworkedField]
    public bool Analysing = false;
}

[Serializable, NetSerializable]
public enum ContrabandAnalyzerVisuals : byte
{
    Accept,
    Cancel
}
