using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Actions.Components;

[RegisterComponent]
public sealed partial class SplodingComponent : Component
{
    [DataField]
    public SplodingState State = SplodingState.Standing;

    [DataField]
    public bool Gib = false;

    [DataField]
    public string ExplosionType = "FireKeepTiles";

    [DataField]
    public float ExplodeIntensity = 40f;

    [DataField]
    public float ExplodeMaxIntensity = 40f;

    [DataField]
    public float ExplodeDropoff = 3f;

    [DataField]
    public float Modifier = 1f;

    [DataField]
    public float Progress = 0f;

    [DataField]
    public SoundSpecifier? SplodeSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Actions/SplodeCharge.ogg");

    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(0.5);

    [DataField]
    public TimeSpan Timer = TimeSpan.FromSeconds(4);

    [DataField]
    public TimeSpan StartTime;

    public TimeSpan NextUpdate;
}

public enum SplodingState
{
    Standing,
    Charging,
    Exploding
}
