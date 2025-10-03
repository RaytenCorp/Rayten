using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Trigger.Components.Effects;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared.Vanilla.Teleportation.Components;

/// <summary>
///     Создаёт портал который создаёт второй портал с координатами после триггера
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpawnCoordinatedPortalOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    ///     Прототип портала
    /// </summary>
    [DataField]
    public string PortalPrototype = "CoordinatedPortalEntry";

    [DataField]
    public MapCoordinates? Coordinates { get; set; }
}
