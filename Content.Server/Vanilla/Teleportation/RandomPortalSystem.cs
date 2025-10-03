using Content.Server.Atmos.EntitySystems;
using Content.Server.Administration.Logs;
using Content.Server.Station.Components;
using Content.Server.Atmos.Components;
using Content.Server.Station.Systems;
using Content.Server.Procedural;
using Content.Server.Parallax;

using Content.Shared.Vanilla.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Content.Shared.Station.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Parallax.Biomes;
using Content.Shared.Procedural;
using Content.Shared.Parallax;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.Popups;
using Content.Shared.Atmos;

using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Map;

using Robust.Server.GameObjects;
using Robust.Server.Audio;

using System.Collections.Generic;
using System.Numerics;
using System.Linq;


namespace Content.Server.Vanilla.Teleportation;

public sealed class RandomPortalSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly DungeonSystem _dungeonSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;
    [Dependency] private readonly BiomeSystem _biomeSystem = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        {
            SubscribeLocalEvent<PortalMapComponent, ComponentInit>(OnPortalMapInit);
            SubscribeLocalEvent<RandomPortalComponent, MapInitEvent>(OnRandomPortalMapInit);
        }
    }

    private void OnRandomPortalMapInit(Entity<RandomPortalComponent> entity, ref MapInitEvent args)
    {
        var transform = Transform(entity);

        if (entity.Comp.OnlyInStationTeleport)
            StationTeleport(entity, transform);
        else if (entity.Comp.OnlyInMapTeleport)
            InMapTeleport(entity, transform);
        else
            DimensionalTeleport(entity, transform);
    }

    private void StationTeleport(Entity<RandomPortalComponent> portal, TransformComponent trans)
    {
        var stationGrid = FindStationGrid();
        if (stationGrid == null)
        {
            InMapTeleport(portal, trans);
            return;
        }

        if (!TryComp<MapGridComponent>(stationGrid, out var grid))
        {
            InMapTeleport(portal, trans);
            return;
        }

        if (!TryFindSuitableTile(stationGrid.Value, grid, out var coords))
            return;

        SpawnAndLinkPortal(portal, coords);
    }

    private EntityUid? FindStationGrid()
    {
        var query = EntityQueryEnumerator<StationDataComponent>();
        while (query.MoveNext(out var stationUid, out var stationData))
        {
            var gridUid = _stationSystem.GetLargestGrid((stationUid, stationData));
            if (gridUid == null)
                continue;

            var gridTrans = Transform(gridUid.Value);
            if (gridTrans.MapID != MapId.Nullspace)
                return gridUid.Value;
        }
        return null;
    }

    private void InMapTeleport(Entity<RandomPortalComponent> portal, TransformComponent xform)
    {
        var stationGrid = FindStationGrid();

        if (stationGrid == null)
            return;

        var mapId = _transform.GetMapCoordinates(stationGrid.Value).MapId;

        var validGrids = EntityQuery<MapGridComponent, TransformComponent>()
            .Where(x => x.Item2.MapID == mapId && !HasComp<BecomesStationComponent>(x.Item2.GridUid))
            .Select(x => (x.Item2.GridUid, x.Item1))
            .ToList();

        if (validGrids.Count == 0)
            return;

        var (gridUid, grid) = _random.Pick(validGrids);
        if (!TryFindSuitableTile(gridUid!.Value, grid, out var coords))
            return;

        SpawnAndLinkPortal(portal, coords);
    }

    private bool TryFindSuitableTile(EntityUid gridUid, MapGridComponent gridComp, out EntityCoordinates coords)
    {
        var validTiles = new List<Vector2i>();
        var tileEnumerator = _mapSystem.GetAllTilesEnumerator(gridUid, gridComp, true);

        while (tileEnumerator.MoveNext(out var tile))
        {
            var worldPos = _mapSystem.GridTileToWorld(gridUid, gridComp, tile.Value.GridIndices);
            var box = new Box2(worldPos.Position - new Vector2(0.5f), worldPos.Position + new Vector2(0.5f));

            if (!_lookup.GetEntitiesIntersecting(worldPos.MapId, box).Any(e => e != gridUid))
                validTiles.Add(tile.Value.GridIndices);
        }

        if (validTiles.Count == 0)
        {
            coords = default;
            return false;
        }

        coords = _mapSystem.GridTileToLocal(gridUid, gridComp, _random.Pick(validTiles));
        return true;
    }

    private void SpawnAndLinkPortal(Entity<RandomPortalComponent> portal, EntityCoordinates coords)
    {
        var exitPortal = Spawn(portal.Comp.SecondPortalPrototype, coords);
        _link.TryLink(portal, exitPortal, true);
    }

    private void DimensionalTeleport(Entity<RandomPortalComponent> portal, TransformComponent transform)
    {
        if (_random.Prob(0.15f))
            CreateDungeonMap(portal);
        else
            CreatePlanet(portal);
    }

    private void CreateDungeonMap(Entity<RandomPortalComponent> portal)
    {
        if (portal.Comp.AllowedDungeons.Count == 0)
            return;

        var mapId = _mapManager.CreateMap();
        var mapUid = _mapManager.GetMapEntityId(mapId);

        var gravity = EnsureComp<GravityComponent>(mapUid);
        gravity.Enabled = true;

        EnsureComp<PortalMapComponent>(mapUid);

        if (portal.Comp.AllowedParallaxes.Count > 0)
        {
            var parallax = EnsureComp<ParallaxComponent>(mapUid);
            parallax.Parallax = _random.Pick(portal.Comp.AllowedParallaxes);
        }

        var gridUid = _mapManager.CreateGrid(mapId).Owner;
        if (!TryComp<MapGridComponent>(gridUid, out var gridComp))
        {
            _mapManager.DeleteMap(mapId);
            return;
        }

        var dungeonProto = _prototype.Index<DungeonConfigPrototype>(_random.Pick(portal.Comp.AllowedDungeons));
        _dungeonSystem.GenerateDungeonAsync(dungeonProto, gridUid, gridComp, Vector2i.Zero, _random.Next());

        if (_mapManager.MapExists(mapId) && TryFindSuitableTile(gridUid, gridComp, out var coords))
            SpawnAndLinkPortal(portal, coords);
    }

    private void CreatePlanet(Entity<RandomPortalComponent> portal)
    {
        if (portal.Comp.AllowedPlanets.Count == 0)
            return;

        var mapId = _mapManager.CreateMap();
        _mapManager.SetMapPaused(mapId, false);
        var mapUid = _mapManager.GetMapEntityId(mapId);

        var biomeProto = _prototype.Index<BiomeTemplatePrototype>(_random.Pick(portal.Comp.AllowedPlanets));
        _biomeSystem.EnsurePlanet(mapUid, biomeProto);

        var grid = _mapManager.CreateGrid(mapId);
        var gridUid = grid.Owner;

        EnsureComp<PortalMapComponent>(mapUid);

        if (!TryComp<MapGridComponent>(gridUid, out var gridComp))
        {
            _mapManager.DeleteMap(mapId);
            return;
        }

        Gas[] primaryGases = new[] { Gas.Oxygen, Gas.Nitrogen };
        Gas[] secondaryGases;

        float temperature = _random.NextFloat(273f, 313f);

        if (biomeProto.ID == "PortalLava")
        {
            primaryGases = new[] { Gas.Oxygen, Gas.Nitrogen, Gas.CarbonDioxide };
            temperature = _random.NextFloat(800f, 700f);
        }
        else if (biomeProto.ID == "PortalSnow")
        {
            primaryGases = new[] { Gas.Oxygen, Gas.Nitrogen, Gas.Plasma, Gas.WaterVapor };
            temperature = _random.NextFloat(230f, 273f); 
        }

        secondaryGases = new[] { Gas.CarbonDioxide, Gas.Plasma, Gas.Tritium, Gas.Ammonia, Gas.NitrousOxide };

        var mixture = new GasMixture(2500) { Temperature = temperature };

        var selectedGases = primaryGases.OrderBy(_ => _random.Next()).Take(2).ToList();

        foreach (var gas in selectedGases)
            mixture.AdjustMoles(gas, _random.NextFloat(40f, 150f));

        if (primaryGases.Length > 2 && _random.Prob(0.3f))
        {
            var traceGas = _random.Pick(primaryGases.Except(selectedGases).ToArray());
            mixture.AdjustMoles(traceGas, _random.NextFloat(3f, 10f));
        }

        foreach (var gas in secondaryGases)
        {
            if (_random.Prob(0.6f))
            {
                mixture.AdjustMoles(gas, _random.NextFloat(1f, 5f));
            }
        }

        _atmosphereSystem.SetMapAtmosphere(mapUid, false, mixture);

        var zeroCoords = _mapSystem.GridTileToLocal(gridUid, gridComp, Vector2i.Zero);
        var exitPortal = Spawn(portal.Comp.SecondPortalPrototype, zeroCoords);
        _link.TryLink(portal, exitPortal, true);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<PortalMapComponent, MapComponent>();
        var currentTime = _timing.CurTime;

        while (query.MoveNext(out var uid, out var comp, out var mapComp))
        {
            if (currentTime < comp.NextCheckTime)
                continue;

            comp.NextCheckTime = currentTime + TimeSpan.FromSeconds(comp.UpdateRate);

            if (HasActivePlayers(mapComp.MapId) || !comp.Enabled)
                continue;

            _mapSystem.DeleteMap(mapComp.MapId);
        }
    }

    private bool HasActivePlayers(MapId mapId)
    {
        var playerQuery = EntityQueryEnumerator<ActorComponent, TransformComponent>();
        while (playerQuery.MoveNext(out _, out _, out var trans))
        {
            if (trans.MapID == mapId)
                return true;
        }

        return false;
    }

    private void OnPortalMapInit(Entity<PortalMapComponent> entity, ref ComponentInit args)
    {
        entity.Comp.NextCheckTime = _timing.CurTime + TimeSpan.FromSeconds(entity.Comp.UpdateRate);
    }
}
