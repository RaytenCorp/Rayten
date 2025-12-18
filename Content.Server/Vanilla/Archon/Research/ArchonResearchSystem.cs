using Content.Server.Research.Systems;
using Content.Shared.Vanilla.Archon.Research;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Examine;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Utility;
using Robust.Shared.Timing;

namespace Content.Server.Vanilla.Archon.Research;

public sealed partial class ArchonBeaconSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly MobStateSystem _mobstate = default!;
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
            //если маяк не заряжен, или не привязан к серверу, продлеваем изучение
            if (!_power.IsPowered(uid) || !_research.TryGetClientServer(uid, out _, out _))
            {
                foreach (var (archon, researchTime) in beaconComp.LinkedArchons)
                    beaconComp.LinkedArchons[archon] += TimeSpan.FromSeconds(1);

                continue;
            }

            CheckLinks((uid, beaconComp));
            LinkBeaconToArchons((uid, beaconComp));
            ExtractResearchPoints(uid, beaconComp);

            _appearance.SetData(uid, ArchonBeaconVisuals.Link, beaconComp.LinkedArchons.Count > 0);
        }
    }
    /// <summary>
    /// Проверки
    /// 1. Жив ли архонт
    /// 3. В радиусе маяка ли он
    /// это общие для всех архонтов проверки, специальные проверки нужно прописывать в отдельных системах для отдельных архонтов
    /// </summary>
    private void OnAttempt(EntityUid uid, ArchonComponent stunned, ResearchAttemptEvent args)
    {
        if (!_mobstate.IsAlive(uid))
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
            args.PushMarkup(component.LinkedArchons.Count == 0 ? Loc.GetString("archonbeacon-examine-no-links") : Loc.GetString("archonbeacon-examine-header"));
            foreach (var (archon, researchTime) in component.LinkedArchons)
                args.PushMarkup(Loc.GetString("archonbeacon-examine-archon", ("archon", Name(archon)), ("time", (researchTime - now).TotalSeconds)));
        }
    }
    private void OnRemove(EntityUid uid, ArchonComponent component, ref ComponentRemove args)
    {
        if (TryComp<ArchonBeaconComponent>(component.LinkedBeacon, out var beacon))
            beacon.LinkedArchons.Remove(uid);
    }
    /// <summary>
    /// Проверяем текущие соединения и разрываем их в случае нарушений условий содержания
    /// </summary>
    public void CheckLinks(Entity<ArchonBeaconComponent> beacon)
    {
        foreach (var (archon, _) in beacon.Comp.LinkedArchons)
        {
            var ev = new ResearchAttemptEvent(beacon);
            RaiseLocalEvent(archon, ev);
            if (ev.Cancelled)
            {
                beacon.Comp.LinkedArchons.Remove(archon);
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
        var archons = _lookup.GetEntitiesInRange<ArchonComponent>(Transform(beacon.Owner).Coordinates, beacon.Comp.Radius);
        foreach (var archon in archons)
        {
            //уже связан?
            if (archon.Comp.LinkedBeacon != null)
                continue;

            var ev = new ResearchAttemptEvent(beacon);
            RaiseLocalEvent(archon.Owner, ev);
            if (ev.Cancelled)
                continue;

            beacon.Comp.LinkedArchons[archon.Owner] = _timing.CurTime + TimeSpan.FromMinutes(7f);
            archon.Comp.LinkedBeacon = beacon.Owner;
        }
    }
    /// <summary>
    /// Выдаем очки продвинутого изучения
    /// Каждый архонт генерирует отдельные очки
    /// </summary>
    public void ExtractResearchPoints(EntityUid uid, ArchonBeaconComponent component)
    {
        if (!_research.TryGetClientServer(uid, out var server, out var serverComponent))
            return;

        var now = _timing.CurTime;
        foreach (var (archon, researchTime) in component.LinkedArchons)
        {
            if (now < researchTime)
                continue;

            _research.ModifyServerAdvancedPoints(server.Value, 1, serverComponent);
            component.LinkedArchons[archon] = now + TimeSpan.FromMinutes(7f);
        }
    }
}
