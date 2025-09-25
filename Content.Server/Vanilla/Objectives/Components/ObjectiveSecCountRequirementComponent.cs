using Content.Server.Objectives.Systems;

namespace Content.Server.Vanilla.Objectives.Components;

[RegisterComponent]
public sealed partial class ObjectiveSecCountRequirementComponent : Component
{
    /// <summary>
    /// Минимальное количество сотрудников СБ
    /// </summary>
    [DataField]
    public int MinSec = 2;
}
