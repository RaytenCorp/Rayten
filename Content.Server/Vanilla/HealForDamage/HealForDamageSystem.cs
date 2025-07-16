using Content.Shared.Vanilla.HealForDamage;
using Content.Shared.Mobs.Components;
using Content.Shared.Damage;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Server.Vanilla.HealForDamage.Systems;

public sealed class HealForDamageSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!; 

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DamageableComponent, DamageChangedEvent>(HealOnDamage);
    }

    private void HealOnDamage(EntityUid uid, DamageableComponent damageable, DamageChangedEvent args)
    {
        if (args.DamageDelta is not { } delta || args.Origin is not { } origin || origin == uid)
            return;

        var totalDamage = delta.GetTotal();
        if (totalDamage <= 0f)
            return;

        if (!TryComp<HealForDamageComponent>(origin, out var heal))
            return;

        if (!_mobStateSystem.IsAlive(uid))
            return;

        if (!TryComp<TransformComponent>(origin, out var healerXform) ||
            !TryComp<TransformComponent>(uid, out var targetXform))
            return;

        var healerPos = _transformSystem.GetWorldPosition(healerXform);
        var targetPos = _transformSystem.GetWorldPosition(targetXform);
        var distance = (targetPos - healerPos).Length();

        if (distance > heal.Radius)
            return;

        var distanceMod = 1f - distance / heal.Radius;
        var healAmount = totalDamage * heal.HealMultiplier * distanceMod;
        var damage = healAmount / totalDamage;

        var healDamage = new DamageSpecifier();
        foreach (var (type, amount) in delta.DamageDict)
        {
            healDamage.DamageDict[type] = -amount * damage;
        }

        _damageableSystem.TryChangeDamage(origin, healDamage);
    }
}
