using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Archon.WithManyVoices;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class WithManyVoicesComponent : Component
{
    /// <summary>
    /// Делаем чек не каждый тик
    /// </summary>
    [ViewVariables]
    public TimeSpan? SeeResetAt = null;
    /// <summary>
    /// Сколько времени видит многоголосый
    /// </summary>
    [ViewVariables]
    public TimeSpan SeeTime = TimeSpan.FromSeconds(15);
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет стоять
    /// </summary>
    [DataField("visibleDistanceStand"), AutoNetworkedField]
    public float VisibleDistanceStand = 2f;
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет идти на шифте
    /// </summary>
    [DataField("visibleDistanceWalk"), AutoNetworkedField]
    public float VisibleDistanceWalk = 4f;
    /// <summary>
    /// на таком расстоянии мы увидем чувака если он будет бежать
    /// </summary>
    [DataField("visibleDistanceRun"), AutoNetworkedField]
    public float VisibleDistanceRun = 16f;
    [DataField]
    public SoundSpecifier ExoSound { get; private set; } = new SoundCollectionSpecifier("Scream939");
}
public sealed partial class WithManyVoicesExoEvent : InstantActionEvent { }