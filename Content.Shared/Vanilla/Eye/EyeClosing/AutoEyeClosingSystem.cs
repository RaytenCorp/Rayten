using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class AutoEyeClosingSystem : EntitySystem
{
    [Dependency] private readonly EyeClosingSystem _eyeClosingSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    public TimeSpan NextCheckTime;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var curTime = _gameTiming.CurTime;

        // не забываем моргать парни
        var autoeyequery = EntityQueryEnumerator<AutoEyeClosingComponent, EyeClosingComponent>();
        while (autoeyequery.MoveNext(out var uid, out var autoeye, out var eye))
        {
            if (eye.EyesClosed)
            {
                if (curTime >= autoeye.OpenEyeTime)
                {
                    // Время открывать глазки
                    _eyeClosingSystem.SetEyelids(uid, false);

                    // Следующее закрытие через N секунд
                    autoeye.CloseEyeTime = curTime + TimeSpan.FromSeconds(autoeye.OpenDuration);
                }
            }
            else if (curTime >= autoeye.CloseEyeTime)
            {
                // Время закрывать глазки
                _eyeClosingSystem.SetEyelids(uid, true);

                // Следующее открытие через N секунд
                autoeye.OpenEyeTime = curTime + TimeSpan.FromSeconds(autoeye.CloseDuration);
            }
        }

        if (curTime < NextCheckTime)
            return;

        NextCheckTime = curTime + TimeSpan.FromSeconds(1);

        // выдаем автоклозинг тем кто рядом со статуей
        var query = EntityQueryEnumerator<EyeClosingComponent>();
        while (query.MoveNext(out var uid, out var eye))
        {
            if (ObjectInRange(uid) && _mobStateSystem.IsAlive(uid))
                EnsureComp<AutoEyeClosingComponent>(uid);
            else
                RemComp<AutoEyeClosingComponent>(uid);
        }
    }


    /// <summary>
    /// Проверяет, есть ли в радиусе сущности с BlockMovementOnEyeContactComponent
    /// </summary>
    private bool ObjectInRange(EntityUid viewerUid, float range = 16f)
    {
        var target = _lookup.GetEntitiesInRange<BlockMovementOnEyeContactComponent>(Transform(viewerUid).Coordinates, range);

        return target.Count > 0;
    }
}
