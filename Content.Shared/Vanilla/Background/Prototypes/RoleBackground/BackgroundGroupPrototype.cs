using Robust.Shared.Prototypes;
using Content.Shared.Vanilla.Skill;

namespace Content.Shared.Vanilla.Background;

[Serializable, Prototype("BackgroundGroup")]
public sealed class BackgroundGroupPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("btype")]
    public BackgroundGroupType Type { get; set; }

    [DataField("backgrounds")]
    public List<ProtoId<BackgroundPrototype>> Backgrounds { get; set; } = new();
}
public enum BackgroundGroupType
{
    Baby,
    Adult,
    General
}
