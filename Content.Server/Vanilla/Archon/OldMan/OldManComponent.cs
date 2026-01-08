using Content.Shared.Vanilla.Archon.OldMan;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Audio;
using Robust.Shared.Utility;

namespace Content.Server.Vanilla.Archon.OldMan;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class OldManComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public bool IsActivePhase = false;
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan PhaseSwitchAt = TimeSpan.Zero;
    #region звуки и анимации
    /// <summary>
    /// звук ухода и появления в карманное измерение
    /// </summary>
    [DataField]
    public SoundSpecifier TeleportSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/106decay.ogg");
    /// <summary>
    /// Длительность входа в портал
    /// </summary>
    [DataField]
    public TimeSpan TeleportInDuration = TimeSpan.FromSeconds(2.45);
    /// <summary>
    /// Длительность выхода из портала
    /// </summary>
    [DataField]
    public TimeSpan TeleportOutDuration = TimeSpan.FromSeconds(2.6);
    /// <summary>
    /// путь к карманному измерению
    /// </summary>
    [DataField]
    public ResPath DimensionMap = new ResPath("/Maps/Vanilla/Misc/PocketDimension.yml");
    [DataField("actionTeleport", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionId = "Action106Teleport";
    [ViewVariables]
    public EntityUid? ActionEnt;
    #endregion
    /// <summary>
    /// Грид карманного измерения, на него возвращается старик
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid DimensionGridUid = default;
    /// <summary>
    /// карта карманного измерения
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid DimensionUid = default;
    /// <summary>
    /// Грид, с которого старик ушел в карманное измерение, только на этот грид старик может переместиться
    /// Если грид будет уничтожен, старик не сможет вернуться на станцию.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid StationGridUid = default;
    [ViewVariables(VVAccess.ReadOnly)]
    public bool InDimention = false;
    /// <summary>
    /// дедушка телепортируется?
    /// </summary>
    public TeleportState TPState = TeleportState.NoTP;
    /// <summary>
    /// момент времени когда анимация захода в портал закончится
    /// </summary>
    public TimeSpan TeleportationInEndAt = TimeSpan.Zero;
    /// <summary>
    /// момент времени когда анимация выхода из портала закончится
    /// </summary>
    public TimeSpan TeleportationOutEndAt = TimeSpan.Zero;
}
