using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.Research;

[RegisterComponent, NetworkedComponent]
public sealed partial class ArchonComponent : Component
{
    /// <summary>
    /// маяк к которому привязан
    /// </summary>
    [ViewVariables]
    public EntityUid? LinkedBeacon;
}
/// <summary>
/// Ивент проверки условия содержания, при cancel() отвязывает архонта от маяка/запрещает привязку
/// вызывается на архонте
/// </summary>
public sealed class ResearchAttemptEvent(Entity<ArchonBeaconComponent> beacon) : CancellableEntityEventArgs
{
    public Entity<ArchonBeaconComponent> Beacon { get; } = beacon;
}