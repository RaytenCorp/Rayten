using Content.Shared.Dataset;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;
namespace Content.Shared.Vanilla.Archon.MastersFeather;

[RegisterComponent]
public sealed partial class MastersFeatherComponent : Component
{
    [DataField]
    public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(30);
    public HashSet<EntityUid> UsedBy = [];

    [DataField(required: true)]
    public List<ProtoId<LocalizedDatasetPrototype>> BiographyDatasets = [];

    [DataField]
    public SoundSpecifier WritingSound = new SoundPathSpecifier("/Audio/Vanilla/Items/Archon/067/Writing.ogg");
    [DataField]
    public SoundSpecifier DoneSound = new SoundPathSpecifier("/Audio/Vanilla/Items/Archon/067/Done.ogg");
    [DataField]
    public EntityUid? AudioStream;
}