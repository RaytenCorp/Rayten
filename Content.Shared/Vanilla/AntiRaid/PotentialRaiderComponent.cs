namespace Content.Shared.Vanilla.AntiRaid;

using Robust.Shared.Player;

[RegisterComponent]
public sealed partial class PotentialRaiderComponent : Component
{
    [ViewVariables]
    public ICommonSession? Session { get; set; } = null;

    [ViewVariables]
    public int Warns = 0;

    [ViewVariables]
    public Dictionary<EntityUid, float> Attackers { get; set; } = new();
}

