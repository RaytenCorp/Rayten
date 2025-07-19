using Content.Shared.Actions;

namespace Content.Shared.Vanilla.Actions.Events;

public sealed partial class SplodeMarkingEvent : InstantActionEvent
{

    [DataField]
    public float Strength = 1f;

    [DataField("explosionType")]
    public string ExplosionType = "FireKeepTiles";

}
