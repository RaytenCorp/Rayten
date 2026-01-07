using Content.Shared.Vanilla.Skill;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Skill;

[RegisterComponent]
public sealed partial class GunCanBeFallComponent : Component
{
    [DataField("RequiresWeaponLevel")]
    public SkillLevel RequiresWeaponLevel { get; set; } = SkillLevel.Basic;

    [DataField("Recoil")]
    public float Recoil { get; set; } = 10f;

    [DataField("ChanceToFallPerLevel")]
    public float ChanceToFallPerLevel { get; set; } = 0.5f;
    public SoundSpecifier ThrowSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/bang.ogg");
}
