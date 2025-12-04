using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.Network;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class AutoEyeClosingSystem : EntitySystem
{
    [Dependency] private readonly EyeClosingSystem _eyeClosingSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    public TimeSpan NextCheckTime;

    public override void Initialize()
    {
        base.Initialize();
        // UpdatesOutsidePrediction = true;
        SubscribeLocalEvent<AutoEyeClosingComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<AutoEyeClosingComponent, ComponentStartup>(OnComponentStartup);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var now = _timing.CurTime;
        // не забываем моргать парни
        var autoeyequery = EntityQueryEnumerator<AutoEyeClosingComponent, EyeClosingComponent>();
        while (autoeyequery.MoveNext(out var uid, out var comp, out var eye))
        {
            if (!_eyeClosingSystem.AreEyesClosed((uid, eye)))
            {
                //настало время закрывать глаза
                if (now >= comp.BlinkInTime)
                {
                    _eyeClosingSystem.SetEyelids(uid, true);
                    comp.BlinkOutTime = comp.BlinkInTime + comp.BlinkDuration;
                }
            }
            else
            {
                //настало время открывать глаза
                if (now >= comp.BlinkOutTime)
                {
                    _eyeClosingSystem.SetEyelids(uid, false);
                    comp.BlinkInTime = comp.BlinkOutTime + comp.BlinkInerval;
                }
            }
        }

        if (now < NextCheckTime)
            return;

        NextCheckTime = now + TimeSpan.FromSeconds(1);

        // выдаем автоклозинг тем кто рядом со статуей
        var query = EntityQueryEnumerator<EyeClosingComponent>();
        while (query.MoveNext(out var uid, out var eye))
        {
            if (_mobStateSystem.IsAlive(uid) && ObjectInRange(uid))
                EnsureComp<AutoEyeClosingComponent>(uid);
            else
                RemComp<AutoEyeClosingComponent>(uid);
        }
    }

    private void OnComponentShutdown(EntityUid uid, AutoEyeClosingComponent comp, ref ComponentShutdown args)
    {
        if (HasComp<EyeClosingComponent>(uid))
            _eyeClosingSystem.SetEyelids(uid, false);
    }

    private void OnComponentStartup(EntityUid uid, AutoEyeClosingComponent comp, ref ComponentStartup args)
    {
        var now = _timing.CurTime;
        comp.BlinkOutTime = now + comp.BlinkDuration + comp.BlinkInerval;
        comp.BlinkInTime = now + comp.BlinkInerval;
        Dirty(uid, comp);
    }
    /// <summary>
    /// Проверяет, есть ли в радиусе сущности с BlockMovementOnEyeContactComponent
    /// </summary>
    private bool ObjectInRange(EntityUid viewerUid, float range = 14f)
    {
        var target = _lookup.GetEntitiesInRange<BlockMovementOnEyeContactComponent>(Transform(viewerUid).Coordinates, range);

        return target.Count > 0;
    }
}
