using Content.Server.Temperature.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Temperature.Systems;
using Content.Server.Light.Components;
using Content.Server.Atmos;

using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Vanilla.Warmer;
using Content.Shared.Inventory;
using Content.Shared.Atmos;
using Content.Shared.Item;

using Robust.Server.GameObjects;
using Robust.Shared.Timing;
using Robust.Shared.Map;

namespace Content.Server.Vanilla.Warmer;

public sealed class WarmerSystem : EntitySystem
{
    [Dependency] private readonly TemperatureSystem _temperature = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<WarmerComponent, TransformComponent>();
        var curTime = _gameTiming.CurTime;

        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            if (curTime < comp.NextUpdate)
                continue;

            if (!IsActive(uid))
                continue;

            comp.NextUpdate = curTime + comp.HeatSpeed;

            HeatTile(uid, comp, xform);
            HeatBody(uid, comp);
            ExhaustGas(uid, comp);
        }
    }

    private bool IsActive(EntityUid uid)
    {
        if (TryComp<ItemToggleComponent>(uid, out var toggle) && !toggle.Activated)
            return false;

        if (TryComp<ExpendableLightComponent>(uid, out var light) && !light.Activated)
            return false;

        return true;
    }

    private void HeatTile(EntityUid uid, WarmerComponent comp, TransformComponent xform)
    {
        var coordinates = xform.Coordinates;
        var tileHeat = comp.TileHeatStrength;
        var maxTemp = comp.HeatMaxTemp;

        var environment = _atmosphere.GetContainingMixture(uid, false, true);

        if (environment == null)
            return;

        if (environment.Temperature < maxTemp)
        {
            var heatEnergy = tileHeat * (float)comp.HeatSpeed.TotalSeconds;
            environment.Temperature = Math.Min(environment.Temperature + heatEnergy, maxTemp);
        }
    }

    private void HeatBody(EntityUid uid, WarmerComponent comp)
    {
        if (comp.OnlyWeared)
        {
            if (!_inventory.TryGetContainingSlot(uid, out var slotDef))
            {
                return;
            }
        }

        if (!TryComp<TransformComponent>(uid, out var xform))
            return;

        var parent = xform.ParentUid;

        if (!HasComp<InventoryComponent>(parent) || !TryComp<TemperatureComponent>(parent, out var temp))
            return;

        _temperature.ChangeHeat(parent, comp.BodyHeatStrength, false, temp);
    }

    private void ExhaustGas(EntityUid uid, WarmerComponent component)
    {
        var exhaustMixture = new GasMixture();
        exhaustMixture.SetMoles(component.GasType, component.MoleRatio);
        exhaustMixture.Temperature = component.HeatMaxTemp;

        var environment = _atmosphere.GetContainingMixture(uid, false, true);
        if (environment != null)
            _atmosphere.Merge(environment, exhaustMixture);
    }
}
