using Content.Shared.Access;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Access.AlertLevelAccess;

[RegisterComponent]
public sealed partial class AlertLevelAccessComponent : Component
{
    [DataField]
    public string ResetOnLevel = "green";
    [DataField]
    public HashSet<ProtoId<AccessLevelPrototype>> Blue = new();

    [DataField]
    public HashSet<ProtoId<AccessLevelPrototype>> Red = new()
    {
        "Cargo",
        "Salvage",
        "Command",
        "Engineering",
        "Atmospherics",
        "Medical",
        "Chemistry",
        "Paramedic",
        "Research",
        "Bar",
        "Kitchen",
        "Hydroponics",
        "Service",
        "Janitor",
        "Theatre",
        "Chapel",
        "Lawyer"
    };
    [DataField]
    public HashSet<ProtoId<AccessLevelPrototype>> Gamma = new()
    {
        "Cargo",
        "Salvage",
        "Command",
        "Engineering",
        "Atmospherics",
        "Medical",
        "Chemistry",
        "Paramedic",
        "Research",
        "Bar",
        "Kitchen",
        "Hydroponics",
        "Service",
        "Janitor",
        "Theatre",
        "Chapel",
        "Lawyer"
    };
    [DataField]
    public HashSet<ProtoId<AccessLevelPrototype>> Delta = new()
    {
        "Cargo",
        "Salvage",
        "Command",
        "Engineering",
        "Atmospherics",
        "Medical",
        "Chemistry",
        "Paramedic",
        "Research",
        "Bar",
        "Kitchen",
        "Hydroponics",
        "Service",
        "Janitor",
        "Theatre",
        "Chapel",
        "Lawyer"
    };
    [DataField]
    public HashSet<ProtoId<AccessLevelPrototype>> AddedAccess = new();
}
