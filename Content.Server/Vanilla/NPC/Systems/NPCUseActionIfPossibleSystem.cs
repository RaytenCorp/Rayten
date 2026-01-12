using Content.Server.Vanilla.NPC.Components;
using Content.Server.NPC.HTN;
using Content.Shared.Actions;
using Robust.Shared.Timing;

namespace Content.Server.Vanilla.NPC.Systems;

public sealed class NPCUseActionIfPossibleSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private TimeSpan _nextUpdate = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NPCUseActionIfPossibleComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<NPCUseActionIfPossibleComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.ActionEnt = _actions.AddAction(ent, ent.Comp.ActionId);
    }

    public bool TryUseAction(Entity<NPCUseActionIfPossibleComponent?> user)
    {
        if (!Resolve(user, ref user.Comp, false))
            return false;

        if (_actions.GetAction(user.Comp.ActionEnt) is not {} action)
            return false;

        if (!_actions.ValidAction(action))
            return false;

        _actions.PerformAction(user.Owner, action, predicted: false);
        return true;
    }

    public bool HaveTarget(EntityUid uid, NPCUseActionIfPossibleComponent comp)
    {
        if (comp.TargetKey == null)
            return false;

        if (!TryComp<HTNComponent>(uid, out var htn))
            return false;

        if (!htn.Blackboard.TryGetValue<EntityUid>(comp.TargetKey, out var target, EntityManager))
            return false;

        return true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;

        if (curTime < _nextUpdate)
            return;

        _nextUpdate = curTime + TimeSpan.FromSeconds(1);

        var query = EntityQueryEnumerator<NPCUseActionIfPossibleComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {

            if (comp.TargetKey != null && !HaveTarget(uid, comp))
                return;

            TryUseAction((uid, comp));
        }
    }
}
