using Content.Shared.Vanilla.TimeStop;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Events;
using Content.Shared.Damage;

using Robust.Shared.GameObjects;
using Robust.Shared.Physics;

using System.Numerics;

namespace Content.Shared.Vanilla.TimeStop;

public sealed class SharedTimeStopSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TimeStoppedComponent, BeforeDamageChangedEvent>(OnBeforeDamageChanged);
        SubscribeLocalEvent<TimeStoppedComponent, BeforeStaminaDamageEvent>(OnBeforeStaminaDamage);
    }

    private void OnBeforeDamageChanged(EntityUid uid, TimeStoppedComponent comp, ref BeforeDamageChangedEvent args)
    {
        if (comp.Enabled == false)
            return;

        args.Cancelled = true;

        comp.StoredDamage += args.Damage;


    }

    private void OnBeforeStaminaDamage(EntityUid uid, TimeStoppedComponent comp, ref BeforeStaminaDamageEvent args)
    {
        if (comp.Enabled == false)
            return;

        args.Cancelled = true;

        comp.StoredStaminaDamage += args.Value;
    }
}
