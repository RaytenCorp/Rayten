using Content.Shared.Damage.Systems;
namespace Content.Shared.Vanilla.Archon.BlindPredator;

public abstract class SharedBlindPredatorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BlindPredatorComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<BlindPredatorComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<BlindPredatorComponent, DamageChangedEvent>(OnDamageChanged);
        SubscribeLocalEvent<PredatorVisibleMarkComponent, ComponentStartup>(OnVictimStartup);
        SubscribeLocalEvent<PredatorVisibleMarkComponent, BeforeDamageChangedEvent>(OnBeforeDamageChanged);
    }

    private void OnDamageChanged(EntityUid uid, BlindPredatorComponent component, DamageChangedEvent args)
    {
        if (args.Origin == null)
            return;

        if (!TryComp<PredatorVisibleMarkComponent>(args.Origin.Value, out var mark))
            return;

        SetVisibility(args.Origin.Value, uid, true, mark);
    }

    private void OnBeforeDamageChanged(EntityUid uid, PredatorVisibleMarkComponent component, ref BeforeDamageChangedEvent args)
    {
        if (args.Origin == null)
            return;

        if (component.Predators.TryGetValue(args.Origin.Value, out var val) && !val)
            args.Cancelled = true;
    }

    private void OnComponentRemove(EntityUid uid, BlindPredatorComponent component, ComponentRemove args)
    {
        var query = EntityQueryEnumerator<PredatorVisibleMarkComponent>();
        while (query.MoveNext(out var ent, out var mark))
        {
            mark.Predators.Remove(uid);
            Dirty(ent, mark);
        }
    }

    private void OnVictimStartup(EntityUid uid, PredatorVisibleMarkComponent mark, ref ComponentStartup args)
    {
        var query = EntityQueryEnumerator<BlindPredatorComponent>();
        while (query.MoveNext(out var ent, out _))
            SetVisibility(uid, ent, false, mark);
    }


    private void OnComponentStartup(EntityUid uid, BlindPredatorComponent component, ref ComponentStartup args)
    {
        var query = EntityQueryEnumerator<PredatorVisibleMarkComponent>();
        while (query.MoveNext(out var ent, out var mark))
            SetVisibility(ent, uid, false, mark);
    }

    public virtual void SetVisibility(EntityUid victim, EntityUid predator, bool visible, PredatorVisibleMarkComponent? comp = null)
    {
        if (!Resolve(victim, ref comp))
            return;

        if (comp.Predators.TryGetValue(predator, out var val) && val == visible)
            return;

        comp.Predators[predator] = visible;
        Dirty(victim, comp);
    }
}
