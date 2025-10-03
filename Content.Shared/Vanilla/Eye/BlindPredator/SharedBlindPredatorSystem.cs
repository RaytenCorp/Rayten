using Content.Shared.Animals.Components;
using Content.Shared.Popups;
using Content.Shared.Movement.Components;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Shared.Vanilla.Eye.BlindPredator;

public abstract class SharedBlindPredatorSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DisableBlindlessEvent>(OnDisableAction);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BlindPredatorComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_gameTiming.CurTime < comp.NextCheckTime)
                continue;

            comp.NextCheckTime = _gameTiming.CurTime + TimeSpan.FromMilliseconds(250);

            UpdateVisibility(uid, comp);
        }
    }

    private void UpdateVisibility(EntityUid predatorUid, BlindPredatorComponent blindComp)
    {
        if (_gameTiming.CurTime < blindComp.EnableTime)
            return;

        var newlyVisible = new HashSet<EntityUid>();

        var runModifier = (TryComp<InputMoverComponent>(predatorUid, out var predatorMover) && predatorMover.Sprinting) ? 1f : blindComp.UserRunModifier;

        // Собираем всех, кого видим в данный тик в небольшом радиусе вокруг нас
        foreach (var target in _lookup.GetEntitiesInRange<InputMoverComponent>(Transform(predatorUid).Coordinates, blindComp.GetMaxRange() * runModifier))
        {
            var targetUid = target.Owner;

            if (HasComp<BlindPredatorComponent>(targetUid))
                continue;

            var visibleDistance = target.Comp.Sprinting ? blindComp.VisibleDistanceRun : blindComp.VisibleDistanceWalk;

            if (TryComp<PhysicsComponent>(targetUid, out var physics) && physics.LinearVelocity.Length() < 0.1f)
                visibleDistance = blindComp.VisibleDistanceStand;

            visibleDistance *= runModifier;

            if (_examine.InRangeUnOccluded(predatorUid, targetUid, visibleDistance, ignoreInsideBlocker: false))
                newlyVisible.Add(targetUid);
        }

        // Скрыть тех, кто был видим, но больше не видим
        foreach (var old in blindComp.VisibleEnts)
        {
            if (!newlyVisible.Contains(old))
                ChangeVictimVisablity(old, false);
        }

        // Показать тех, кто стал новым видимым
        foreach (var cur in newlyVisible)
        {
            if (!blindComp.VisibleEnts.Contains(cur))
                ChangeVictimVisablity(cur, true);
        }

        blindComp.VisibleEnts = newlyVisible;
    }

    private void OnDisableAction(DisableBlindlessEvent args)
    {
        if (args.Handled)
            return;

        var uid = args.Performer;

        if (!TryComp<BlindPredatorComponent>(uid, out var blindComp))
            return;

        if (TryComp<ParrotMemoryComponent>(uid, out var parrotComp) &&
            parrotComp.SpeechMemories.Count > 0)
        {
            var memory = _random.Pick(parrotComp.SpeechMemories);

            Say(uid, memory.Message, memory.Name);

            blindComp.EnableTime = _gameTiming.CurTime + args.DisableDelay;

            foreach (var target in _lookup.GetEntitiesInRange<InputMoverComponent>(Transform(uid).Coordinates, blindComp.GetMaxRange()))
                ChangeVictimVisablity(target, true);

            _audio.PlayPvs(args.Sound, uid);

            args.Handled = true;
        }
        else
            _popup.PopupClient("Вы ещё не скопировали ничей голос", uid, uid, PopupType.SmallCaution);
    }

    protected abstract void Say(EntityUid uid, string msg, string? name);
    protected abstract void ChangeVictimVisablity(EntityUid target, bool visible);
}
