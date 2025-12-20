using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.TimeStop;

public sealed partial class TimeStopEvent : InstantActionEvent
{
    [DataField(required: true)]
    public EntProtoId Prototype;
}
