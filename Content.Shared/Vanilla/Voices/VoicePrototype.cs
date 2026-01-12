using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.VoiceSpeech;

[Prototype("VoiceSpeech")]
public sealed partial class VoiceSpeechPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("name")]
    public string Name { get; private set; } = string.Empty;

    [DataField("sex", required: true)]
    public Sex Sex { get; private set; } = default!;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("voice", required: true)]
    public SoundSpecifier Voice = new SoundPathSpecifier("/Audio/Vanilla/Effects/Voices/SANS.ogg");

    [DataField("roundStart")]
    public bool RoundStart { get; private set; } = true;

    [DataField("sponsorOnly")]
    public bool SponsorOnly { get; private set; } = false;
}