using Content.Shared.Vanilla.Background;
using Content.Shared.Popups;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Vanilla.Jammer;

[DataDefinition]
public sealed partial class OverrideJammerTimeEvent : BackgroundEvent
{
    [DataField("minutes")]
    public float Minutes = 5;

    public OverrideJammerTimeEvent() { }

    public OverrideJammerTimeEvent(float minutes)
    {
        Minutes = minutes;
    }
}
