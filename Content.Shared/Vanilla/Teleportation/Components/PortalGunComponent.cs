using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;
using Robust.Shared.Map;

namespace Content.Shared.Vanilla.Teleportation.Components;

/// <summary>
///     Давай морти приключение на 20 минут
/// </summary>
[RegisterComponent]
public sealed partial class PortalGunComponent : Component
{

    /// <summary>
    ///     Небходимый реагент
    /// </summary>
    [DataField]
    public ProtoId<ReagentPrototype> ReagentName = "PortalJuice";

    /// <summary>
    ///     Название ёмкости
    /// </summary>
    [DataField]
    public string SolutionName = "portal";

    /// <summary>
    ///     Координаты пишутся в окошке
    /// </summary>
    [DataField]
    public bool CanTypeCoordinates = false;

    [DataField]
    public TimeSpan LastClick = TimeSpan.Zero;

    [DataField]
    public float FireRate = 0.5f;

    /// <summary>
    ///     Сохранение позиции для дальнейшей телепортации туда
    /// </summary>
    [DataField]
    public bool CanSaveCoordinates = false;

    [DataField]
    public MapCoordinates? SavedCoordinates;

    /// <summary>
    /// Бля такие костыли
    /// </summary>
    [DataField]
    public EntProtoId? CoordinatedPortalProjectile = new();

    [DataField]
    public SoundSpecifier SaveCoordinatesSound =
    new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg")
    {
        Params = AudioParams.Default.WithVolume(-2f)
    };

    [DataField]
    public SoundSpecifier ShotSound =
    new SoundPathSpecifier("/Audio/Vanilla/Weapons/Guns/Gunshots/portalgun.ogg")
    {
    Params = AudioParams.Default.WithVolume(8f)
    };

    [DataField]
    public SoundSpecifier EmptyShotSound =
    new SoundPathSpecifier("/Audio/Weapons/Guns/Empty/empty.ogg");
}

[Serializable, NetSerializable]
public sealed partial class PortalGunDoAfterEvent : SimpleDoAfterEvent
{
}

public sealed partial class PortalGunShootEvent : EntityEventArgs
{
    public EntityUid User { get; set; }

    public PortalGunShootEvent(EntityUid user)
    {
        User = user;
    }
}
