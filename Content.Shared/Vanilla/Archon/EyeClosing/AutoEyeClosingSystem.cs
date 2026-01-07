using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Timing;
using Robust.Shared.Random;
using Robust.Shared.Player;

namespace Content.Shared.Vanilla.Archon.EyeClosing;
/// <summary>
/// Закрытие и открытие глаз
/// Глаза открываются и закрывают детерминировано, чтобы можно было предсказать расписание следующего моргания
/// </summary>
public sealed class AutoEyeClosingSystem : EntitySystem
{
    [Dependency] private readonly EyeClosingSystem _eyeClosingSystem = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    public TimeSpan NextCheckTime;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AutoEyeClosingComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<AutoEyeClosingComponent, ComponentInit>(OnComponentInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var now = _timing.CurTime;
        var player = _playerManager.LocalSession?.AttachedEntity;
        var autoeyequery = EntityQueryEnumerator<AutoEyeClosingComponent, EyeClosingComponent>();
        while (autoeyequery.MoveNext(out var uid, out var comp, out var eye))
        {
            if (!_eyeClosingSystem.AreEyesClosed((uid, eye)))
            {
                //настало время закрывать глаза
                if (now >= comp.BlinkInTime)
                {
                    if (player != uid) _popup.PopupClient("моргнул", uid, player);
                    _eyeClosingSystem.SetEyelids(uid, true);
                    comp.BlinkOutTime = comp.BlinkInTime + comp.BlinkDuration;
                }
            }
            else
            {
                //настало время открывать глаза
                if (now >= comp.BlinkOutTime)
                {
                    _eyeClosingSystem.SetEyelids(uid, false);
                    comp.BlinkInTime = comp.BlinkOutTime + comp.BlinkInterval;
                }
            }
        }

        if (now < NextCheckTime)
            return;

        NextCheckTime = now + TimeSpan.FromSeconds(0.5f);

        // выдаем автоклозинг тем кто рядом со статуей
        var query = EntityQueryEnumerator<EyeClosingComponent>();
        while (query.MoveNext(out var uid, out var eye))
        {
            if (_mobStateSystem.IsAlive(uid) && ObjectInRange(uid))
                EnsureComp<AutoEyeClosingComponent>(uid);
            else
                RemComp<AutoEyeClosingComponent>(uid);
        }
    }

    private void OnComponentShutdown(EntityUid uid, AutoEyeClosingComponent comp, ref ComponentShutdown args)
    {
        if (HasComp<EyeClosingComponent>(uid))
            _eyeClosingSystem.SetEyelids(uid, false);
    }

    private void OnComponentInit(EntityUid uid, AutoEyeClosingComponent comp, ref ComponentInit args)
    {
        var firstBlinkTime = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(0.75f, 3f));

        comp.BlinkOutTime = firstBlinkTime + comp.BlinkDuration;
        comp.BlinkInTime = firstBlinkTime;
        Dirty(uid, comp);
    }

    /// <summary>
    /// Проверяет, есть ли в радиусе живые сущности с BlockMovementOnEyeContactComponent
    /// </summary>
    private bool ObjectInRange(EntityUid viewerUid, float range = 14f)
    {
        var targets = _lookup.GetEntitiesInRange<BlockMovementOnEyeContactComponent>(Transform(viewerUid).Coordinates, range);
        foreach (var target in targets)
            return _mobStateSystem.IsAlive(target);

        return false;
    }
}
