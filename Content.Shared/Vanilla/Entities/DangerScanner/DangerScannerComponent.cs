using Content.Shared.DoAfter;
using Content.Shared.Radio;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Entities.DangerScanner;

[RegisterComponent, NetworkedComponent]
public sealed partial class DangerScannerComponent : Component
{
    [DataField]
    public float ScanDoAfterDuration = 2f;

    [DataField]
    public ProtoId<RadioChannelPrototype> SecurityChannel = "Security";
    [DataField]
    public SoundSpecifier? CompleteSound = new SoundPathSpecifier("/Audio/Vanilla/Items/mtgSuccess.ogg");
    [DataField]
    public SoundSpecifier? ScanSound = new SoundPathSpecifier("/Audio/Vanilla/Items/archonScan.ogg");
    [DataField]
    public SoundSpecifier? AlarmSound = new SoundPathSpecifier("/Audio/Vanilla/Items/mtgDeny.ogg");
}

[Serializable, NetSerializable]
public sealed partial class ScannerDoAfterEvent : SimpleDoAfterEvent
{
}
[Serializable, NetSerializable]
public enum DangerScannerVisuals : byte
{
    Mode
}
[Serializable, NetSerializable]
public enum DangerScannerLayers : byte
{
    Mode
}

