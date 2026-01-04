using Content.Server.Ghost.Roles;
using Content.Server.Ghost.Roles.Components;
using Content.Server.Ghost;
using Content.Shared.Vanilla.Archon.Research;
using Content.Shared.Vanilla.Archon.OldMan;
using Content.Shared.Vanilla.Damage.Events;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Damage;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Overlays;
using Content.Shared.Administration;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Humanoid;
using Content.Shared.FixedPoint;
using Content.Shared.Maps;
using Content.Shared.Mind;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Jittering;
using Content.Shared.Damage.Events;
using Content.Shared.Actions;
using Content.Shared.Bed.Sleep;
using Robust.Shared.Physics.Events;
using Robust.Shared.Map.Components;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Random;
using Robust.Shared.Audio.Systems;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow;
using System.Linq;

namespace Content.Server.Vanilla.Archon.OldMan;

public sealed class OldManSystem : EntitySystem
{
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly GhostSystem _ghost = default!;
    [Dependency] private readonly GhostRoleSystem _ghostrole = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;


    private const float UpdateRate = 0.1f;
    private float _updateDif;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<OldManComponent, ResearchAttemptEvent>(OnResearchAttempt);
        SubscribeLocalEvent<OldManComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<OldManComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<OldManComponent, OldManTeleportEvent>(OnTeleportEvent);
        SubscribeLocalEvent<OldManComponent, MeleeHitEvent>(OnMeleeHit);
        SubscribeLocalEvent<OldManComponent, StaminaCritEvent>(OnStamCrit);
        SubscribeLocalEvent<OldManComponent, MobStateChangedEvent>(OnMobStateChanged);
        SubscribeLocalEvent<OldManComponent, BeforeDamageChangedEvent>(OnBeforeDamageChanged);
        SubscribeLocalEvent<OldManComponent, BeforeStaminaDamageEvent>(OnBeforeStaminaDamage);

        SubscribeLocalEvent<DimensionVictimComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMoveSpeed);
        SubscribeLocalEvent<DimensionVictimComponent, MapInitEvent>(OnVictimInit);
        SubscribeLocalEvent<DimensionVictimComponent, MobStateChangedEvent>(OnVictimStateChanged);

        SubscribeLocalEvent<DimensionEscapeTeleportComponent, StartCollideEvent>(OnCollide);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _updateDif += frameTime;
        if (_updateDif < UpdateRate)
            return;

        _updateDif -= UpdateRate;
        var now = _timing.CurTime;
        var query = EntityQueryEnumerator<OldManComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var trans))
        {
            ProcessTeleport(uid, comp, trans, now);
            if (now > comp.PhaseSwitchAt)
                SwitchPhase(uid, comp);
        }

        var victimQuery = EntityQueryEnumerator<DimensionVictimComponent>();
        while (victimQuery.MoveNext(out var uid, out var comp))
            DamageVictim(uid, comp, now);
    }
    #region старик
    private void OnResearchAttempt(EntityUid uid, OldManComponent comp, ResearchAttemptEvent args)
    {
        if (comp.IsActivePhase)
            args.Cancel();
    }
    private void OnBeforeDamageChanged(EntityUid uid, OldManComponent component, ref BeforeDamageChangedEvent args)
    {
        if (args.Origin != null && HasComp<DimensionVictimComponent>(args.Origin.Value))
            args.Cancelled = true;
    }
    private void OnBeforeStaminaDamage(EntityUid uid, OldManComponent component, ref BeforeStaminaDamageEvent args)
    {
        args.Cancelled = true;
    }
    private void OnStamCrit(EntityUid uid, OldManComponent comp, StaminaCritEvent args)
    {
        if (comp.IsActivePhase)
            SwitchPhase(uid, comp);

        ReturnAllVictims((uid, comp));
    }
    private void OnMobStateChanged(EntityUid uid, OldManComponent comp, MobStateChangedEvent args)
    {
        if (args.OldMobState > args.NewMobState)
            return;

        ReturnAllVictims((uid, comp));

        if (args.NewMobState == MobState.Critical)
        {
            if (comp.InDimention)
                return;
            var action = _actions.GetAction(comp.ActionEnt);
            if (action != null && _actions.ValidAction(action.Value))
                _actions.PerformAction(uid, action.Value, predicted: false);
        }

        if (comp.IsActivePhase)
            SwitchPhase(uid, comp);

        //отмена тп при смерти
        if (args.NewMobState == MobState.Dead)
        {
            comp.TPState = TeleportState.NoTP;
            _appearance.SetData(uid, DamageVisualizerKeys.DamageUpdateGroups, comp.TPState);
            RemComp<AdminFrozenComponent>(uid);
        }
    }
    private void OnComponentShutdown(EntityUid uid, OldManComponent comp, ref ComponentShutdown args)
    {
        ReturnAllVictims((uid, comp));
        if (!Deleted(comp.DimensionUid))
            QueueDel(comp.DimensionUid);
    }

    /// <summary>
    /// При смене с активной фазы на пассивную дед выкидывается в гост, следующая активная фаза через 25-45 минут
    /// При смене с пассивной фазы на активную можно поиграть за деда, активная фаза длится три минуты после взятия роли
    /// <summary>
    private void SwitchPhase(EntityUid uid, OldManComponent comp)
    {
        if (!TryComp<GhostRoleComponent>(uid, out var ghostRole))
            return;

        if (comp.IsActivePhase)
        {
            if (_mind.TryGetMind(uid, out var mindId, out var mind))
                _ghost.OnGhostAttempt(mindId, false, true, true, mind);
            _ghostrole.UnregisterGhostRole((uid, ghostRole));
            var nextTime = _random.NextFloat(15f, 35f);
            comp.PhaseSwitchAt = _timing.CurTime + TimeSpan.FromMinutes(nextTime);
            var sleep = EnsureComp<SleepingComponent>(uid);
            sleep.CooldownEnd = _timing.CurTime + TimeSpan.FromMinutes(nextTime);
        }
        else
        {
            comp.PhaseSwitchAt = _timing.CurTime + TimeSpan.FromMinutes(5);
            _ghostrole.RegisterGhostRole((uid, ghostRole));
        }

        comp.IsActivePhase = !comp.IsActivePhase;
    }

    private void TeleportOldMan(EntityUid uid, OldManComponent comp)
    {
        if (Transform(uid).GridUid == null)
            return;

        EnsureComp<AdminFrozenComponent>(uid);
        _appearance.SetData(uid, DamageVisualizerKeys.DamageUpdateGroups, TeleportState.In);
        _audio.PlayPvs(comp.TeleportSound, uid);
        comp.TPState = TeleportState.In;
        comp.TeleportationInEndAt = _timing.CurTime + comp.TeleportInDuration;
        comp.TeleportationOutEndAt = comp.TeleportationInEndAt + comp.TeleportOutDuration;
    }

    private void ProcessTeleport(EntityUid uid, OldManComponent comp, TransformComponent trans, TimeSpan now)
    {
        //вошли в телепорт
        if (comp.TPState == TeleportState.In && now >= comp.TeleportationInEndAt)
        {
            comp.TPState = TeleportState.Out;

            if (trans.GridUid == null || !TryGetTpCoords(comp, out var coords))
            {
                comp.TPState = TeleportState.NoTP;
                _appearance.SetData(uid, DamageVisualizerKeys.DamageUpdateGroups, comp.TPState);
                RemComp<AdminFrozenComponent>(uid);
                return;
            }
            //запоминаем грид с которого мы телепортировались
            if (!comp.InDimention)
                comp.StationGridUid = trans.GridUid.Value;
            else
                comp.DimensionGridUid = trans.GridUid.Value;

            _trans.SetCoordinates(uid, coords.Value);
            comp.InDimention = !comp.InDimention;
            _appearance.SetData(uid, DamageVisualizerKeys.DamageUpdateGroups, comp.TPState);
            _audio.PlayPvs(comp.TeleportSound, uid);
        }
        //вышли из телепорта
        if (comp.TPState == TeleportState.Out && now >= comp.TeleportationOutEndAt)
        {
            comp.TPState = TeleportState.NoTP;
            _appearance.SetData(uid, DamageVisualizerKeys.DamageUpdateGroups, comp.TPState);
            RemComp<AdminFrozenComponent>(uid);
        }
    }
    private bool TryGetTpCoords(OldManComponent comp, [NotNullWhen(true)] out EntityCoordinates? coords)
    {
        coords = null;
        //1. Если должны вернуться домой - идем туда
        if (!comp.InDimention)
        {
            if (!TryGetRandomExistingTile(comp.DimensionGridUid, out coords))
                coords = Transform(comp.DimensionUid).Coordinates;

            return true;
        }

        //2. Если должны телепортироваться на станцию - ищем самого хлипкого игрока
        EntityUid? uid = null;
        FixedPoint2 maxDamage = 0;
        var query = EntityQueryEnumerator<MobStateComponent, TransformComponent, DamageableComponent, HumanoidAppearanceComponent>();
        while (query.MoveNext(out var target, out var mob, out var trans, out var dmg, out _))
        {
            //Должен быть в сознании
            if (mob.CurrentState != MobState.Alive)
                continue;

            //Должен быть на гриде где мы телепортировались
            if (trans.GridUid != comp.StationGridUid)
                continue;

            if (dmg.TotalDamage > maxDamage)
            {
                uid = target;
                maxDamage = dmg.TotalDamage;
            }
        }

        if (uid != null)
        {
            coords = Transform(uid.Value).Coordinates;
            return true;
        }

        //3. Если все фуллхп - Просто тпшимся на грид с которого уходили
        if (TryGetRandomExistingTile(comp.StationGridUid, out coords))
            return true;

        //невозможно попасть никуда
        return false;
    }

    public bool TryGetRandomExistingTile(EntityUid gridUid, [NotNullWhen(true)] out EntityCoordinates? coords)
    {
        coords = null;
        if (!Exists(gridUid) || Deleted(gridUid))
            return false;

        if (!TryComp<MapGridComponent>(gridUid, out var grid))
            return false;

        var tiles = _mapSystem.GetAllTiles(gridUid, grid).ToList();
        _random.Shuffle(tiles);
        foreach (var tile in tiles)
        {
            if (_turf.IsTileBlocked(tile, CollisionGroup.MobMask))
                continue;

            coords = new EntityCoordinates(gridUid, tile.GridIndices);
            return true;
        }

        return false;
    }

    private void OnMeleeHit(EntityUid uid, OldManComponent comp, ref MeleeHitEvent args)
    {
        if (!args.IsHit)
            return;

        if (!TryGetRandomExistingTile(comp.DimensionGridUid, out var coords))
            coords = Transform(comp.DimensionUid).Coordinates;

        foreach (var target in args.HitEntities)
        {
            if (!TryComp<MobStateComponent>(target, out var mob))
                continue;
            if (mob.CurrentState == MobState.Dead)
                continue;

            _trans.SetCoordinates(target, coords.Value);
            var victim = EnsureComp<DimensionVictimComponent>(target);
            victim.OldMan = (uid, comp);
            EnsureComp<NoirOverlayComponent>(target);
        }
    }

    private void OnTeleportEvent(EntityUid uid, OldManComponent comp, OldManTeleportEvent args)
    {
        if (args.Handled)
            return;

        if (comp.TPState != TeleportState.NoTP)
            return;

        TeleportOldMan(uid, comp);
        args.Handled = true;
    }

    private void OnMapInit(EntityUid uid, OldManComponent comp, ref MapInitEvent args)
    {
        if (!_mapLoader.TryLoadMap(comp.DimensionMap, out var dimension, out _))
            return;

        _mapSystem.InitializeMap(dimension.Value.Comp.MapId);
        comp.DimensionUid = dimension.Value.Owner;
        comp.PhaseSwitchAt = _timing.CurTime + TimeSpan.FromMinutes(5);
        comp.ActionEnt = _actions.AddAction(uid, comp.ActionId);
        TeleportOldMan(uid, comp);
    }


    #endregion
    #region измерение и жертвы
    private void DamageVictim(EntityUid uid, DimensionVictimComponent comp, TimeSpan now)
    {
        if (now >= comp.NextDamage)
        {
            comp.NextDamage = now + comp.DamageInterval;

            _audio.PlayPvs(comp.DamageSound, uid);
            _popup.PopupEntity("Кожа гниёт на глазах", uid, PopupType.SmallCaution);//туду в фтл
            _damageableSystem.TryChangeDamage(uid, comp.Damage);
        }
    }

    private void OnVictimStateChanged(EntityUid uid, DimensionVictimComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Critical)
        {
            if (_random.Prob(0.8f))
            {
                _damageableSystem.TryChangeDamage(uid, component.FinalDamage);
                return;
            }

            if (TryGetRandomExistingTile(component.DimensionGridUid, out var coords))
                _trans.SetCoordinates(uid, coords.Value);

            _popup.PopupEntity("П О Д Н И М А Й С Я", uid, PopupType.SmallCaution);//туду в фтл
            if (TryComp<DamageableComponent>(uid, out var damagComp))
                _damageableSystem.SetAllDamage((uid, damagComp), 0);
        }
        if (args.NewMobState == MobState.Dead)
            ReturnVictimOnStation(uid, component);
    }

    private void OnRefreshMoveSpeed(EntityUid uid, DimensionVictimComponent component, RefreshMovementSpeedModifiersEvent args)
    {
        args.ModifySpeed(0.5f, 0.5f);
    }

    private void OnVictimInit(EntityUid uid, DimensionVictimComponent comp, ref MapInitEvent args)
    {
        comp.NextDamage = _timing.CurTime + comp.DamageInterval;

        var grid = Transform(uid).GridUid;
        if (grid == null)
        {
            RemComp<DimensionVictimComponent>(uid);
            return;
        }
        comp.DimensionGridUid = grid.Value;
        _jitter.AddJitter(uid, 2, 2);
        _audio.PlayGlobal(comp.DimensionAmbient, uid);

        for (int i = 0; i < comp.TeleportsAmount; i++)
        {
            if (TryGetRandomExistingTile(grid.Value, out var coords))
                comp.Portals.Add(Spawn(comp.TeleportPrototype, coords.Value));
        }

        for (int i = 0; i < comp.FakeTeleportsAmount; i++)
        {
            if (TryGetRandomExistingTile(grid.Value, out var coords))
                comp.Portals.Add(Spawn(comp.FakeTeleportPrototype, coords.Value));
        }
    }

    private void OnCollide(EntityUid uid, DimensionEscapeTeleportComponent comp, ref StartCollideEvent args)
    {
        if (!TryComp<DimensionVictimComponent>(args.OtherEntity, out var victim))
            return;

        QueueDel(uid);

        if (comp.IsFake)
        {
            _audio.PlayGlobal(victim.DimensionEscapeSound, args.OtherEntity);
            return;
        }

        ReturnVictimOnStation(args.OtherEntity, victim);
    }

    private void ReturnVictimOnStation(EntityUid uid, DimensionVictimComponent comp)
    {
        void TP(EntityCoordinates targetCoords)
        {
            _trans.SetCoordinates(uid, targetCoords);
            RemComp<DimensionVictimComponent>(uid);
            RemComp<NoirOverlayComponent>(uid);
            RemCompDeferred<JitteringComponent>(uid);
            _audio.PlayPvs(comp.DimensionEscapeSound, uid);
            foreach (var portal in comp.Portals)
            {
                if (Exists(portal) && !Deleted(portal))
                    QueueDel(portal);
            }
        }

        var grid = comp.OldMan.Comp.StationGridUid;
        if (!Exists(grid) || Deleted(grid))
            return;

        //1. тпшимся к другому игроку
        var query = EntityQueryEnumerator<TransformComponent, HumanoidAppearanceComponent>();
        while (query.MoveNext(out var target, out var trans, out _))
        {
            //Должен быть на гриде где дедушка уходил в карманное измерение последний раз
            if (trans.GridUid != grid)
                continue;

            TP(Transform(target).Coordinates);
            _popup.PopupEntity($"{Name(uid)} падает с потолка", uid, PopupType.LargeCaution);//туду в фтл
            return;
        }

        //2. Если не получилось, то просто тпшимся на грид с которого уходили
        if (TryGetRandomExistingTile(grid, out var coords))
            TP(coords.Value);
    }
    /// <summary>
    /// возврат всех жертв на станцию
    /// </summary>
    private void ReturnAllVictims(Entity<OldManComponent> OldMan)
    {
        var victimQuery = EntityQueryEnumerator<DimensionVictimComponent>();
        while (victimQuery.MoveNext(out var uid, out var comp))
        {
            if (comp.OldMan == OldMan)
                ReturnVictimOnStation(uid, comp);
        }

    }
    #endregion
}
