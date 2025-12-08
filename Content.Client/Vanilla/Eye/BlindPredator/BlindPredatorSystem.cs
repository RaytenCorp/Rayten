using Content.Shared.Vanilla.Eye.BlindPredator;
using Content.Shared.Movement.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Robust.Client.GameObjects;
using Robust.Client.Player;

namespace Content.Client.Vanilla.Eye.BlindPredator;

public sealed class BlindPredatorSystem : SharedBlindPredatorSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PredatorVisibleMarkComponent, AfterAutoHandleStateEvent>(OnHandleState);
    }
    protected override void UpdateVisibility(EntityUid uid, PredatorVisibleMarkComponent comp)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        var locEnt = _playerManager.LocalSession?.AttachedEntity;
        if (locEnt == null)
            return;

        if (comp.Predators.TryGetValue(locEnt.Value, out var val))
            sprite.Visible = val;
        else
            sprite.Visible = true;
    }
    private void OnHandleState(EntityUid uid, PredatorVisibleMarkComponent comp, ref AfterAutoHandleStateEvent args)
    {
        UpdateVisibility(uid, comp);
    }
}
