using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.Research;

[RegisterComponent, NetworkedComponent]
public sealed partial class ArchonComponent : Component
{
    #region пассивные очки
    /// <summary>
    /// очки будут выдаваться 1 раз в какое-то время
    /// </summary>
    [DataField]
    public TimeSpan? ResearchCoolDown = null;
    public TimeSpan NextResearchAt;
    #endregion

    /// <summary>
    /// маяк к которому привязан
    /// </summary>
    [ViewVariables]
    public EntityUid? LinkedBeacon;

    /// <summary>
    /// Модификатор изучаемых очков
    /// </summary>
    [DataField]
    public float PointsModifier = 1.0f;
    /// <summary>
    /// Модификатор изучаемых очков
    /// </summary>
    [DataField]
    public float APointsModifier = 1.0f;

    /// <summary>
    /// продвинутые очки за изучение
    /// </summary>
    public int GetAPoints()
    {
        return (int)(1 * APointsModifier);
    }
    /// <summary>
    /// обычные очки за изучение
    /// </summary>
    public int GetPoints()
    {
        return (int)(25000 * PointsModifier);
    }

}

/// <summary>
/// Ивент проверки условия содержания, при cancel() отвязывает архонта от маяка/запрещает привязку
/// вызывается на архонте
/// </summary>
public sealed class ResearchAttemptEvent(Entity<ArchonBeaconComponent> beacon) : CancellableEntityEventArgs
{
    public Entity<ArchonBeaconComponent> Beacon { get; } = beacon;
}
/// <summary>
/// Ивент вызывается в момент разрыва связи с архонтом
/// вызывается на архонте
/// </summary>
public readonly struct ResearchLinkDisconnectionEvent
{
}