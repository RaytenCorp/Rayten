using Content.Shared.Chat;
using Content.Shared.Animals.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Random;

namespace Content.Shared.Vanilla.Eye.BlindPredator;
/*
--------------------туду-лист--------------------
1. Спокойное состояние через холод, пацифизм и запрет на вскрытие дверей при нем
2. Выкачака очков только в спокойном состоянии
3. ИИ

Статус: Готово? НЕТ
-------------------------------------------------
*/
public abstract class SharedBlindPredatorSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DisableBlindlessEvent>(OnDisableBlindAction);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BlindPredatorComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_gameTiming.CurTime < comp.NextCheckTime)
                continue;

            comp.NextCheckTime = _gameTiming.CurTime + TimeSpan.FromSeconds(0.15f);

            if (_gameTiming.CurTime < comp.EnableTime)
                return;

            foreach (var target in _lookup.GetEntitiesInRange<InputMoverComponent>(Transform(uid).Coordinates, 14f))
            {
                var targetUid = target.Owner;

                var mark = EnsureComp<PredatorVisibleMarkComponent>(targetUid);

                var visibleDistance = target.Comp.Sprinting ? comp.VisibleDistanceRun : comp.VisibleDistanceWalk;

                if (TryComp<PhysicsComponent>(targetUid, out var physics) && physics.LinearVelocity.Length() < 0.1f)
                    visibleDistance = comp.VisibleDistanceStand;

                if (_examine.InRangeUnOccluded(uid, targetUid, visibleDistance, ignoreInsideBlocker: false))
                    mark.Predators[uid] = true;
                else
                    mark.Predators[uid] = false;
                UpdateVisibility(targetUid, mark);
            }
        }
    }


    private void OnDisableBlindAction(DisableBlindlessEvent args)
    {
        if (args.Handled)
            return;

        var uid = args.Performer;

        if (!TryComp<BlindPredatorComponent>(uid, out var blindComp))
            return;

        if (TryComp<ParrotMemoryComponent>(uid, out var parrotComp) && parrotComp.SpeechMemories.Count > 0)
        {
            var memory = _random.Pick(parrotComp.SpeechMemories);
            _chat.TrySendInGameICMessage(uid, memory.Message, InGameICChatType.Speak, false, nameOverride: memory.Name, ignoreActionBlocker: true);
        }

        blindComp.EnableTime = _gameTiming.CurTime + args.DisableDelay;
        foreach (var target in _lookup.GetEntitiesInRange<InputMoverComponent>(Transform(uid).Coordinates, 14f))
        {
            var mark = EnsureComp<PredatorVisibleMarkComponent>(target.Owner);
            mark.Predators[uid] = true;
            UpdateVisibility(uid, mark);
        }
        args.Handled = true;
    }

    protected abstract void UpdateVisibility(EntityUid uid, PredatorVisibleMarkComponent comp);

}
