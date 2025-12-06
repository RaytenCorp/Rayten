using Content.Shared.Archon.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.GameTicking;
using Robust.Shared.Prototypes;
using Content.Shared.Popups;
using Content.Shared.Paper;
using Content.Shared.Radio;

using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

using Content.Server.Radio.EntitySystems;
using Content.Server.Research.Systems;

using System.Text.RegularExpressions;
using System.Text;

namespace Content.Server.Archon.Systems;

public sealed partial class ArchonResearchSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedPowerReceiverSystem _power = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ResearchSystem _research = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly RadioSystem _radio = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArchonScannerComponent, AfterInteractEvent>(OnInteract);
    }

    private int UpdateSpeed = 2;
    private TimeSpan NextUpdate;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;

        if (curTime < NextUpdate)
            return;

        NextUpdate = curTime + TimeSpan.FromSeconds(UpdateSpeed);

        var beaconQuery = EntityQueryEnumerator<ArchonBeaconComponent, TransformComponent>();

        while (beaconQuery.MoveNext(out var uid, out var beaconComp, out var trans))
        {
            BeaconUpdate(uid, beaconComp, trans);
        }
    }

    private void BeaconUpdate(EntityUid uid, ArchonBeaconComponent comp, TransformComponent xform)
    {
        if (!_power.IsPowered(uid))
        {
            _appearance.SetData(uid, ArchonBeaconVisuals.Classes, ArchonBeaconClasses.NonPowered);
            return;
        }

        if (!TryComp<ArchonDataComponent>(comp.LinkedArchon, out var dataComp))
        {
            _appearance.SetData(uid, ArchonBeaconVisuals.Classes, ArchonBeaconClasses.None);
            return;
        }

        if (CheckInContainment(uid, comp, dataComp, xform))
        {
            int mod = 1;

            // Кол-во очков модифируется относительно класса архонта
            if (comp.ModificatePointsByClass)
            {
                mod = dataComp.Class switch
                {
                    ArchonClass.Safe => 1,
                    ArchonClass.Keter => 2,
                    ArchonClass.Euclid => 3,
                    ArchonClass.Thaumiel => 4,
                };
            }

            if (!_research.TryGetClientServer(uid, out var server, out var serverComponent))
                return;

            _research.ModifyServerPoints(server.Value, comp.ResearchPointsPerSecond * mod, serverComponent);
        }
    }

    /// <summary>
    /// Система сканера архонтов
    /// </summary>
    private void OnInteract(EntityUid uid, ArchonScannerComponent comp, AfterInteractEvent args)
    {
        if (args.Handled)
            return;

        if (args.Target is not { } target)
            return;

        if (!args.CanReach)
            return;

        // При сканировании архонта
        if (TryComp<ArchonDataComponent>(target, out var dataComp))
        {
            comp.LinkedArchon = target;

            _audio.PlayPvs(comp.ScanSound, uid);
            _popup.PopupEntity($"Архон просканирован, сигнатура: {comp.LinkedArchon.Value}", uid);
        }
        // Передача архонта маяку
        else if (TryComp<ArchonBeaconComponent>(target, out var beaconComp) && TryComp<ArchonDataComponent>(comp.LinkedArchon, out var dataComp2) && comp.LinkedArchon != null && _power.IsPowered(target))
        {

            beaconComp.LinkedArchon = comp.LinkedArchon;

            if (dataComp2.Beacon != null && TryComp<ArchonBeaconComponent>(dataComp2.Beacon, out var beaconCompToNull))
            {
                beaconCompToNull.LinkedArchon = null;
            }

            dataComp2.Beacon = target;

            _audio.PlayPvs(comp.LoadSound, uid);
            _popup.PopupEntity("Сигнатура Архонта передана маяку", uid);

            SetClass(target, beaconComp);

        }
    }

    /// <summary>
    /// Устанавливает визуал класса
    /// </summary>
    private void SetClass(EntityUid uid, ArchonBeaconComponent comp)
    {
        if (!TryComp<ArchonDataComponent>(comp.LinkedArchon, out var dataComp))
            return;

        var visualState = dataComp.Class switch
        {
            ArchonClass.Safe => ArchonBeaconClasses.Safe,
            ArchonClass.Euclid => ArchonBeaconClasses.Euclid,
            ArchonClass.Keter => ArchonBeaconClasses.Keter,
            ArchonClass.Thaumiel => ArchonBeaconClasses.Thaumiel
        };

        _appearance.SetData(uid, ArchonBeaconVisuals.Classes, visualState);
    }

    /// <summary>
    /// Проверка состояний архонта - сбежал, на содержании, не найден/списан
    /// </summary>
    private bool CheckInContainment(EntityUid uid, ArchonBeaconComponent comp, ArchonDataComponent dataComp, TransformComponent xform)
    {
        if (comp.LinkedArchon == null || !TryComp<TransformComponent>(comp.LinkedArchon, out var archonXform))
        {
            _appearance.SetData(uid, ArchonBeaconVisuals.Classes, ArchonBeaconClasses.None);
            return false;
        }

        var beaconPos = _trans.GetWorldPosition(xform);
        var archonPos = _trans.GetWorldPosition(archonXform);
        var distance = (beaconPos - archonPos).Length();

        if (distance > comp.Radius && comp.Breached == false)
        {

            comp.Breached = true;
            _appearance.SetData(uid, ArchonBeaconVisuals.Classes, ArchonBeaconClasses.Breach);

            string message = "Обнаружено нарушение условий содержания аномального архонт объекта!";

            _radio.SendRadioMessage(uid, message, _prototypeManager.Index<RadioChannelPrototype>(comp.ScienceChannel), uid);

            return false;
        }
        else if (comp.Breached == true)
        {
            comp.Breached = false;

            SetClass(uid, comp);
        }

        return true;
    }
}
