using Content.Shared.Vanilla.Background;

namespace Content.Shared.Vanilla.Jammer;

[DataDefinition]
public sealed partial class OverrideJammerTimeEvent : BackgroundEvent
{
    [DataField("minutes")]
    public float Minutes = 5;

    public OverrideJammerTimeEvent() {}

    public OverrideJammerTimeEvent(float minutes)
    {
        Minutes = minutes;
    }
}
