using Robust.Shared.Prototypes;
using Content.Shared.Vanilla.Skill;

namespace Content.Shared.Vanilla.Background;

[Serializable, Prototype("BackgroundGroup")]
public sealed partial class BackgroundGroupPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("backgrounds")]
    public List<ProtoId<BackgroundPrototype>> Backgrounds { get; set; } = new();
}
