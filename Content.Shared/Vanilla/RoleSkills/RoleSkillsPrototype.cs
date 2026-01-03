using Content.Shared.Dataset;
using Content.Shared.Vanilla.Skill;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.RoleSkills;

[Prototype]
public sealed partial class RoleSkillsPrototype : IPrototype
{

    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField]
    public Dictionary<SkillType, SkillLevel> BasicSkills = [];

    [DataField]
    public HashSet<SkillType> EasySkills = [];

    [DataField]
    public int SkillPoints = 8;
}
