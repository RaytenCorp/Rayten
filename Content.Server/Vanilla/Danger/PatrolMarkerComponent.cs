namespace Content.Server.Vanilla.Danger;

[RegisterComponent]
public sealed partial class PatrolMarkerComponent : Component
{
    [ViewVariables]
    public TimeSpan NewValidVisitAt = TimeSpan.Zero;

    public TimeSpan VisitTime = TimeSpan.FromSeconds(10);
}
