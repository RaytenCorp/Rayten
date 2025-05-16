using Content.Shared.Dataset;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Background;

[Prototype]
public sealed partial class RoleBackgroundPrototype : IPrototype
{

    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField]
    public ProtoId<BackgroundGroupPrototype>? Baby = null;

    [DataField]
    public ProtoId<BackgroundGroupPrototype>? Adult  = null;

    [DataField]
    public ProtoId<BackgroundGroupPrototype>? General  = null;
}