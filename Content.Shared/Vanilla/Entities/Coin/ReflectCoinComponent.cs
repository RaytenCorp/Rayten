using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;

namespace Content.Shared.Vanilla.Coin;

[RegisterComponent]
public sealed partial class ReflectCoinComponent : Component
{

    [DataField]
    public FixedPoint2 DamageModifier = 1.5f;

    [DataField]
    public FixedPoint2 FlashingDamageModifier = 2.5f;

    [ViewVariables]
    public EntityUid? Shooter;

    [ViewVariables]
    public DamageSpecifier? StoredDamage;

    [ViewVariables]
    public bool Flashing = false;

    [DataField("flashEffect")]
    public string FlashEffectPrototype = "CoinFlash";
}
