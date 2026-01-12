using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Vanilla.Research;

[RegisterComponent]
public sealed partial class ParaxitExtractorComponent : Component
{
    /// <summary>
    /// Сколько продвинутых очков стоит 1 параксит
    /// </summary>
    [DataField]
    public int PricePerExtract = 1;

    /// <summary>
    /// прототип того что будет добыто
    /// </summary>
    [DataField("extractPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>)), ViewVariables(VVAccess.ReadWrite)]
    public string ExtractPrototype = "MaterialParaxit1";

    /// <summary>
    /// The sound made when printing occurs
    /// </summary>
    [DataField]
    public SoundSpecifier ExtractSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg");
}