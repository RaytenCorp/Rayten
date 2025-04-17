using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.VoiceSpeech;

[Prototype("VoiceSpeech")]
public sealed class VoiceSpeechPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name")]
    public string Name { get; } = string.Empty;

    [DataField("sex", required: true)]
    public Sex Sex { get; } = default!;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("voice", required: true)]
    public SoundSpecifier Voice = new SoundPathSpecifier("/Audio/Vanilla/Effects/Voices/SANS.ogg");

    [DataField("roundStart")]
    public bool RoundStart { get; } = true;

    [DataField("sponsorOnly")]
    public bool SponsorOnly { get; } = false;
}