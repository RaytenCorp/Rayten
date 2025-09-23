using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Actions;

namespace Content.Shared.Vanilla.Skill;

[RegisterComponent, NetworkedComponent]
public sealed partial class AssComponent : Component
{
    [DataField]
    public EntProtoId AssImplant = "AssImplant";

    [DataField]
    public float dropchance = 0.15f;

    public EntityUid? Action;
    public EntityUid? ImplantUid;

}
public sealed partial class OpenAssStorageEvent : InstantActionEvent
{

}