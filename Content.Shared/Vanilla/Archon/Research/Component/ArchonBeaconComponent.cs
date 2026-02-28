using Robust.Shared.Serialization;
using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.Research;

/// <summary>
/// Стационарная штука, которая приносит очки, если архонт в радиусе действия
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ArchonBeaconComponent : Component
{
    /// <summary>
    /// привязанные архонтики
    /// </summary>
    [ViewVariables]
    public HashSet<EntityUid> LinkedArchons = [];

    /// <summary>
    /// Радиус содержания архонта
    /// </summary>
    [DataField]
    public float Radius = 3f;
}



[Serializable, NetSerializable]
public enum ArchonBeaconVisuals : byte
{
    Link
}
public enum ArchonBeaconVisualsLayers : byte
{
    Link
}