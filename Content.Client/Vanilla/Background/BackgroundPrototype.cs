using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Content.Shared.Vanilla.Background;

namespace Content.Client.Vanilla.Background;

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