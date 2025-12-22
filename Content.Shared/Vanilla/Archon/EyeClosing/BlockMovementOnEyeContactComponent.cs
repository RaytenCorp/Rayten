using Content.Shared.Damage;
using Content.Shared.Actions;
using Robust.Shared.Map;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Archon.EyeClosing;

/// <summary>
///     Блокирует движение при зрительном контакте
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class BlockMovementOnEyeContactComponent : Component
{
    /// <summary>
    /// Таргет, который мы убьем в момент когда все закроют глаза
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public EntityUid? ScragTarget = null;

    [ViewVariables, AutoNetworkedField]
    public EntityCoordinates? TPTarget = null;

    /// <summary>
    /// момент времени, в который у всех будут закрыты глаза (для телепорта или убийства)
    /// </summary>
    [ViewVariables, AutoNetworkedField, AutoPausedField]
    public TimeSpan? BlinkMoment = null;

    /// <summary>
    /// Момент времени, в который мы можем действовать
    /// 3 секунды после появления печенья она ничего не может делать, иначе она убьет всех вокруг при появлении
    /// </summary>
    [ViewVariables, AutoNetworkedField, AutoPausedField]
    public TimeSpan GracePeriod = TimeSpan.Zero;

    [DataField(required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Damage = new();

    [DataField, AutoNetworkedField]
    public SoundSpecifier? Sound { get; set; } = new SoundCollectionSpecifier("Alarm173");

    [DataField, AutoNetworkedField]
    public SoundSpecifier ScragAlarm = new SoundPathSpecifier("/Audio/Vanilla/Effects/Actions/173_Alarm1.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? DamageSound { get; set; } = new SoundCollectionSpecifier("Snap173");
}
//акшен-ивент сворачивания шеи
public sealed partial class ScragEvent : EntityTargetActionEvent
{
}
//акшен-ивент телепортации
public sealed partial class SculptureTeleportEvent : WorldTargetActionEvent
{
}