using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Server.Vanilla.EventTeam;

/// <summary>
/// Спавнит верфь ДСО
/// </summary>
[RegisterComponent]
public sealed partial class StationERTComponent : Component
{

    [DataField]
    public ResPath Map = new("/Maps/Vanilla/Maps/VerfDSO.yml");

    [DataField]
    public EntityUid? Entity;

    [DataField]
    public EntityUid? MapEntity;

    [DataField]
    public bool ERTCalled = false;
}
