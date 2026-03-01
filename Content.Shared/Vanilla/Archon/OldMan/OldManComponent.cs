using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Audio;
using Robust.Shared.Utility;
using Robust.Shared.Map;
using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.OldMan;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OldManComponent : Component
{
    [AutoNetworkedField]
    public bool Eats = false;
    #region звуки и анимации
    /// <summary>
    /// звук ухода и появления в карманное измерение
    /// </summary>
    [DataField]
    public SoundSpecifier TeleportSound = new SoundCollectionSpecifier("106teleport");
    [DataField]
    public SoundSpecifier MapInitSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/106mapinit.ogg");
    /// <summary>
    /// Координаты ухода в карманное измерение, на них старик телепортируется при невалидной точке выхода (космос, другой грид)
    /// </summary>
    public EntityCoordinates? FallBackCoords = null;
    /// <summary>
    /// Грид ухода в карманное измерение, телепортация разрешена только в пределах одного грида
    /// </summary>
    public EntityUid PreviousGrid = default;
    /// <summary>
    /// путь к карманному измерению
    /// </summary>
    [DataField]
    public ResPath DimensionMap = new ResPath("/Maps/Vanilla/Misc/PocketDimension.yml");

    [DataField("actionTeleport", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionId = "Action106Teleport";
    public EntityUid? ActionEnt;
    #endregion
    /// <summary>
    /// Грид карманного измерения, на него возвращается старик
    /// </summary>
    public EntityUid DimensionGridUid = default;
    /// <summary>
    /// карта карманного измерения
    /// </summary>
    public EntityUid DimensionUid = default;
    /// <summary>
    /// Грид станции
    /// </summary>
    public EntityUid StationGridUid = default;
}
[RegisterComponent]
public sealed partial class OldManPolymorphComponent : Component
{
    public EntityUid OldMan = default;
    public EntityUid StationGridUid = default;
}
public sealed partial class OldManTeleportEvent : InstantActionEvent { }

