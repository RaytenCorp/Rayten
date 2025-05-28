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

    [DataField("specialDesc")]
    public List<string>? SpecialDesc { get; set; } = null;

    [DataField(customTypeSerializer: typeof(DictionarySerializer<skillType, SkillLevel>))]
    public Dictionary<skillType, SkillLevel> Skills { get; set; } = new();

    [DataField("easySkills")]
    public HashSet<skillType> EasySkills { get; set; } = new();

    [DataField("specials")]
    public List<BackgroundSpecial> Specials { get; set; } = new();

    [DataField("skillPoints")]
    public int SkillPoints { get; set; } = 0;

    [DataField("sponsorOnly")]
    public bool SponsorOnly { get; set; } = false;
}

[ImplicitDataDefinitionForInheritors]
public abstract partial class BackgroundSpecial
{
    public abstract void apply(EntityUid mob);
}
public abstract class BackgroundEvent;
