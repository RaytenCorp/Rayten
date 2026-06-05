using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.EntityEffects;
using Content.Shared.Localizations;
using Content.Shared.Vanilla.Entities.BrainWorm;
using Robust.Shared.Prototypes;
using Robust.Shared.Maths;

namespace Content.Shared.Vanilla.EntityEffects.Effects;

/// <summary>
/// Наносит урон мозговому червю в теле носителя
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class DamageBrainWormEntityEffectSystem : EntityEffectSystem<MetaDataComponent, DamageBrainWorm>
{
    [Dependency] private DamageableSystem _dmg = default!;

    protected override void Effect(Entity<MetaDataComponent> entity, ref EntityEffectEvent<DamageBrainWorm> args)
    {
        if (TryComp<BrainWormHostComponent>(entity, out var hostcomp))
        {
            _dmg.TryChangeDamage(
                hostcomp.HostedBrainWorm,
                args.Effect.Damage);
        }
    }
}

public sealed partial class DamageBrainWorm : EntityEffectBase<DamageBrainWorm>
{
    [DataField(required: true)]
    public DamageSpecifier Damage = default!;

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        var damages = new List<string>();

        var damageSpec = new DamageSpecifier(Damage);
        var damageableSystem = entSys.GetEntitySystem<DamageableSystem>();
        var universalReagentDamageModifier = damageableSystem.UniversalReagentDamageModifier;
        var universalReagentHealModifier = damageableSystem.UniversalReagentHealModifier;

        damageSpec = damageableSystem.ApplyUniversalAllModifiers(damageSpec);

        foreach (var (type, amount) in damageSpec.DamageDict)
        {
            if (amount == FixedPoint2.Zero)
                continue;

            var localizedType = prototype.Index<DamageTypePrototype>(type).LocalizedName;
            var isHealing = amount < 0;
            var modifier = isHealing ? universalReagentHealModifier : universalReagentDamageModifier;

            damages.Add(Loc.GetString(
                "health-change-display",
                ("kind", localizedType),
                ("amount", MathF.Abs(amount.Float() * modifier)),
                ("deltasign", isHealing ? -1 : 1)
            ));
        }
        var damageList = ContentLocalizationManager.FormatList(damages);

        if (damages.Count == 0)
        {
            return Loc.GetString("entity-effect-guidebook-brainworm-damage-none");
        }

        return Loc.GetString("entity-effect-guidebook-brainworm-damage-detailed",
            ("changes", damageList));
    }

}
