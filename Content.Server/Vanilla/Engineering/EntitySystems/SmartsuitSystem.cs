using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Content.Shared.Vanilla.Engineering.Components;
using Content.Shared.Inventory;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Toggleable;
using Content.Shared.Verbs;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Clothing.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Prototypes;
using Content.Shared.Verbs;
using Content.Shared.Popups;
using Robust.Shared.Utility;

namespace Content.Server.Vanilla.Engineering.EntitySystems;

public sealed class SmartsuitSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphere = null!;
    [Dependency] private readonly TransformSystem _transform = null!;
    [Dependency] private readonly InventorySystem _inventory = null!;
    [Dependency] private readonly IGameTiming _gameTiming = null!;
    [Dependency] private readonly ChatSystem _chat = null!;
    [Dependency] private readonly IPrototypeManager _proto = null!;
    [Dependency] private readonly ClothingSystem _clothing = null!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SmartsuitComponent, ToggleClothingEvent>(OnForceToggle);
        SubscribeLocalEvent<SmartsuitComponent, GetVerbsEvent<Verb>>(OnGetVerbs);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<SmartsuitComponent, ClothingComponent>();
        var curTime = _gameTiming.CurTime;

        while (query.MoveNext(out var uid, out var comp, out var clothing))
        {
            if (curTime < comp.NextUpdate)
                continue;

            comp.NextUpdate = curTime + comp.UpdateSpeed;

            if (!comp.Activated)
                continue;

            if (!TryComp<TransformComponent>(uid, out var xform))
                continue;

            var wearer = xform.ParentUid;
            if (wearer == EntityUid.Invalid || wearer == uid)
                continue;

            if (!_inventory.TryGetSlotEntity(wearer, "outerClothing", out var equipped) || equipped != uid)
                continue;

            CheckEnvironment(uid, wearer, comp);
            CheckUser(uid, wearer, comp);
            AbsorbGas(uid, wearer, comp);
        }
    }

    private void CheckUser(EntityUid suitUid, EntityUid wearer, SmartsuitComponent comp)
    {
        if (!TryComp<MetaDataComponent>(wearer, out var meta))
            return;

        bool isNitrogenBreather = meta.EntityPrototype?.ID == "MobSlimePerson" ||
                                 meta.EntityPrototype?.ID == "MobVox";

        if ((isNitrogenBreather && comp.GasType != Gas.Nitrogen) ||
            (!isNitrogenBreather && comp.GasType != Gas.Oxygen))
        {
            comp.GasType = isNitrogenBreather ? Gas.Nitrogen : Gas.Oxygen;

            if (comp.Reporting)
            {
                var gasName = comp.GasType == Gas.Nitrogen ? "азота" : "кислорода";
                _chat.TrySendInGameICMessage(
                    suitUid,
                    $"Обнаружен новый пользователь. Переключено поглощение {gasName}.",
                    InGameICChatType.Speak,
                    true);
            }

            if (TryComp<GasTankComponent>(suitUid, out var gasTank) &&
                gasTank.Air != null &&
                gasTank.Air.TotalMoles > 0)
            {
                if (TryComp<TransformComponent>(wearer, out var transform))
                {
                    var environment = _atmosphere.GetContainingMixture(wearer, false, true);
                    if (environment != null && gasTank.Air != null)
                    {
                        _atmosphere.Merge(environment, gasTank.Air);
                        gasTank.Air.Clear();
                    }
                }
            }
        }
    }

    private void AbsorbGas(EntityUid suitUid, EntityUid wearer, SmartsuitComponent comp)
    {
        if (!TryComp<GasTankComponent>(suitUid, out var gasTank) || gasTank.Air == null)
            return;

        var availableVolume = gasTank.Air.Volume - gasTank.Air.TotalMoles;
        if (availableVolume <= 0)
            return;

        if (gasTank.Air.Pressure >= 1000f)
            return;

        if (!TryComp<TransformComponent>(wearer, out var transform))
            return;

        var gridUid = transform.GridUid;
        var mapUid = transform.MapUid;
        var coords = _transform.GetGridOrMapTilePosition(wearer, transform);

        if (gridUid == null && mapUid == null)
            return;

        var atmosphere = _atmosphere.GetTileMixture(gridUid ?? mapUid, null, coords, true);
        if (atmosphere == null)
            return;

        var availableGas = atmosphere.GetMoles(comp.GasType);
        if (availableGas <= 0)
            return;

        var transferAmount = Math.Min(
            comp.AbsorptionRate,
            Math.Min(availableGas, availableVolume)
        );

        if (transferAmount <= 0)
            return;

        atmosphere.AdjustMoles(comp.GasType, -transferAmount);

        gasTank.Air.AdjustMoles(comp.GasType, transferAmount);

        _atmosphere.React(gasTank.Air, gasTank);
    }

    private void CheckEnvironment(EntityUid suitUid, EntityUid wearer, SmartsuitComponent comp)
    {
        if (!TryComp<TransformComponent>(wearer, out var transform))
            return;

        var gridUid = transform.GridUid;
        var mapUid = transform.MapUid;
        var coords = _transform.GetGridOrMapTilePosition(wearer, transform);

        if (gridUid == null && mapUid == null)
            return;

        var atmosphere = _atmosphere.GetTileMixture(gridUid ?? mapUid, null, coords, true);

        if (atmosphere == null)
        {
            Process(suitUid, wearer, "Обнаружен вакуум!", comp);
            return;
        }

        var pressure = atmosphere.Pressure;
        if (pressure < 40f)
        {
            Process(suitUid, wearer, "Обнаружено критически низкое давление!", comp);
            return;
        }
        else if (pressure > 200f)
        {
            Process(suitUid, wearer, "Обнаружено критически высокое давление!", comp);
            return;
        }

        var temperature = atmosphere.Temperature;
        if (temperature < 223.15f) // -50°C
        {
            Process(suitUid, wearer, "Внимание! Критически низкая температура!", comp);
            return;
        }
        else if (temperature > 343.15f) // 70°C
        {
            Process(suitUid, wearer, "Внимание! Критически высокая температура!", comp);
            return;
        }

        foreach (var gas in Enum.GetValues<Gas>())
        {
            if (gas == Gas.Oxygen || gas == Gas.Nitrogen)
                continue;

            var gasAmount = atmosphere.GetMoles(gas);
            if (gasAmount > 7f)
            {
                if (comp.Reporting)
                {
                    Process(suitUid, wearer, $"Обнаружены вредоносные газы: {Loc.GetString($"gases-{gas.ToString().ToLower()}")} {gasAmount.ToString("F1")} моль!", comp);
                }
                return;
            }
        }

        Process(suitUid, wearer, "Показатели атмосферы стандартные", comp, true);
    }

    private void Process(EntityUid suitUid, EntityUid wearer, string message, SmartsuitComponent comp, bool unequip = false)
    {

        if (!comp.DeviceConnection || string.IsNullOrWhiteSpace(comp.HelmetPrototype))
            return;

        if (unequip)
        {
            HandleHelmetRemoval(suitUid, wearer, comp, message);
        }
        else
        {
            HandleHelmetEquipping(suitUid, wearer, comp, message);
        }
    }

    private void HandleHelmetRemoval(EntityUid suitUid, EntityUid wearer, SmartsuitComponent comp, string message)
    {
        if (TryComp<GasTankComponent>(suitUid, out var gasTank) && gasTank.IsConnected)
            RaiseLocalEvent(suitUid, new ToggleActionEvent { Performer = wearer });

        if (!_inventory.TryGetSlotEntity(wearer, "head", out var helmet))
            return;

        if (!TryComp<MetaDataComponent>(helmet, out var meta) ||
            string.IsNullOrWhiteSpace(meta.EntityPrototype?.ID) ||
            meta.EntityPrototype.ID != comp.HelmetPrototype || comp.ForceEquipped)
            return;

        RaiseLocalEvent(suitUid, new ToggleClothingEvent { Performer = suitUid });

        if (comp.Reporting)
            _chat.TrySendInGameICMessage(suitUid, message, InGameICChatType.Speak, true);
    }

    private void HandleHelmetEquipping(EntityUid suitUid, EntityUid wearer, SmartsuitComponent comp, string message)
    {
        if (TryComp<GasTankComponent>(suitUid, out var gasTank) && !gasTank.IsConnected)
            RaiseLocalEvent(suitUid, new ToggleActionEvent { Performer = wearer });

        if (_inventory.TryGetSlotEntity(wearer, "head", out var existingHelmet) &&
            TryComp<MetaDataComponent>(existingHelmet, out var meta) &&
            !string.IsNullOrWhiteSpace(meta.EntityPrototype?.ID) &&
            meta.EntityPrototype.ID == comp.HelmetPrototype)
        {
            return;
        }

        RaiseLocalEvent(suitUid, new ToggleClothingEvent { Performer = suitUid });

        comp.ForceEquipped = false;

        if (comp.Reporting)
            _chat.TrySendInGameICMessage(suitUid, message, InGameICChatType.Speak, true);
    }

    private void OnForceToggle(EntityUid suitUid, SmartsuitComponent comp, ToggleClothingEvent args)
    {
        if (args.Performer != suitUid)
        {
            comp.ForceEquipped = true;
            if (comp.Reporting)
                _chat.TrySendInGameICMessage(suitUid, "Переключён автономный режим шлема", InGameICChatType.Speak, true);
        }
    }

    private void OnGetVerbs(Entity<SmartsuitComponent> entity, ref GetVerbsEvent<Verb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var user = args.User;
        var component = entity.Comp;

        args.Verbs.Add(new InteractionVerb
        {
            Text = Loc.GetString("Переключить ИИ"),
            Priority = 1,
            Act = () =>
            {
                component.Activated = !component.Activated;
                _popupSystem.PopupEntity(
                    $"ИИ скафандра {(component.Activated ? "активирован" : "деактивирован")}",
                    entity,
                    user);
            }
        });

        args.Verbs.Add(new InteractionVerb
        {
            Text = Loc.GetString("Переключить синхронизацию"),
            Priority = 2,
            Act = () =>
            {
                component.DeviceConnection = !component.DeviceConnection;
                _popupSystem.PopupEntity(
                    $"Синхронизация с устройствами {(component.DeviceConnection ? "установлена" : "разорвана")}",
                    entity,
                    user);
            }
        });
    }
}
