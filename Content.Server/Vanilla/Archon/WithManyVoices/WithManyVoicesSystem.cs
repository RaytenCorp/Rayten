using Content.Server.Body.Components;
using Content.Shared.Vanilla.Archon.Research;
using Content.Shared.Vanilla.Archon.WithManyVoices;
using Content.Shared.Bed.Sleep;
using Content.Shared.Temperature;
using Content.Shared.Temperature.Components;
using Content.Shared.FixedPoint;

namespace Content.Server.Vanilla.Archon.BlindPredator;

public sealed class WithManyVoicesSystem : SharedWithManyVoicesSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WithManyVoicesComponent, OnTemperatureChangeEvent>(OnTempChange);
        SubscribeLocalEvent<WithManyVoicesComponent, ResearchAttemptEvent>(OnAttempt);
    }
    private void OnAttempt(EntityUid uid, WithManyVoicesComponent comp, ResearchAttemptEvent args)
    {
        if (!HasComp<SleepingComponent>(uid))
            args.Cancel();
    }

    private void OnTempChange(EntityUid uid, WithManyVoicesComponent comp, OnTemperatureChangeEvent args)
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
            sleep.WakeThreshold = FixedPoint2.New(5);
            sleep.CooldownEnd = Timing.CurTime + TimeSpan.FromMinutes(90);
        }
        else
        {
            RemComp<SleepingComponent>(uid);
        }
    }
}
