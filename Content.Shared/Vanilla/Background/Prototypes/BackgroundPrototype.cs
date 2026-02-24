using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Content.Shared.Vanilla.Skill;

namespace Content.Shared.Vanilla.Background;

[Prototype("Background")]
public sealed partial class BackgroundPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string Name { get; set; } = "Неизвестная предыстория";

    [DataField]
    public string Description { get; set; } = "Описание отсутствует";

    [DataField]
    public List<string>? SpecialDesc { get; set; } = null;

    [DataField(customTypeSerializer: typeof(DictionarySerializer<SkillType, SkillLevel>))]
    public Dictionary<SkillType, SkillLevel> Skills { get; set; } = [];

    [DataField]
    public HashSet<SkillType> EasySkills { get; set; } = [];

    [DataField]
    public List<BackgroundSpecial> Specials { get; set; } = [];

    [DataField]
    public int SkillPoints { get; set; } = 0;

    [DataField]
    public bool SponsorOnly { get; set; } = false;
}

[ImplicitDataDefinitionForInheritors]
public abstract partial class BackgroundSpecial
{
    public abstract void Apply(EntityUid mob);
}
public abstract class BackgroundEvent;
