using Content.Shared.Vanilla.Archon.BlindPredator;
using Content.Shared.Animals.Components;
using Content.Shared.Movement.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Vanilla.Archon.WithManyVoices;

public abstract class SharedWithManyVoicesSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] private readonly SharedBlindPredatorSystem _predator = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!Timing.IsFirstTimePredicted)
            return;

        var query = EntityQueryEnumerator<WithManyVoicesComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var uidTrans))
        {
            if (Timing.CurTime < comp.NextCheckTime)
                continue;

            comp.NextCheckTime = Timing.CurTime + TimeSpan.FromSeconds(0.1f);

            var victimQuery = EntityQueryEnumerator<InputMoverComponent, PredatorVisibleMarkComponent, PhysicsComponent, TransformComponent>();
            while (victimQuery.MoveNext(out var targetUid, out var input, out var mark, out var physics, out var xform))
            {
                var visibleDistance = input.Sprinting ? comp.VisibleDistanceRun : comp.VisibleDistanceWalk;

                if (physics.LinearVelocity.Length() < 0.1f)
                    visibleDistance = comp.VisibleDistanceStand;

                if (!uidTrans.Coordinates.TryDistance(EntityManager, xform.Coordinates, out var distance))
                {
                    _predator.SetVisibility(targetUid, uid, false, mark);
                    continue;
                }

                _predator.SetVisibility(targetUid, uid, distance <= visibleDistance, mark);
            }
        }
    }
}
