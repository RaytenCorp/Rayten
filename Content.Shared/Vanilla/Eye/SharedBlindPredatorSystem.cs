using Content.Shared.Vanilla.Eye.Components;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Vanilla.Eye;

public sealed class SharedBlindPredatorSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BlindPredatorComponent, ComponentStartup>(OnBlindStartup);
        SubscribeLocalEvent<BlindPredatorComponent, ComponentRemove>(OnBlindRemove);
    }

    private void OnBlindStartup(EntityUid uid, BlindPredatorComponent component, ComponentStartup args)
    {
        UpdateEntities();
    }

    private void OnBlindRemove(EntityUid uid, BlindPredatorComponent component, ComponentRemove args)
    {
        var query = EntityQueryEnumerator<PredatorVisibleMarkComponent>();
        while (query.MoveNext(out var entity, out _))
        {
            RemComp<PredatorVisibleMarkComponent>(entity);
        }
    }

    private void UpdateVisibility(EntityUid predatorUid, BlindPredatorComponent blindComp)
    {
        if (!blindComp.Enabled)
        {
            if (_gameTiming.CurTime >= blindComp.EnableTime && blindComp.DelayEnabled)
            {
                blindComp.Enabled = true;
                blindComp.DelayEnabled = false;
            }
            else
            {
                var query = EntityQueryEnumerator<PredatorVisibleMarkComponent>();
                while (query.MoveNext(out var entity, out _))
                {
                    RemComp<PredatorVisibleMarkComponent>(entity);
                }
                return;
            }
        }

        float runModifier = 1f;
        if (TryComp<InputMoverComponent>(predatorUid, out var predatorMover) && predatorMover.Sprinting)
        {
            runModifier = blindComp.UserRunModifier;
        }

        var predatorPos = _transform.GetWorldPosition(predatorUid);

        var moverQuery = EntityQueryEnumerator<InputMoverComponent, TransformComponent>();
        while (moverQuery.MoveNext(out var entity, out var victimMover, out var transform))
        {
            if (entity == predatorUid)
                continue;

            if (HasComp<BlindPredatorComponent>(entity))
                continue;

            float visibleDistance = blindComp.VisibleDistanceWalk;

            if (TryComp<PhysicsComponent>(entity, out var physics) && physics.LinearVelocity.Length() < 0.1f)
                visibleDistance = blindComp.VisibleDistanceStand;
            else if (victimMover.Sprinting)
                visibleDistance = blindComp.VisibleDistanceRun;
            else if (victimMover.Sprinting == false)
                visibleDistance = blindComp.VisibleDistanceWalk;

            visibleDistance *= runModifier;

            var distance = (transform.WorldPosition - predatorPos).Length();
            if (distance <= visibleDistance)
            {
                EnsureComp<PredatorVisibleMarkComponent>(entity);
            }
            else
            {
                RemComp<PredatorVisibleMarkComponent>(entity);
            }
        }
    }

    private void UpdateEntities()
    {
        var query = EntityQueryEnumerator<BlindPredatorComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            UpdateVisibility(uid, comp);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateEntities();
    }
}
