using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Skill;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class UnskilledWeaponComponent : Component
{
    [DataField("MinAnglePenalty"), AutoNetworkedField]
    public Angle MinAnglePenalty { get; set; } = Angle.Zero;

    [DataField("MaxAnglePenalty"), AutoNetworkedField]
    public Angle MaxAnglePenalty { get; set; } = Angle.Zero;

    [DataField("AngleIncreasePenalty"), AutoNetworkedField]
    public Angle AngleIncreasePenalty { get; set; } = Angle.Zero;
}