using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Interaction;
using Content.Shared.Examine;
using Content.Shared.Damage;
using Robust.Shared.Timing;
using Robust.Shared.Physics;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Utility;
using Robust.Shared.Maths;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed partial class BlockMovementOnEyeContactSystem : EntitySystem
{
    [Dependency] private readonly SharedInteractionSystem _interaction = default!;
    [Dependency] private readonly ExamineSystemShared _examineSystem = default!;
    [Dependency] private readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;

    private readonly HashSet<EntityUid> _entSet = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BlockMovementTeleportAttemptEvent>(OnTeleportAttempt);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BlockMovementOnEyeContactComponent, InputMoverComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var blockComp, out var input, out var transform))
        {
            var hasObserver = CheckForObservers(uid, transform);

            var shouldBlock = hasObserver;

            input.CanMove = !shouldBlock;

            if (blockComp.TeleportEnabled && blockComp.TeleportPos.HasValue && shouldBlock == false)
            {
                _trans.SetCoordinates(uid, blockComp.TeleportPos.Value);
                _trans.AttachToGridOrMap(uid);

                Snap(uid, transform, blockComp);

                blockComp.TeleportEnabled = false;
                blockComp.TeleportPos = null;
            }
        }
    }

    private bool CheckForObservers(EntityUid targetUid, TransformComponent targetTransform)
    {
        var targetPos = _trans.GetWorldPosition(targetTransform);

        var eyeQuery = EntityQueryEnumerator<EyeClosingComponent, TransformComponent>();
        while (eyeQuery.MoveNext(out var observerUid, out var eyeComp, out var observerTransform))
        {
            if (observerUid == targetUid)
                continue;

            var observerPos = _trans.GetWorldPosition(observerTransform);
            var distance = (observerPos - targetPos).Length();

            if (distance < 16f && _examineSystem.CanExamine(observerUid, targetUid))
            {
                if (!eyeComp.EyesClosed)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Snap(EntityUid uid, TransformComponent transform, BlockMovementOnEyeContactComponent comp)
    {
        var position = _trans.GetMapCoordinates(uid, transform);

        _entSet.Clear();
        _lookup.GetEntitiesInRange(position.MapId, position.Position, 4f, _entSet,
            flags: LookupFlags.Dynamic | LookupFlags.Sundries);

        foreach (var entity in _entSet)
        {
            if (entity == uid)
                continue;

            if (!HasComp<MobMoverComponent>(entity))
                continue;

            if (comp.Sound != null)
            {
                var resolvedSound = _audio.ResolveSound(comp.Sound);
                _audio.PlayEntity(resolvedSound, entity, entity);
            }
        }

        _entSet.Clear();
        _lookup.GetEntitiesInRange(position.MapId, position.Position, 2f, _entSet,
            flags: LookupFlags.Dynamic | LookupFlags.Sundries);

        foreach (var entity in _entSet)
        {
            if (!_mobStateSystem.IsAlive(entity))
                continue;

            if (entity == uid)
                continue;

            if (!HasComp<MobMoverComponent>(entity) || !HasComp<DamageableComponent>(entity))
                continue;

            if (comp.Damage != null)
            {
                _damageable.TryChangeDamage(entity, comp.Damage, origin: uid);

                if (comp.DamageSound != null)
                    _audio.PlayPvs(comp.DamageSound, uid);
            }
        }

        _entSet.Clear();
    }

    private void OnTeleportAttempt(BlockMovementTeleportAttemptEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<BlockMovementOnEyeContactComponent>(args.Performer, out var comp))
            return;

        var transform = Transform(args.Performer);

        if (transform.MapID != _trans.GetMapId(args.Target))
            return;

        var performerPos = _trans.GetWorldPosition(transform);
        var targetPos = _trans.ToMapCoordinates(args.Target).Position;
        var distance = (targetPos - performerPos).Length();

        if (distance > comp.TeleportMaxRadius)
        {
            var direction = (targetPos - performerPos).Normalized();
            var limitedPos = performerPos + direction * comp.TeleportMaxRadius;

            comp.TeleportPos = new EntityCoordinates(_mapManager.GetMapEntityId(transform.MapID), limitedPos);
        }
        else
        {
            comp.TeleportPos = args.Target;
        }

        comp.TeleportEnabled = true;
        args.Handled = true;
    }
}
