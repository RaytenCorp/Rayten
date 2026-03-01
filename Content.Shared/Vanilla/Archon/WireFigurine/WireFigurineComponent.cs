using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
namespace Content.Shared.Vanilla.Archon.WireFigurine;

[Serializable, NetSerializable]
public sealed partial class EatMetal018DoAfterEvent : SimpleDoAfterEvent
{
}
public sealed partial class FigurineOrderActionEvent : InstantActionEvent
{
    [DataField]
    public FigurineOrderType Order = FigurineOrderType.Follow;
}

[Serializable, NetSerializable]
public enum FigurineOrderType : byte
{
    Follow,
    Eat,
}
