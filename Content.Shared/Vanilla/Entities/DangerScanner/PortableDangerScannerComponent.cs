using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Entities.DangerScanner;

[RegisterComponent]
public sealed partial class PortableDangerScannerComponent : Component
{
    [DataField]
    public float ScanDoAfterDuration = 5f;

    [DataField]
    public SoundSpecifier? CompleteSound = new SoundPathSpecifier("/Audio/Items/beep.ogg");
}

[Serializable, NetSerializable]
public sealed partial class ScannerDoAfterEvent : SimpleDoAfterEvent
{
}
