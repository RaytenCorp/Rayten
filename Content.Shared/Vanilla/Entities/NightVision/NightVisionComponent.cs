namespace Content.Shared.Vanilla.Entities.NightVision;

[RegisterComponent]
public sealed partial class NightVisionComponent : Component
{
    [DataField("enabled")]
    public bool Enabled = true;
}
