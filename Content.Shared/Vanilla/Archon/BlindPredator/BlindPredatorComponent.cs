using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Archon.BlindPredator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class BlindPredatorComponent : Component
{
    /// <summary>
    /// Делаем чек не каждый тик
    /// </summary>
    [ViewVariables]
    public TimeSpan NextCheckTime;

    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет стоять
    /// </summary>
    [DataField("visibleDistanceStand"), AutoNetworkedField]
    public float VisibleDistanceStand = 0.5f;
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет идти на шифте
    /// </summary>
    [DataField("visibleDistanceWalk"), AutoNetworkedField]
    public float VisibleDistanceWalk = 2.5f;
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет бежать
    /// </summary>
    [DataField("visibleDistanceRun"), AutoNetworkedField]
    public float VisibleDistanceRun = 6.5f;
    /// <summary>
    /// Длительность в течении которой мы будем всех видеть
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan EnableTime = TimeSpan.Zero;

}

public sealed partial class DisableBlindlessEvent : InstantActionEvent
{
    [DataField]
    public TimeSpan DisableDelay = TimeSpan.FromSeconds(2);
}
