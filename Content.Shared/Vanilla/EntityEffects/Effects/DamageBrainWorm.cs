using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.Vanilla.Entities.BrainWorm;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.EntityEffects.Effects;

public sealed partial class ChemDamageBrainWorm : EntityEffect
{
    /// <summary>
    /// Сколько урона наносим червю.
    /// </summary>
    [DataField("amount")]
    public float Amount = 0.5f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-brainworm-damage", ("amount", Amount));

    public override void Effect(EntityEffectBaseArgs args)
    {
        var entMan = args.EntityManager;
        var sysMan = entMan.EntitySysManager;
        var damageable = sysMan.GetEntitySystem<DamageableSystem>();

        // проверяем — это червь?
        if (entMan.TryGetComponent<BrainWormHostComponent>(args.TargetEntity, out var hostcomp))
        {
            DamageSpecifier dmg = new()
            {
                DamageDict = new()
                {
                    { "Poison", Amount }
                }
            };
            damageable.TryChangeDamage(
                hostcomp.HostedBrainWorm,
                dmg);
        }
    }
}
