namespace Content.Server.Vanilla.Objectives.Components;

[RegisterComponent]
public sealed partial class OldManEatConditionComponent : Component
{
    [DataField]
    public bool Completed = false;
}
