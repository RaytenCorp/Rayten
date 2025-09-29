using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;
using Robust.Shared.Map;

namespace Content.Shared.Teleportation.Components;

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
    ///     Скорость проджектайла
    /// </summary>
    [DataField]
    public float ProjectileVelocity = 25f;

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
    public EntProtoId CoordinatedPortalProjectile = new();

    [DataField]
    public SoundSpecifier SaveCoordinatesSound =
    new SoundPathSpecifier("/Audio/Machines/high_tech_confirm.ogg")
    {
        Params = AudioParams.Default.WithVolume(-2f)
    };
}

[Serializable, NetSerializable]
public sealed partial class PortalGunDoAfterEvent : SimpleDoAfterEvent
{
}
