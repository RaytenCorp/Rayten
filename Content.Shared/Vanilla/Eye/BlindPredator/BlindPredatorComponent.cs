using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

using Content.Shared.Actions;

namespace Content.Shared.Vanilla.Eye.BlindPredator;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class BlindPredatorComponent : Component
{
    /// <summary>
    /// Сущности, которых видит яшпшерица
    /// </summary>
    [DataField]
    public HashSet<EntityUid> VisibleEnts = new();
    /// <summary>
    /// Делаем чек не каждый тик
    /// </summary>
    [ViewVariables]
    public TimeSpan NextCheckTime;

    [DataField("visibleDistanceStand"), AutoNetworkedField]
    public float VisibleDistanceStand = 1.5f;

    [DataField("visibleDistanceWalk"), AutoNetworkedField]
    public float VisibleDistanceWalk = 2.5f;

    [DataField("visibleDistanceRun"), AutoNetworkedField]
    public float VisibleDistanceRun = 6.5f;

    [DataField("userRunModifier"), AutoNetworkedField]
    public float UserRunModifier = 1f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan EnableTime = TimeSpan.Zero;
    /// <summary>
    /// Возвращает максимальный ренж сущностей, до которых мы можем дотянуться нашими глазиками
    /// </summary>
    public float GetMaxRange()
    {
        return MathF.Max(VisibleDistanceRun, MathF.Max(VisibleDistanceWalk, VisibleDistanceStand));
    }
}

public sealed partial class DisableBlindlessEvent : InstantActionEvent
{

    [DataField]
    public TimeSpan DisableDelay = TimeSpan.FromSeconds(2);

    [DataField("sound"), AutoNetworkedField]
    public SoundSpecifier? Sound { get; set; } = new SoundCollectionSpecifier("Alarm939");

}
