using Content.Shared.Interaction;
using Content.Shared.Examine;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Movement.Events;
using Content.Shared.ActionBlocker;
using Content.Shared.Physics;
using Content.Shared.Eye.Blinding.Components;

using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Audio.Systems;

namespace Content.Shared.Vanilla.Archon.EyeClosing;

public sealed partial class BlockMovementOnEyeContactSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ActionBlockerSystem _blocker = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ScragEvent>(OnScragEvent);
        SubscribeLocalEvent<SculptureTeleportEvent>(OnTeleportEvent);
        SubscribeLocalEvent<BlockMovementOnEyeContactComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<BlockMovementOnEyeContactComponent, UpdateCanMoveEvent>(OnUpdateCanMove);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var now = _timing.CurTime;

        var query = EntityQueryEnumerator<BlockMovementOnEyeContactComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (now < comp.GracePeriod)
                continue;

            if (now < comp.BlinkMoment)
                continue;

            if (!_mobStateSystem.IsAlive(uid)
                || !TryComp<StaminaComponent>(uid, out var stamina) || stamina.Critical)
            {
                comp.ScragTarget = null;
                comp.BlinkMoment = null;
                comp.TPTarget = null;
                continue;
            }

            if (comp.ScragTarget != null)
            {
                Scrag(uid, comp);
                continue;
            }

            if (comp.TPTarget != null)
            {
                Teleport(uid, comp);
                continue;
            }
        }
    }

    public void OnTeleportEvent(SculptureTeleportEvent ev)
    {
        if (ev.Handled)
            return;

        var uid = ev.Performer;
        var transform = Transform(ev.Performer);

        if (transform.MapID != _transform.GetMapId(ev.Target))
            return;

        if (!_interaction.InRangeUnobstructed(uid, ev.Target, range: 10f, popup: true))
            return;

        if (!TryComp<BlockMovementOnEyeContactComponent>(uid, out var comp))
            return;

        if (!TryComp<StaminaComponent>(uid, out var stamina) || stamina.Critical)
            return;

        var now = _timing.CurTime;

        if (!TryGetBlinkTiming(uid, out var TPTime, now))
        {
            _popup.PopupCursor(Loc.GetString("Нет подходящего момента для телепортации"));
            return;
        }

        ev.Handled = true;
        comp.BlinkMoment = TPTime;
        comp.TPTarget = ev.Target;
        comp.ScragTarget = null; //тп прерывает попытку убийства

        if (now == TPTime)//инстатп
            Teleport(uid, comp);

        Dirty(uid, comp);
    }

    public void OnScragEvent(ScragEvent ev)
    {
        if (ev.Handled || !Exists(ev.Target))
            return;

        var uid = ev.Performer;
        var target = ev.Target;
        var now = _timing.CurTime;

        if (!TryComp<BlockMovementOnEyeContactComponent>(uid, out var comp))
            return;

        if (now < comp.GracePeriod)
            return;

        if (HasComp<EyeClosingComponent>(target) && !HasComp<AutoEyeClosingComponent>(target))
            return;

        if (!TryComp<StaminaComponent>(uid, out var stamina) || stamina.Critical)
            return;

        if (!_interaction.InRangeUnobstructed(uid, target, 5f, popup: true))
            return;

        if (!_mobStateSystem.IsAlive(target))
        {
            _popup.PopupCursor(Loc.GetString("цель должна быть живой"));
            return;
        }

        if (!TryGetBlinkTiming(uid, out var killTime, now))
        {
            _popup.PopupCursor(Loc.GetString("Нет подходящего момента для убийства"));
            return;
        }

        if (TryComp<ActorComponent>(target, out var actor))
            _audio.PlayGlobal(comp.ScragAlarm, actor.PlayerSession);

        ev.Handled = true;
        comp.ScragTarget = target;
        comp.BlinkMoment = killTime;
        comp.TPTarget = null; //килл прерывает попытку телепортации

        if (now == killTime)//инстакилл
            Scrag(uid, comp);

        Dirty(uid, comp);
    }

    private void Teleport(EntityUid user, BlockMovementOnEyeContactComponent comp)
    {
        if (comp.TPTarget != null)
        {
            _transform.SetLocalPositionNoLerp(user, comp.TPTarget.Value.Position);
            _transform.AttachToGridOrMap(user, Transform(user));
        }
        comp.TPTarget = null;
        comp.BlinkMoment = null;
        Dirty(user, comp);
    }

    private void Scrag(EntityUid user, BlockMovementOnEyeContactComponent comp)
    {
        if (comp.ScragTarget != null)
        {
            var target = comp.ScragTarget.Value;

            if (!Exists(target)                                                 //цель перестала существовать
                || !_mobStateSystem.IsAlive(target)                             //цель умерла
                || !_interaction.InRangeUnobstructed(user, target, 6f)          //цель не достижима (за стеной итд)
                || !TryComp<TransformComponent>(target, out var targetXform)
                )
            {
                comp.ScragTarget = null;
                comp.BlinkMoment = null;
                return;
            }

            //телепортируемся
            _transform.SetParent(user, Transform(target).ParentUid);
            _transform.SetLocalPositionNoLerp(user, Transform(target).LocalPosition);
            //сворачиваем шею
            if (comp.DamageSound != null)
                _audio.PlayPredicted(comp.DamageSound, target, user);
            if (comp.Damage != null)
                _damageable.TryChangeDamage(target, comp.Damage, origin: user, ignoreResistances: true);
        }

        comp.ScragTarget = null;
        comp.BlinkMoment = null;
        Dirty(user, comp);
    }

    /// <summary>
    /// Функция проверяет, способна ли скульптура совершить действие в момент когда у всех будут закрыты глаза
    /// Возвращает момент времени когда способна действовать
    /// </summary>
    private bool TryGetBlinkTiming(EntityUid uid, out TimeSpan? killTime, TimeSpan now, float range = 14f)
    {
        killTime = null;
        bool any = false;

        TimeSpan globalStart = TimeSpan.Zero;
        TimeSpan globalEnd = TimeSpan.MaxValue;

        foreach (var ent in _lookup.GetEntitiesInRange<AutoEyeClosingComponent>(Transform(uid).Coordinates, range))
        {
            if (_mobStateSystem.IsIncapacitated(ent.Owner))
                continue;

            if (!_examine.InRangeUnOccluded(ent.Owner, uid, range))
                continue;

            any = true;
            GetNextBlink(ent.Comp, now, out var start, out var end);
            if (start > globalStart)
                globalStart = start;

            if (end < globalEnd)
                globalEnd = end;
        }
        //нет наблюдателей → действуем сразу
        if (!any)
        {
            killTime = now;
            return true;
        }

        if (globalStart >= globalEnd)
            return false;

        killTime = globalStart;
        return true;
    }

    /// <summary>
    /// Если у чела закрыты глаза в данный момент - получаем интервал этого моргания
    /// если у чела глаза открыты - получаем интервал будущего моргания
    /// </summary>
    private void GetNextBlink(AutoEyeClosingComponent comp, TimeSpan now, out TimeSpan nextStart, out TimeSpan nextEnd)
    {
        var start = comp.BlinkInTime;
        var end = comp.BlinkOutTime;
        if (start <= now && end > now)
        {
            nextStart = now;
            nextEnd = end;
            return;
        }
        var cycles = Math.Ceiling((now - start).TotalSeconds / comp.BlinkInterval.TotalSeconds);
        nextStart = start + TimeSpan.FromSeconds(cycles * comp.BlinkInterval.TotalSeconds);
        nextEnd = nextStart + comp.BlinkDuration;
    }

    private void OnMapInit(EntityUid uid, BlockMovementOnEyeContactComponent comp, ref MapInitEvent args)
    {
        comp.GracePeriod = _timing.CurTime + TimeSpan.FromSeconds(3);
        _blocker.UpdateCanMove(uid);
    }

    private void OnUpdateCanMove(EntityUid uid, BlockMovementOnEyeContactComponent comp, UpdateCanMoveEvent args)
    {
        args.Cancel();
    }

}
