using Content.Server.Research.Systems;
using Content.Shared.Vanilla.Archon.Research;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Utility;
using Robust.Shared.Timing;

namespace Content.Server.Vanilla.Archon.Research;

public sealed partial class ArchonBeaconSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedPowerReceiverSystem _power = default!;
    [Dependency] private readonly ResearchSystem _research = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    private TimeSpan NextUpdate;

    public override void Initialize()
    {
        SubscribeLocalEvent<ArchonComponent, ResearchAttemptEvent>(OnAttempt);
        SubscribeLocalEvent<ArchonComponent, ComponentRemove>(OnRemove);
        SubscribeLocalEvent<ArchonBeaconComponent, ExaminedEvent>(OnExamine);
        base.Initialize();
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var now = _timing.CurTime;

        if (now < NextUpdate)
            return;

        NextUpdate = now + TimeSpan.FromSeconds(1);

        var query = EntityQueryEnumerator<ArchonBeaconComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var beaconComp, out var beaconTrans))
        {
            //если маяк не заряжен, или не привязан к серверу, или существуют другие маяки рядом, то продлеваем изучение
            if (!_power.IsPowered(uid) || !_research.TryGetClientServer(uid, out _, out _) || CheckAnyOtherBeacons((uid, beaconComp)))
            {
                if (beaconComp.LinkedArchon != null)
                    beaconComp.ResearchTime += TimeSpan.FromSeconds(1);

                continue;
            }

            CheckLink((uid, beaconComp));
            LinkBeaconToArchons((uid, beaconComp));
            ExtractResearchPoints(uid, beaconComp);

            _appearance.SetData(uid, ArchonBeaconVisuals.Link, beaconComp.LinkedArchon != null);
        }
    }
    /// <summary>
    /// Проверки
    /// 1. Жив ли архонт
    /// 3. В радиусе маяка ли он
    /// это общие для всех архонтов проверки, специальные проверки нужно прописывать в отдельных системах для отдельных архонтов
    /// </summary>
    private void OnAttempt(EntityUid uid, ArchonComponent component, ResearchAttemptEvent args)
    {
        if (TryComp<MobStateComponent>(uid, out var mobstate) && mobstate.CurrentState != MobState.Alive)
            args.Cancel();

        Transform(uid).Coordinates.TryDistance(EntityManager, Transform(args.Beacon.Owner).Coordinates, out var distance);

        if (distance > args.Beacon.Comp.Radius)
            args.Cancel();
    }

    private void OnExamine(EntityUid uid, ArchonBeaconComponent component, ExaminedEvent args)
    {
        if (!args.IsInDetailsRange || !_power.IsPowered(uid))
            return;

        var now = _timing.CurTime;
        using (args.PushGroup(nameof(ArchonBeaconComponent)))
        {
            if (component.LinkedArchon is { } archon)
            {
                args.PushMarkup(Loc.GetString("archonbeacon-examine-header"));
                args.PushMarkup(Loc.GetString(
                    "archonbeacon-examine-archon",
                    ("archon", Name(archon)),
                    ("time", (component.ResearchTime - now).TotalSeconds)
                ));
            }
            else
                args.PushMarkup(Loc.GetString("archonbeacon-examine-no-links"));
        }
    }

    private void OnRemove(EntityUid uid, ArchonComponent component, ref ComponentRemove args)
    {
        if (TryComp<ArchonBeaconComponent>(component.LinkedBeacon, out var beacon))
            beacon.LinkedArchon = null;
    }

    /// <summary>
    /// Проверяем другие маяки в радиусе
    /// чтобы нельзя было настакать несколько маяков в одной комнате
    /// </summary>
    public bool CheckAnyOtherBeacons(Entity<ArchonBeaconComponent> beacon)
    {
        var beacons = _lookup.GetEntitiesInRange<ArchonBeaconComponent>(Transform(beacon.Owner).Coordinates, beacon.Comp.Radius);
        foreach (var otherbeacon in beacons)
        {
            if (otherbeacon != beacon)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Проверяем текущее соединения и разрываем его в случае нарушений условий содержания
    /// </summary>
    public void CheckLink(Entity<ArchonBeaconComponent> beacon)
    {
        if (beacon.Comp.LinkedArchon is { } archon)
        {
            var ev = new ResearchAttemptEvent(beacon);
            RaiseLocalEvent(archon, ev);
            if (ev.Cancelled)
            {
                beacon.Comp.LinkedArchon = null;
                if (TryComp<ArchonComponent>(archon, out var archonComp))
                    archonComp.LinkedBeacon = null;
            }
        }
    }

    /// <summary>
    /// Связываем еще не связанных с маяком архонтов вокруг маяка с маяком
    /// </summary>
    public void LinkBeaconToArchons(Entity<ArchonBeaconComponent> beacon)
    {
        // если маяк уже занят — ничего не делаем
        if (beacon.Comp.LinkedArchon != null)
            return;

        var archons = _lookup.GetEntitiesInRange<ArchonComponent>(
            Transform(beacon.Owner).Coordinates,
            beacon.Comp.Radius);

        foreach (var archon in archons)
        {
            // архонт уже связан с другим маяком
            if (archon.Comp.LinkedBeacon != null)
                continue;

            var ev = new ResearchAttemptEvent(beacon);
            RaiseLocalEvent(archon.Owner, ev);
            if (ev.Cancelled)
                continue;

            beacon.Comp.LinkedArchon = archon.Owner;
            beacon.Comp.ResearchTime = _timing.CurTime + archon.Comp.ResearchTime;
            archon.Comp.LinkedBeacon = beacon.Owner;
            break;
        }
    }

    /// <summary>
    /// Выдаем очки продвинутого изучения
    /// </summary>
    public void ExtractResearchPoints(EntityUid uid, ArchonBeaconComponent component)
    {
        if (!_research.TryGetClientServer(uid, out var server, out var serverComponent))
            return;

        if (component.LinkedArchon is not { } entry)
            return;

        var now = _timing.CurTime;

        if (now < component.ResearchTime)
            return;

        if (!TryComp<ArchonComponent>(entry, out var archonComp))
            return;

        _research.ModifyServerAdvancedPoints(server.Value, 1, serverComponent);
        component.ResearchTime = now + archonComp.ResearchTime;
    }

}
