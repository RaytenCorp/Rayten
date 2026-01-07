using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Skill;
/// <summary>
/// Компонент медленно даёт опыт в определённый навык
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SkillAmnesiaComponent : Component
{
    [DataField, AutoNetworkedField]
    public SkillType Skilltype { get; set; } = 0;

    [DataField, AutoNetworkedField]
    public int Exptorestore { get; set; } = 600;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public TimeSpan TimeOfDeath = TimeSpan.Zero;

    [ViewVariables(VVAccess.ReadWrite), DataField]
    public TimeSpan NextUpdateTime;
}
