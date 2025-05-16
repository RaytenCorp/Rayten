using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Shared.Vanilla.Background;
using Content.Server.Mind;
using Content.Shared.Mind;
using Content.Server.Roles;
using Content.Server.Ghost.Roles;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Roles;
using Content.Shared.Vanilla.Jammer;
using Robust.Shared.GameObjects;
using JetBrains.Annotations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.Utility;
using Content.Shared.Implants;


using Robust.Shared.Utility;

namespace Content.Server.Vanilla.Background;

public sealed partial class ChangeMindSpecial : BackgroundSpecial
{
    [DataField(required: true)]
    public List<EntProtoId> MindRoles;

    public override void apply(EntityUid mob)
    {
    }
}

public sealed partial class AddItemSpecial : BackgroundSpecial
{
    [DataField(required: true)]
    public List<EntProtoId> Items;

    public override void apply(EntityUid mob)
    {
    }
}
public sealed partial class AddComponentsSpecial : BackgroundSpecial
{
    [DataField(required: true)]
    public ComponentRegistry Components { get; private set; }

    public override void apply(EntityUid mob)
    {
    }
}
public sealed partial class RaiseEventSpecial : BackgroundSpecial
{
    [DataField(required: true)]
    public List<BackgroundEvent> Events { get; private set; }
    public override void apply(EntityUid mob)
    {
    }
}
public sealed partial class AddImplantSpecial : BackgroundSpecial
{
    [DataField("implants", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<EntityPrototype>))]
    public HashSet<String> Implants { get; private set; } = new();
    public override void apply(EntityUid mob)
    {
    }
}