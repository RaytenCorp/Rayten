using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.WithManyVoices;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class WithManyVoicesComponent : Component
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
    public float VisibleDistanceStand = 1f;
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет идти на шифте
    /// </summary>
    [DataField("visibleDistanceWalk"), AutoNetworkedField]
    public float VisibleDistanceWalk = 2.5f;
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет бежать
    /// </summary>
    [DataField("visibleDistanceRun"), AutoNetworkedField]
    public float VisibleDistanceRun = 12f;
}
