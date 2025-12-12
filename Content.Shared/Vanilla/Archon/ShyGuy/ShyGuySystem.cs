using Content.Shared.Mobs.Systems;
using Content.Shared.Examine;
using Content.Shared.Movement.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Audio;
using Content.Shared.Jittering;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Shared.Vanilla.Archon.ShyGuy;
/*
--------------------туду-лист--------------------
1. Вскрытие дверей только в рейдже
2. Выкачака очков только в спокойном состоянии
3. Приндутильный вход в спокойное состояние при стамкрите, крите, смерти

Статус: Готово? НЕТ
-------------------------------------------------
*/
public sealed class ShyGuySystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MobStateSystem _mobstate = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambient = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;

    public override void Initialize()
    {
        base.Initialize();
        UpdatesOutsidePrediction = true;
        SubscribeLocalEvent<ShyGuyComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMoveSpeed);
        SubscribeLocalEvent<ShyGuyComponent, OutlineHoverEvent>(OnLook);
        SubscribeAllEvent<ShyGuyGazeEvent>(OnGaze);
    }

    private void OnLook(EntityUid uid, ShyGuyComponent comp, OutlineHoverEvent args)
    {
        if (!IsReachable(uid, args.User, comp))
            return;

        RaisePredictiveEvent(new ShyGuyGazeEvent(GetNetEntity(uid), GetNetEntity(args.User)));
    }

    private void OnGaze(ShyGuyGazeEvent ev)
    {
        var shyGuy = GetEntity(ev.ShyGuy);
        var user = GetEntity(ev.User);

        if (!TryComp<ShyGuyComponent>(shyGuy, out var comp))
            return;

        if (!IsReachable(shyGuy, user, comp))
            return;

        _audio.PlayLocal(comp.StingerSound, user, user);
        _popup.PopupClient("Беги", user, PopupType.LargeCaution);

        comp.Targets.Add(user);
        Dirty(shyGuy, comp);
        SetPreparing(shyGuy, comp, user);
        var baseTime = comp.RageStartAt > _timing.CurTime ? comp.RageStartAt : _timing.CurTime;
        comp.TargetChaseEnd = baseTime + comp.OneTargetChaseTime;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (!_timing.IsFirstTimePredicted)
            return;

        var curTime = _timing.CurTime;

        var query = EntityQueryEnumerator<ShyGuyComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (curTime < comp.nextUpdate)
                continue;

            comp.nextUpdate = curTime + TimeSpan.FromSeconds(1);
            if (curTime >= comp.TargetChaseEnd)
                SetCalm(uid, comp);

            if (comp.State == ShyGuyState.Preparing && curTime >= comp.RageStartAt)
                SetRage(uid, comp);
        }
    }

    public void SetPreparing(EntityUid uid, ShyGuyComponent comp, EntityUid initiator)
    {
        if (comp.State != ShyGuyState.Calm)
            return;
        comp.RageStartAt = _timing.CurTime + comp.PreparingTime;
        comp.State = ShyGuyState.Preparing;

        _jitter.AddJitter(uid, 20, 20);
        _ambient.SetAmbience(uid, false);
        _audio.PlayPredicted(comp.PreparingSound, uid, initiator);
        Dirty(uid, comp);
    }

    public void SetCalm(EntityUid uid, ShyGuyComponent comp)
    {
        if (comp.State == ShyGuyState.Calm)
            return;
        EnsureComp<PacifiedComponent>(uid);
        comp.State = ShyGuyState.Calm;
        comp.RageStartAt = TimeSpan.Zero;
        comp.TargetChaseEnd = _timing.CurTime;

        _movementSpeed.RefreshMovementSpeedModifiers(uid);
        RemCompDeferred<JitteringComponent>(uid);
        comp.Targets.Clear();

        if (comp.CalmAmbient != null)
        {
            _ambient.SetSound(uid, comp.CalmAmbient);
            _ambient.SetAmbience(uid, true);
        }
        Dirty(uid, comp);
    }

    public void SetRage(EntityUid uid, ShyGuyComponent comp)
    {
        if (comp.State == ShyGuyState.Rage)
            return;
        _jitter.AddJitter(uid, 10, 10);

        comp.State = ShyGuyState.Rage;
        _movementSpeed.RefreshMovementSpeedModifiers(uid);
        RemComp<PacifiedComponent>(uid);
        if (comp.RageAmbient != null)
        {
            _ambient.SetSound(uid, comp.RageAmbient);
            _ambient.SetAmbience(uid, true);
        }
        Dirty(uid, comp);
    }

    public bool IsRaged(EntityUid uid, ShyGuyComponent? component = null)
    {
        return Resolve(uid, ref component, false) && component.State == ShyGuyState.Rage;
    }

    /// <summary>
    /// возвращает список всех посмотревших на скромника чувачков в радиусе
    /// </summary>
    public IEnumerable<EntityUid> GetNearbyObservers(Entity<ShyGuyComponent?> ent, float range)
    {
        if (!Resolve(ent, ref ent.Comp, false))
            return Array.Empty<EntityUid>();

        var nearby = _lookup
            .GetEntitiesInRange<MobStateComponent>(_xform.GetMapCoordinates(ent), range)
            .Select(e => e.Owner);

        return nearby.Where(victim => ent.Comp.Targets.Contains(victim));
    }

    protected bool IsReachable(EntityUid uid, EntityUid user, ShyGuyComponent comp)
    {
        if (user == uid)
            return false;

        if (comp.Targets.Contains(user))
            return false;

        if (!HasComp<MobStateComponent>(user))
            return false;

        if (TryComp<BlindableComponent>(user, out var blind) && blind.IsBlind)
            return false;

        if (!_mobstate.IsAlive(user) || !_mobstate.IsAlive(uid))
            return false;

        if (!_examine.InRangeUnOccluded(user, uid, 16f))
            return false;

        return true;
    }

    private void OnRefreshMoveSpeed(EntityUid uid, ShyGuyComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        if (component.State != ShyGuyState.Rage)
            return;

        args.ModifySpeed(component.WalkModifier, component.SprintModifier);
    }
}
