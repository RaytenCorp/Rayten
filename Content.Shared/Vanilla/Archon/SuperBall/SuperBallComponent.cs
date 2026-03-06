using Robust.Shared.Audio;
namespace Content.Shared.Vanilla.Archon.SuperBall;

[RegisterComponent]
public sealed partial class SuperBallComponent : Component
{
    [DataField] public float DamageMinSpeed = 20f;
    [DataField] public float MaxSpeed = 35f;

    [DataField]
    public SoundSpecifier LowSpeedSounds { get; private set; } = new SoundCollectionSpecifier("SuperBallLow");
    [DataField]
    public SoundSpecifier MediumSpeedSounds { get; private set; } = new SoundCollectionSpecifier("SuperBallMedium");
    [DataField]
    public SoundSpecifier HighSpeedSounds { get; private set; } = new SoundCollectionSpecifier("SuperBallHigh");
}
