using Content.Shared.Damage;
using Robust.Shared.Audio;
namespace Content.Server.Vanilla.entities.AutoDefib;

[RegisterComponent]
public sealed partial class AutoDefibrillatorComponent : Component
{
    [DataField]
    public TimeSpan WritheDuration = TimeSpan.FromSeconds(3);

    [DataField]
    public int ZapDamage = 5;

    [DataField(required: true)]
    public DamageSpecifier ZapHeal = default!;
    [DataField]
    public SoundSpecifier ZapSound = new SoundPathSpecifier("/Audio/Items/Defib/defib_zap.ogg");
}