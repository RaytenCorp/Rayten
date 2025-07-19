using Content.Server.GameTicking.Rules.Components;
using Content.Server.Administration.Managers;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Temperature.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Light.EntitySystems;
using Content.Server.Station.Components;
using Content.Server.GameTicking.Rules;
using Content.Server.Tesla.Components;
using Content.Server.Cargo.Components;
using Content.Server.Atmos.Components;
using Content.Server.Station.Systems;
using Content.Server.Light.Components;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Power.SMES;
using Content.Server.RoundEnd;
using Content.Server.Weather;
using Content.Server.Damage;
using Content.Server.Resist;
using Content.Server.Audio;
using Content.Server.Roles;

using Content.Shared.GameTicking.Components;
using Content.Shared.Shuttles.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Weather;
using Content.Shared.Salvage;
using Content.Shared.Damage;
using Content.Shared.Atmos;
using Content.Shared.Audio;
using Content.Shared.Maps;
using Content.Shared.Tag;

using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Timing;
using Robust.Shared.Random;
using Robust.Shared.Player;
using Robust.Shared.Audio;
using Robust.Shared.Map;

using Robust.Server.Player;

using System.Linq;

namespace Content.Server.Vanilla.GameTicking.Rules.WhiteOut;

public sealed class WhiteoutRuleSystem : GameRuleSystem<WhiteoutRuleComponent>
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly PoweredLightSystem _poweredLight = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly RoundEndSystem _roundEnd = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly WeatherSystem _weather = default!;
    [Dependency] private readonly ExplosionSystem _boom = default!;
    [Dependency] private readonly TagSystem _tagSystem = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly TurfSystem _turf = default!;



    public override void Initialize()
    {
        base.Initialize();
    }

    // Базовые действия при начале геймрула
    protected override void Started(EntityUid uid, WhiteoutRuleComponent comp, GameRuleComponent rule, GameRuleStartedEvent args)
    {
        base.Started(uid, comp, rule, args);

        // Сброс значений
        comp.TimeActive = 0f;
        comp.NextUpdate = _gameTiming.CurTime + TimeSpan.FromSeconds(1);
        comp.CurrentState = WhiteoutState.Preparing;
        comp.NextGlassBreak = TimeSpan.Zero;
        comp.PrestartPlayed = false;

        // Установление значении карты
        var stationQuery = EntityQueryEnumerator<StationDataComponent>();
        while (stationQuery.MoveNext(out var stationUid, out var stationData))
        {

            var gridUid = _stationSystem.GetLargestGrid(stationData);
            if (gridUid == null)
                continue;

            var gridTrans = Transform(gridUid.Value); // хаха грид транс
            if (gridTrans.MapID == MapId.Nullspace)
                continue;

            comp.ActiveMapId = gridTrans.MapID;
            break;
        }

        // UID карты
        comp.ActiveMapUid = _mapManager.GetMapEntityId(comp.ActiveMapId);

        // Удаление компонента трейдпоста у поста для усложнения
        var query = EntityQueryEnumerator<TradeStationComponent>();
        while (query.MoveNext(out var tradepost, out _))
        {
            RemComp<TradeStationComponent>(tradepost);
        }

        // _jammer.SetJammer(TimeSpan.FromSeconds(comp.WhiteoutLength+comp.WhiteoutFinalLength+comp.WhiteoutPrepareTime));

        _chat.DispatchGlobalAnnouncement(Loc.GetString(comp.WhiteoutPrepareAnnouncement), playSound: true, announcementSound: comp.WhiteoutSoundAnnouncement, colorOverride: Color.Red);

    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var currentTime = _gameTiming.CurTime;
        var query = EntityQueryEnumerator<WhiteoutRuleComponent, GameRuleComponent>();

        while (query.MoveNext(out var uid, out var comp, out var rule))
        {
            if (currentTime < comp.NextUpdate) continue;

            comp.NextUpdate = currentTime + TimeSpan.FromSeconds(1);
            comp.TimeActive += 1f;
            ProcessWhiteout(uid, comp, rule);
        }
    }

    // Сам вайтаут
    private void ProcessWhiteout(EntityUid uid, WhiteoutRuleComponent comp, GameRuleComponent rule)
    {
        var now = _gameTiming.CurTime;
        switch (comp.CurrentState)
        {
            case WhiteoutState.Preparing:

                if (comp.TimeActive >= comp.WhiteoutPrepareTime)
                {
                    StartWhiteout(uid, comp);
                    comp.CurrentState = WhiteoutState.Active;
                }
                else
                {
                    float prepareStrength = comp.WhiteoutPrepareStrength * (comp.TimeActive / comp.WhiteoutPrepareTime);

                    Freeze(comp.WhiteoutPrepareTemp, prepareStrength, comp.ActiveMapId);

                    if (comp.TimeActive >= comp.WhiteoutPrepareTime - 240f & !comp.PrestartPlayed)
                    {
                        comp.PrestartPlayed = true;
                        _audio.PlayGlobal(comp.WhiteoutPrestartMusic, Filter.Broadcast(), true);
                        _chat.DispatchGlobalAnnouncement(Loc.GetString(comp.WhiteoutPrestartAnnouncement), colorOverride: Color.Red);

                        SetWeather(comp, comp.PrestartWeather);
                    }
                }

                break;

            case WhiteoutState.Active:
            case WhiteoutState.FinalPhase:
                var isFinal = comp.CurrentState == WhiteoutState.FinalPhase;
                var (temp, strength) = comp.GetWhiteoutParams(isFinal);
                Freeze(temp, strength, comp.ActiveMapId);
                ExplodeSmes(comp.ActiveMapUid, comp.ActiveMapId);
                ChangeTiles(comp.ActiveMapId);

                // Повреждение объектов
                if (now >= comp.NextGlassBreak)
                {
                    var damage = new DamageSpecifier();
                    damage.DamageDict.Add("Blunt", FixedPoint2.New(130));

                    //лампы
                    var lampQuery = EntityQueryEnumerator<PoweredLightComponent, TransformComponent>();
                    while (lampQuery.MoveNext(out var lampEnt, out var light, out var xform))
                    {
                        if (CheckTileTemperature(lampEnt, 183.15f) && RobustRandom.Prob(0.3f) && xform.MapID == comp.ActiveMapId)
                        {
                            _poweredLight.TryDestroyBulb(lampEnt, light);
                        }
                    }
                    //шкафы
                    var lockerQuery = EntityQueryEnumerator<ResistLockerComponent, TransformComponent>();
                    while (lockerQuery.MoveNext(out var lockerEnt, out _, out var xform))
                    {
                        if (CheckTileTemperature(lockerEnt, 133.15f) && RobustRandom.Prob(0.5f) && xform.MapID == comp.ActiveMapId)
                        {
                            _damageable.TryChangeDamage(lockerEnt, damage);
                        }
                    }
                    //окна
                    if (isFinal)
                    {
                        var windows = new List<EntityUid>();
                        var windowQuery = EntityQueryEnumerator<TransformComponent>();
                        while (windowQuery.MoveNext(out var windowEnt, out var xform))
                        {
                            if (_tagSystem.HasTag(windowEnt, "Window") &&
                                xform.MapID == comp.ActiveMapId &&
                                RobustRandom.Prob(0.8f))
                            {
                                windows.Add(windowEnt);
                            }
                        }

                        foreach (var window in windows)
                        {
                            _damageable.TryChangeDamage(window, damage);
                        }
                    }

                    comp.NextGlassBreak = now + TimeSpan.FromSeconds(5);
                }

                var totalDuration = comp.WhiteoutLength + comp.WhiteoutFinalLength;
                var finalPhaseEndWarning = totalDuration - 60f;

                // Первичные действия при инициализации финала
                if (!isFinal)
                {
                    if (comp.TimeActive >= comp.WhiteoutLength)
                    {
                        GameTicker.AddGameRule("GameRuleMeteorSwarmLarge");
                        // Атмосфера -250
                        MakeAtmos(comp.WhiteoutFinalTemp, comp.PlanetMap, comp.ActiveMapId, comp.ActiveMapUid);

                        comp.CurrentState = WhiteoutState.FinalPhase;

                        _audio.PlayGlobal(comp.WhiteoutFinalMusic, Filter.Broadcast(), true);
                        _audio.PlayGlobal(comp.WhiteoutAlarmSound, Filter.Broadcast(), true);
                        _chat.DispatchGlobalAnnouncement(Loc.GetString(comp.WhiteoutFinalAnnouncement), playSound: false, colorOverride: Color.Red);

                        // Добавления эффекта "облаков"
                        var restrictedRange = new RestrictedRangeComponent
                        {
                            Range = 0
                        };

                        AddComp(comp.ActiveMapUid, restrictedRange);
                    }
                }
                // Действия инициализации конца бури
                else
                {
                    if (comp.TimeActive >= finalPhaseEndWarning)
                    {
                        _roundEnd.RequestRoundEnd(TimeSpan.FromMinutes(1), uid, false, "whiteout-evac", "department-CentralCommand");
                    }

                    if (comp.TimeActive >= totalDuration)
                    {
                        EndWhiteout(uid, comp, rule);
                        comp.CurrentState = WhiteoutState.Ended;
                    }
                }
                break;
        }
    }


    // Действия при начале
    private void StartWhiteout(EntityUid uid, WhiteoutRuleComponent comp)
    {
        comp.TimeActive = 0f;

        MakeAtmos(comp.WhiteoutTemp, comp.PlanetMap, comp.ActiveMapId, comp.ActiveMapUid);

        GameTicker.AddGameRule("MeteorSwarmWhiteoutScheduler");
        GameTicker.AddGameRule("GameRuleMeteorSwarmLarge");

        _audio.PlayGlobal(comp.WhiteoutMusic, Filter.Broadcast(), true);

        SetWeather(comp, comp.Weather);

        ChangeHardsuitProtection(true);

        _chat.DispatchGlobalAnnouncement(Loc.GetString(comp.WhiteoutAnnouncement), playSound: false, colorOverride: Color.Red);
        _audio.PlayGlobal(comp.WhiteoutAlarmSound, Filter.Broadcast(), true);

        // Создание карты для лондонцев
        if (HasAnyLondoners())
        {
            var mapId = _mapManager.CreateMap();

            var path = new ResPath("/Maps/Vanilla/Misc/NewLondon.yml");
            _mapLoader.TryLoadGrid(mapId, path, out var grid);

            comp.LondonersMapId = mapId;
            comp.LondonersMapUid = _mapManager.GetMapEntityId(comp.LondonersMapId);

            _metaData.SetEntityName(comp.LondonersMapUid, "Аванпост Новый Лондон");

            AddComp<FTLDestinationComponent>(comp.LondonersMapUid);
        }
    }

    // Действия при конце
    private void EndWhiteout(EntityUid uid, WhiteoutRuleComponent comp, GameRuleComponent rule)
    {
        SetWeather(comp, "null");

        RemComp<MapAtmosphereComponent>(comp.ActiveMapUid);
        RemComp<RestrictedRangeComponent>(comp.ActiveMapUid);

        ChangeHardsuitProtection(false);

        _chat.DispatchGlobalAnnouncement(Loc.GetString(comp.WhiteoutEndAnnouncement), playSound: false, colorOverride: Color.Red);

        GameTicker.EndGameRule(uid, rule);
    }

    // Заморозка газов
    private void Freeze(float targetTemp, float strength, MapId mapId)
    {
        var query = EntityQueryEnumerator<GridAtmosphereComponent, TransformComponent>();
        while (query.MoveNext(out var gridUid, out var grid, out var gridXform))
        {
            if (gridXform.MapID != mapId)
                continue;

            foreach (var tile in grid.Tiles.Values)
            {
                if (tile.Air is not { Pressure: >= 1f, TotalMoles: > 0f } mixture)
                    continue;

                var heatCap = _atmosphere.GetHeatCapacity(mixture, false);
                if (heatCap <= 0)
                    continue;

                var temp = mixture.Temperature;
                if (temp <= targetTemp)
                    continue;

                float delta = (temp - targetTemp) * strength;
                mixture.Temperature = Math.Max(targetTemp, temp - delta);
            }
        }
    }

    private void SetWeather(WhiteoutRuleComponent comp, string weatherType)
    {

        if (weatherType == "null")
        {
            _weather.SetWeather(comp.ActiveMapId, null, TimeSpan.FromMinutes(60));
        }

        if (!_prototypeManager.TryIndex<WeatherPrototype>(weatherType, out var weatherProto))
            return;

        var weatherComp = EntityManager.EnsureComponent<WeatherComponent>(comp.ActiveMapUid);

        _weather.SetWeather(comp.ActiveMapId, weatherProto, TimeSpan.FromMinutes(60));
    }

    // Убирание или добавление резистов скафов
    private void ChangeHardsuitProtection(bool remove)
    {
        var query = EntityQueryEnumerator<TransformComponent>();
        while (query.MoveNext(out var uid, out var ass))
        {
            if (!_tagSystem.HasTag(uid, "Hardsuit") || !_tagSystem.HasTag(uid, "HardsuitHelmet"))
                return;

            if (remove)
            {
                RemComp<TemperatureProtectionComponent>(uid);
                RemComp<PressureProtectionComponent>(uid);
            }
            else
            {
                AddComp<TemperatureProtectionComponent>(uid);
                var pressureProtection = AddComp<PressureProtectionComponent>(uid);
                pressureProtection.LowPressureMultiplier = 1000f;
                pressureProtection.HighPressureMultiplier = 0.3f;
            }
        }
    }

    // Взрыв смэсов
    private void ExplodeSmes(EntityUid ActiveMapUid, MapId ActiveMapId)
    {
        if (!Exists(ActiveMapUid))
            return;

        var query = EntityQueryEnumerator<SmesComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out _, out var xform))
        {
            if (xform.MapID == ActiveMapId && CheckTileTemperature(uid, 133.15f))
                _boom.QueueExplosion(uid, "Cryo", 200, 10, 200);
        }
    }

    // Проверка температуры тайла энтити, для проведения действий
    private bool CheckTileTemperature(EntityUid uid, float threshold)
    {
        if (!Exists(uid) || Deleted(uid))
            return false;

        if (!TryComp<TransformComponent>(uid, out var transform) || transform.GridUid == null)
            return false;

        if (!TryComp<MapGridComponent>(transform.GridUid, out var grid))
            return false;

        var tile = grid.TileIndicesFor(transform.Coordinates);
        var atmosphere = _atmosphere.GetTileMixture(transform.GridUid, null, tile, true);

        return atmosphere?.Temperature < threshold;
    }

    // Смена тайлов на снежочек
    private void ChangeTiles(MapId ActiveMapId)
    {
        float temp = 122f;

        if (!_prototypeManager.TryIndex<ContentTileDefinition>("FloorSnowPlating", out var iceTile))
            return;

        var query = EntityQueryEnumerator<GridAtmosphereComponent, MapGridComponent>();
        while (query.MoveNext(out var gridUid, out var gridAtmos, out var grid))
        {
            if (Transform(gridUid).MapID != ActiveMapId)
                continue;

            foreach (var (tile, atmosTile) in gridAtmos.Tiles)
            {

                if (atmosTile.Air == null)
                    continue;

                bool shouldChange = atmosTile.Air.Temperature < temp || atmosTile.Air.GetMoles(Gas.Frezon) > 0f;

                if (!shouldChange)
                    continue;

                var tileRef = grid.GetTileRef(tile);
                var tileDef = _turf.GetContentTileDefinition(tileRef);

                if (tileRef.Tile.IsEmpty ||
                    tileDef.ID == "FloorSnowPlating" ||
                    tileDef.ID == "Lattice")
                    continue;

                if (RobustRandom.Prob(0.3f))
                {
                    grid.SetTile(tile, new Tile(iceTile.TileId));
                }
            }
        }
    }

    // Создание атмосферы при начале бури, реалистик и +сложность
    private void MakeAtmos(float temp, bool planet, MapId ActiveMapId, EntityUid ActiveMapUid)
    {
        if (!_mapManager.MapExists(ActiveMapId))
            return;

        var moles = new float[Atmospherics.AdjustedNumberOfGases];

        if (planet)
        {
            moles[(int)Gas.Oxygen] = 43.649558f;
            moles[(int)Gas.Nitrogen] = 164.20624f;
        }
        else
        {
            moles[(int)Gas.Oxygen] = 126f;
            moles[(int)Gas.Frezon] = 141f;
            moles[(int)Gas.NitrousOxide] = 132f;
        }

        var mixture = new GasMixture(moles, temp);

        if (Exists(ActiveMapUid))
        {
            var mapAtmos = EnsureComp<MapAtmosphereComponent>(ActiveMapUid);
            _atmosphere.SetMapAtmosphere(ActiveMapUid, false, mixture);
        }
    }

    private bool HasAnyLondoners()
    {
        var query = EntityQueryEnumerator<LondonerRoleComponent>();
        return query.MoveNext(out _, out _);
    }

}
