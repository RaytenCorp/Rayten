using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Interaction;
using Content.Shared.Examine;
using Content.Shared.Damage;
using Robust.Shared.Timing;
using Robust.Shared.Physics;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Administration;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Utility;
using Robust.Shared.Maths;
using System.Linq;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed partial class BlockMovementOnEyeContactSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doaftersystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ScragEvent>(Scrag);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BlockMovementOnEyeContactComponent>();
        while (query.MoveNext(out var uid, out var blockComp))
        {
            if (CheckForObservers(uid))
            {
                EnsureComp<AdminFrozenComponent>(uid);
                CancelAllDoAfters(uid);
            }
            else
            {
                RemComp<AdminFrozenComponent>(uid);
            }
        }
    }

    private bool CheckForObservers(EntityUid cookie, float range = 16f)
    {
        foreach (var target in _lookup.GetEntitiesInRange<EyeClosingComponent>(Transform(cookie).Coordinates, range))
        {
            //полагаю печеньки могут видеть друг друга
            if (HasComp<BlockMovementOnEyeContactComponent>(target))
                continue;

            if (target.Comp.EyesClosed)
                continue;

            //мертв или в крите
            if (_mobStateSystem.IsIncapacitated(target))
                continue;

            //если таргет видит печеньку
            if (_examine.InRangeUnOccluded(target.Owner, cookie, range))
                return true;
        }
        return false;
    }
    public void Scrag(ScragEvent ev)
    {
        if (ev.Handled)
            return;

        var uid = ev.Performer;
        var target = ev.Target;

        if (!TryComp<BlockMovementOnEyeContactComponent>(uid, out var comp))
            return;

        //цель должна быть жива
        if (!_mobStateSystem.IsAlive(target))
            return;

        // нпс ваще пофиг через пол карты юзают даже во фризе поэтому вот такое
        if (HasComp<AdminFrozenComponent>(uid))
            return;

        if (!TryComp<TargetActionComponent>(ev.Action, out var targetaction))
            return;

        var distance = (Transform(uid).Coordinates.Position - Transform(target).Coordinates.Position).Length();
        if (distance > targetaction.Range)
            return;

        if (comp.Damage != null)
        {
            _damageable.TryChangeDamage(target, comp.Damage, origin: uid, ignoreResistances: true);

            if (comp.DamageSound != null)
                _audio.PlayPvs(comp.DamageSound, uid);
        }
        ev.Handled = true;
    }
    private void CancelAllDoAfters(EntityUid cookie, DoAfterComponent? comp = null)
    {
        if (!Resolve(cookie, ref comp, false))
            return;

        var ids = comp.DoAfters.Keys.ToList();

        foreach (var id in ids)
        {
            _doaftersystem.Cancel(cookie, id, comp, force: false);
        }
    }

}
