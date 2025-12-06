using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Content.Shared.DoAfter;
using Robust.Shared.Utility;
using Robust.Shared.Audio;

namespace Content.Shared.Archon.Components;

/// <summary>
/// Портативная штука, которая сканирует анхонты для дальнейшей привязки к оборудованиям
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ArchonScannerComponent : Component
{

    /// <summary>
    /// Привязанный Архонт
    /// </summary>
    [ViewVariables]
    public EntityUid? LinkedArchon;

    /// <summary>
    /// Звук при скане архона
    /// </summary>
    [DataField]
    public SoundSpecifier? ScanSound = new SoundPathSpecifier("/Audio/Vanilla/Items/archonScan.ogg");

    /// <summary>
    /// Звук при передаче архона в анализатор
    /// </summary>
    [DataField]
    public SoundSpecifier? LoadSound = new SoundPathSpecifier("/Audio/Vanilla/Items/archonLoad.ogg");

}
