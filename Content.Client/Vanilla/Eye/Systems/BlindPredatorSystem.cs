using Content.Shared.Vanilla.Eye.BlindPredator;
using Content.Shared.Movement.Components;
using Robust.Shared.Player;
using Robust.Client.GameObjects;
using Robust.Client.Player;

namespace Content.Client.Vanilla.Eye.Systems;

public sealed class BlindPredatorSystem : SharedBlindPredatorSystem
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BlindPredatorComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<BlindPredatorComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
    }

    //Делаем всех снова видимыми если игрок гостанулся
    private void OnPlayerDetached(EntityUid uid, BlindPredatorComponent component, LocalPlayerDetachedEvent args)
    {
        foreach (var target in component.VisibleEnts)
        {
            if (!TryComp<SpriteComponent>(target, out var sprite))
                continue;

            sprite.Visible = true;
        }
    }
    //Делаем спрайты всех мобов невидимыми
    private void OnPlayerAttached(EntityUid uid, BlindPredatorComponent component, LocalPlayerAttachedEvent args)
    {
        var moverQuery = EntityQueryEnumerator<InputMoverComponent>();
        while (moverQuery.MoveNext(out var target, out _))
        {
            if (HasComp<BlindPredatorComponent>(target))
                continue;

            ChangeVictimVisablity(target, false);
        }
    }

    protected override void ChangeVictimVisablity(EntityUid target, bool visible)
    {
        if (!HasComp<BlindPredatorComponent>(_playerManager.LocalSession?.AttachedEntity))
            return;

        if (!TryComp<SpriteComponent>(target, out var sprite))
            return;

        sprite.Visible = visible;
    }

    //никакой реализации, т.к. на клиенте недоступен чат
    protected override void Say(EntityUid uid, string msg, string? name)
    {

    }
}
