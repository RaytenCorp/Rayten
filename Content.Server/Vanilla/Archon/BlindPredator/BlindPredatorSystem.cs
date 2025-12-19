using Content.Server.Body.Components;
using Content.Shared.Vanilla.Archon.Research;
using Content.Shared.Vanilla.Archon.BlindPredator;
using Content.Shared.Bed.Sleep;
using Content.Shared.Temperature;
using Content.Shared.Temperature.Components;

namespace Content.Server.Vanilla.Archon.BlindPredator;

public sealed class BlindPredatorSystem : SharedBlindPredatorSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BlindPredatorComponent, OnTemperatureChangeEvent>(OnTempChange);
        SubscribeLocalEvent<BlindPredatorComponent, ResearchAttemptEvent>(OnAttempt);
    }
    private void OnAttempt(EntityUid uid, BlindPredatorComponent comp, ResearchAttemptEvent args)
    {
        if (HasComp<SleepingComponent>(uid))
            args.Cancel();
    }

    private void OnTempChange(EntityUid uid, BlindPredatorComponent comp, OnTemperatureChangeEvent args)
    {
        float idealTemp;

        if (!TryComp<TemperatureComponent>(uid, out var temperature))
            return;

        if (TryComp<ThermalRegulatorComponent>(uid, out var regulator) &&
            regulator.NormalBodyTemperature > temperature.ColdDamageThreshold &&
            regulator.NormalBodyTemperature < temperature.HeatDamageThreshold)
        {
            idealTemp = regulator.NormalBodyTemperature;
        }
        else
        {
            idealTemp = (temperature.ColdDamageThreshold + temperature.HeatDamageThreshold) / 2;
        }

        if (args.CurrentTemperature <= idealTemp - 30)
        {
            var sleep = EnsureComp<SleepingComponent>(uid);
            sleep.CooldownEnd = Timing.CurTime + TimeSpan.FromMinutes(90);
        }
        else
        {
            RemComp<SleepingComponent>(uid);
        }
    }

    protected override void UpdateVisibility(EntityUid uid, PredatorVisibleMarkComponent comp)
    {

    }
}
