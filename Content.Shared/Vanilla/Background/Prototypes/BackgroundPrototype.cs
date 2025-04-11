using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Content.Shared.Vanilla.Skill;

namespace Content.Shared.Vanilla.Background;

[Serializable, Prototype("Background")]
public sealed class BackgroundPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name")]
    public string Name { get; set; } = "Неизвестная предыстория";

    [DataField("description")]
    public string Description { get; set; } = "Описание отсутствует";

    [DataField(customTypeSerializer: typeof(DictionarySerializer<skillType, SkillLevel>))]
    public Dictionary<skillType, SkillLevel> Skills { get; set; } = new();

    [DataField("easySkills")]
    public HashSet<skillType> EasySkills { get; set; } = new();

    [DataField("specials")]
    public HashSet<ProtoId<BackgroundSpecialPrototype>> Specials { get; set; } = new();

    [DataField("skillPoints")]
    public int SkillPoints { get; set; } = 0;
}