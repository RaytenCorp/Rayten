using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Entities.BrainWorm;
/// <summary>
/// Компонент, необходимый для того чтобы заражённый мог вернуть контроль над телом
/// </summary>
[RegisterComponent]
public sealed partial class UnderControlComponent : Component
{
    public bool IsEscaping = false;
    public EntityUid OriginalMob;
    public float BaseResistTime = 45f;
}

[Serializable, NetSerializable]
public sealed partial class ReControlDoAfterEvent : SimpleDoAfterEvent
{
}

[ByRefEvent]
public record struct ReControlEvent();
