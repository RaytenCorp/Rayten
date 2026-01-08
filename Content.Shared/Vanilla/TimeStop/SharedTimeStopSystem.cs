
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Systems;
using Content.Shared.Projectiles;
using Content.Shared.Movement.Components;
using Content.Shared.Item;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;
namespace Content.Shared.Vanilla.TimeStop;

public sealed class SharedTimeStopSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;
    [Dependency] private readonly MetaDataSystem _meta = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TimeStopEvent>(OnTimeStopEvent);
        SubscribeLocalEvent<TimeStopFieldComponent, StartCollideEvent>(OnStartCollide);
        SubscribeLocalEvent<TimeStopFieldComponent, EndCollideEvent>(OnEndCollide);
        SubscribeLocalEvent<TimeStoppedComponent, BeforeDamageChangedEvent>(OnBeforeDamageChanged);
        SubscribeLocalEvent<TimeStoppedComponent, BeforeStaminaDamageEvent>(OnBeforeStaminaDamage);
    }

    private void OnTimeStopEvent(TimeStopEvent args)
    {
        if (args.Handled)
            return;

        if (!_timing.IsFirstTimePredicted)
            return;

        var uid = PredictedSpawnAtPosition(args.Prototype, Transform(args.Performer).Coordinates);

        if (TryComp<TimeStopFieldComponent>(uid, out var field))
        {
            field.TimeStopOwner = args.Performer;
            Dirty(uid, field);
        }

        args.Handled = true;
    }

    private void OnStartCollide(EntityUid uid, TimeStopFieldComponent comp, ref StartCollideEvent args)
    {
        var ent = args.OtherEntity;

        if (comp.TimeStopOwner == ent)
            return;

        if (HasComp<TimeStoppedComponent>(ent))
            return;

        if (HasComp<TimeStopFieldComponent>(ent))
            return;

        if (HasComp<TimeStopImmunityComponent>(ent))
            return;

        if (!HasComp<ItemComponent>(ent) && !HasComp<ProjectileComponent>(ent) && !HasComp<InputMoverComponent>(ent))
            return;

        var timestop = EnsureComp<TimeStoppedComponent>(ent);
        timestop.TimeStops++;
        _meta.SetEntityPaused(ent, true);
    }

    private void OnEndCollide(EntityUid uid, TimeStopFieldComponent component, ref EndCollideEvent args)
    {
        var ent = args.OtherEntity;

        if (!TryComp<TimeStoppedComponent>(ent, out var timestop))
        {
            _meta.SetEntityPaused(ent, false);
            return;
        }

        timestop.TimeStops--;
        if (timestop.TimeStops <= 0)
        {
            RemComp<TimeStoppedComponent>(ent);
            _meta.SetEntityPaused(ent, false);

            if (timestop.StoredDamage != null)
                _damageableSystem.ChangeDamage(ent, timestop.StoredDamage);

            if (timestop.StoredStaminaDamage != null)
                _stamina.TakeStaminaDamage(ent, timestop.StoredStaminaDamage);
        }
    }

    private void OnBeforeDamageChanged(EntityUid uid, TimeStoppedComponent comp, ref BeforeDamageChangedEvent args)
    {
        args.Cancelled = true;
        comp.StoredDamage += args.Damage;
    }

    private void OnBeforeStaminaDamage(EntityUid uid, TimeStoppedComponent comp, ref BeforeStaminaDamageEvent args)
    {
        args.Cancelled = true;
        comp.StoredStaminaDamage += args.Value;
    }
}
