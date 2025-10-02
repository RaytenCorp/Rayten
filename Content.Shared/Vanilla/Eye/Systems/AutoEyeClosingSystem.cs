using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class AutoEyeClosingSystem : EntitySystem
{
    [Dependency] private readonly EyeClosingSystem _eyeClosingSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    private const float BlinkInterval = 7f;
    private const float BlinkDuration = 0.4f;

    public TimeSpan nextBlinkTime;
    private readonly Dictionary<EntityUid, TimeSpan> blinkEndTimes = new();

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _gameTiming.CurTime;

        var entitiesToRemove = new List<EntityUid>();
        foreach (var (uid, endTime) in blinkEndTimes)
        {
            if (!_mobStateSystem.IsAlive(uid))
                return;

            if (curTime >= endTime && TryComp<EyeClosingComponent>(uid, out var eyeComp))
            {
                _eyeClosingSystem.SetEyelids(uid, false);
                entitiesToRemove.Add(uid);
            }
        }
        foreach (var uid in entitiesToRemove)
        {
            blinkEndTimes.Remove(uid);
        }

        if (curTime < nextBlinkTime)
            return;

        var query = EntityQueryEnumerator<AutoEyeClosingComponent, EyeClosingComponent>();
        while (query.MoveNext(out var uid, out var comp, out var eyeComp))
        {
            if (!ObjectInSight(uid))
                continue;

            _eyeClosingSystem.SetEyelids(uid, true);

            blinkEndTimes[uid] = curTime + TimeSpan.FromSeconds(BlinkDuration);
        }

        nextBlinkTime = curTime + TimeSpan.FromSeconds(BlinkInterval);
    }

    /// <summary>
    /// Проверяет, есть ли в поле зрения сущности с BlockMovementOnEyeContactComponent
    /// </summary>
    private bool ObjectInSight(EntityUid viewerUid)
    {
        var objectQuery = EntityQueryEnumerator<BlockMovementOnEyeContactComponent, TransformComponent>();

        while (objectQuery.MoveNext(out var objectUid, out var _, out var objectform))
        {
            var objectPos = _trans.GetWorldPosition(objectUid);
            var viewerPos = _trans.GetWorldPosition(viewerUid);
            var distance = (viewerPos - objectPos).Length();

            if (distance < 16f)
            {
                return true;
            }
        }

        return false;

    }
}
