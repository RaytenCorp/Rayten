using Content.Shared.Vanilla.Damage.Events;
using Content.Shared.Vanilla.Archon.Research;
using Content.Shared.Prying.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Examine;
using Content.Shared.Movement.Systems;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Audio;
using Content.Shared.Jittering;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Popups;
using Content.Shared.Humanoid;
using Content.Shared.Vanilla.Archon.BlindPredator;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;
using System.Linq;

namespace Content.Shared.Vanilla.Archon.ShyGuy;

public sealed class ShyGuySystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MobStateSystem _mobstate = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;
    [Dependency] private readonly SharedAmbientSoundSystem _ambient = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ShyGuyComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<ShyGuyComponent, StaminaCritEvent>(OnStamCrit);
        SubscribeLocalEvent<ShyGuyComponent, MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<ShyGuyComponent, DamageChangedEvent>(OnDamageChanged);
        SubscribeLocalEvent<ShyGuyComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMoveSpeed);
        SubscribeLocalEvent<ShyGuyComponent, OutlineHoverEvent>(OnLook);
        SubscribeLocalEvent<ShyGuyComponent, ResearchAttemptEvent>(OnResearchAttempt);
        SubscribeAllEvent<ShyGuyGazeEvent>(OnGaze);
    }
    private void OnComponentShutdown(EntityUid uid, ShyGuyComponent component, ComponentShutdown args)
    {
        var query = EntityQueryEnumerator<PredatorVisibleMarkComponent>();
        while (query.MoveNext(out var ent, out var mark))
        {
            mark.Predators.Remove(uid);
            Dirty(ent, mark);
        }

    }

    private void OnDamageChanged(EntityUid uid, ShyGuyComponent component, DamageChangedEvent args)
    {
        if (args.Origin == null)
            return;

        if (args.DamageDelta == null || args.DamageDelta.GetTotal() <= 0)
            return;

        if (!IsReachable(uid, args.Origin.Value, component, onlyHumans: false))
            return;

        SetPreparing(uid, component, args.Origin.Value);
    }

    private void OnResearchAttempt(EntityUid uid, ShyGuyComponent comp, ResearchAttemptEvent args)
    {
        if (comp.State != ShyGuyState.Calm)
            args.Cancel();
    }

    private void OnMobStateChanged(EntityUid uid, ShyGuyComponent comp, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Alive)
            return;

        SetCalm(uid, comp);
    }

    private void OnStamCrit(EntityUid uid, ShyGuyComponent comp, StaminaCritEvent args)
    {
        SetCalm(uid, comp);
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

        SetPreparing(shyGuy, comp, user);
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

        var mark = EnsureComp<PredatorVisibleMarkComponent>(initiator);
        mark.Predators[uid] = true;

        comp.RageStartAt = _timing.CurTime + comp.PreparingTime;
        comp.TargetChaseEnd = comp.RageStartAt + comp.OneTargetChaseTime;
        comp.State = ShyGuyState.Preparing;

        _popup.PopupClient("Беги", uid, initiator, PopupType.LargeCaution);
        _audio.PlayLocal(comp.StingerSound, initiator, initiator);

        _jitter.AddJitter(uid, 20, 20);
        _ambient.SetAmbience(uid, false);
        _audio.PlayPredicted(comp.PreparingSound, uid, initiator);

        Dirty(uid, comp);
    }

    public void SetCalm(EntityUid uid, ShyGuyComponent comp)
    {
        if (comp.State == ShyGuyState.Calm)
            return;

        var pacified = EnsureComp<PacifiedComponent>(uid);
        pacified.DisallowAllCombat = true;

        comp.State = ShyGuyState.Calm;
        comp.RageStartAt = TimeSpan.Zero;
        comp.TargetChaseEnd = _timing.CurTime;

        _movementSpeed.RefreshMovementSpeedModifiers(uid);
        RemCompDeferred<JitteringComponent>(uid);
        var query = EntityQueryEnumerator<PredatorVisibleMarkComponent>();
        while (query.MoveNext(out var ent, out var mark))
        {
            mark.Predators[uid] = false;
            Dirty(ent, mark);
        }

        if (TryComp<PryingComponent>(uid, out var pry))
            pry.Enabled = false;

        if (comp.CalmAmbient != null)
        {
            _ambient.SetSound(uid, comp.CalmAmbient);
            _ambient.SetAmbience(uid, true);
        }
        _appearance.SetData(uid, ShyGuyVisuals.State, false);
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

        if (TryComp<PryingComponent>(uid, out var pry))
            pry.Enabled = true;

        if (comp.RageAmbient != null)
        {
            _ambient.SetSound(uid, comp.RageAmbient);
            _ambient.SetAmbience(uid, true);
        }
        _appearance.SetData(uid, ShyGuyVisuals.State, true);
        Dirty(uid, comp);
    }

    public bool IsRaged(EntityUid uid, ShyGuyComponent? component = null)
    {
        return Resolve(uid, ref component, false) && component.State == ShyGuyState.Rage;
    }

    protected bool IsReachable(EntityUid uid, EntityUid user, ShyGuyComponent comp, bool onlyHumans = true)
    {
        if (user == uid)
            return false;

        if (TryComp<PredatorVisibleMarkComponent>(user, out var mark) && mark.Predators.TryGetValue(uid, out var alreadyLooked) && alreadyLooked)
            return false;

        if (!HasComp<MobStateComponent>(user))
            return false;

        if (TryComp<BlindableComponent>(user, out var blind) && blind.IsBlind)
            return false;

        if (!_mobstate.IsAlive(user) || !_mobstate.IsAlive(uid))
            return false;

        if (!TryComp<StaminaComponent>(uid, out var stamina) || stamina.Critical)
            return false;

        if (onlyHumans && !HasComp<HumanoidAppearanceComponent>(user))
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
