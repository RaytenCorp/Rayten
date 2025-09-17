using Content.Shared.Actions;
using Content.Shared.DoAfter;

using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Entities.BrainWorm;
/// <summary>
/// это пипец че их так много
/// </summary>
///акшенсы
public sealed partial class InsertInBrainEvent : EntityTargetActionEvent
{
}
public sealed partial class EjectBrainEvent : InstantActionEvent
{
}
public sealed partial class MindControlEvent : InstantActionEvent
{
}
public sealed partial class BrainWormShopActionEvent : InstantActionEvent
{
}
public sealed partial class BrainWormChemicalsActionEvent : InstantActionEvent
{
}
public sealed partial class BrainWormForceSayActionEvent : InstantActionEvent
{
}
///дуафтеры
[Serializable, NetSerializable]
public sealed partial class InsertBrainDoAfterEvent : SimpleDoAfterEvent
{
}
[Serializable, NetSerializable]
public sealed partial class EjectBrainDoAfterEvent : SimpleDoAfterEvent
{
}
[Serializable, NetSerializable]
public sealed partial class MindControlDoAfterEvent : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed partial class ChemicalSelectMessage(string selected) : BoundUserInterfaceMessage
{
    public readonly string Selected = selected;
}

[Serializable, NetSerializable]
public sealed partial class ChemicalsupdateMessage(float chemicals, float chemicalscup) : BoundUserInterfaceMessage
{
    public readonly float Chemicals = chemicals;
    public readonly float ChemicalsCup = chemicalscup;
}

[Serializable, NetSerializable]
public sealed partial class ForceSayMessage(string text) : BoundUserInterfaceMessage
{
    public readonly string Text = text;
}


[Serializable, NetSerializable, DataDefinition]
public sealed partial class BrainWormUpgradeEvent : EntityEventArgs
{
    [DataField("upgrade", required: true)]
    public UpgradeType Upgrade { get; set; }

    public BrainWormUpgradeEvent(UpgradeType upgrade)
    {
        Upgrade = upgrade;
    }
}

[Serializable, NetSerializable]
public enum UpgradeType : byte
{
    inserbrain = 1, //проникающий отросток
    reproduce = 2, //Плодовое ядро
    chemupgrade = 3, //Секреторная железа
}
