using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.VoiceSpeech;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class VoiceEmitterComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite),AutoNetworkedField]
    [DataField("voice", customTypeSerializer: typeof(PrototypeIdSerializer<VoiceSpeechPrototype>))]
    public string? VoicePrototypeId { get; set; }

    [DataField("pitch"), AutoNetworkedField]
    public float Pitch = 1.0f;

    public SoundSpecifier Voice = new SoundPathSpecifier("/Audio/Vanilla/Effects/Voices/SANS.ogg");

    public bool iswhisper = false;
}