using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Skill;
//Предмет с данным компонентом невидим для игроков, чей уровень навыка меньше заданного до того момента, пока предмет не подберут.
//только для навыка исследования

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class SkillInvisibleComponent : Component
{

    [ViewVariables, AutoNetworkedField]
    public bool Visible = false;
}
