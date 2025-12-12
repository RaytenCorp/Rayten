using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.EyeClosing;

/// <summary>
///     Существо будет закрывать глаза, если в N радиусе есть объект с компонентом BlockMovementOnEyeContactComponent
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class AutoEyeClosingComponent : Component
{

    [ViewVariables, AutoNetworkedField, AutoPausedField]
    public TimeSpan BlinkInTime = TimeSpan.Zero;

    /// <summary>
    /// Тайминг открытия глаз
    /// </summary>
    [ViewVariables, AutoNetworkedField, AutoPausedField]
    public TimeSpan BlinkOutTime = TimeSpan.Zero;

    /// <summary>
    /// Промежут времени между морганием
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan BlinkInterval = TimeSpan.FromSeconds(3);
    /// <summary>
    /// длительность закрытых глазок
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan BlinkDuration = TimeSpan.FromSeconds(0.25f);
}
