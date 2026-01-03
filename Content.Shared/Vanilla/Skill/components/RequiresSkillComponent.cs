
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Skill;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RequiresSkillComponent : Component
{
    /// <summary>
    /// основные навыки, которыми должен обладать пользователь
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<SkillType, SkillLevel> BasicSkills = [];
    /// <summary>
    /// лёгкие навыки, которыми должен обладать пользователь
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<SkillType> EasySkills = [];
    /// <summary>
    // навык учитывается только в процессе крафта
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool OnlyForCraft { get; set; } = false;
}
