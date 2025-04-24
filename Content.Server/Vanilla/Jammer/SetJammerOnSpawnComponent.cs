using Robust.Shared.GameStates;

namespace Content.Server.Vanilla.Jammer;

[RegisterComponent]
public sealed partial class SetJammerOnSpawnComponent : Component
{
    [DataField("duration")]
    public TimeSpan Duration = TimeSpan.FromMinutes(20);
}

