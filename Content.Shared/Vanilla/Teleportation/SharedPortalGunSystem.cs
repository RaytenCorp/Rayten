using Content.Shared.Interaction.Events;
using Content.Shared.Vanilla.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Content.Shared.Trigger;
using Content.Shared.DoAfter;
using Robust.Shared.Timing;

namespace Content.Shared.Vanilla.Teleportation;

public sealed partial class SharedPortalGunSystem : EntitySystem
{
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private SharedDoAfterSystem _doafter = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private LinkedEntitySystem _link = default!;
    [Dependency] private SharedMapSystem _mapSystem = default!;
    [Dependency] private IGameTiming _timing = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SpawnCoordinatedPortalOnTriggerComponent, TriggerEvent>(OnTrigger);
        SubscribeLocalEvent<PortalGunComponent, ShotAttemptedEvent>(AttemptShoot);

        SubscribeLocalEvent<PortalGunComponent, PortalGunDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<PortalGunComponent, UseInHandEvent>(OnUseInHand);
    }

    private void AttemptShoot(EntityUid uid, PortalGunComponent component, ref ShotAttemptedEvent args)
    {
        args.Cancel();

        var curTime = _timing.CurTime;

        if (component.LastClick + TimeSpan.FromSeconds(component.FireRate) > curTime)
            return;

        component.LastClick = curTime;

        var ev = new PortalGunShootEvent(args.User);
        RaiseLocalEvent(uid, ev, true);
    }

    private void OnDoAfter(EntityUid uid, PortalGunComponent component, DoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (component.SavedCoordinates != null)
        {
            var lastMapUid = _mapSystem.GetMapOrInvalid(component.SavedCoordinates.Value.MapId);

            if (TryComp<PortalMapComponent>(lastMapUid, out var lastPortalMapComp))
                lastPortalMapComp.Enabled = true;
        }

        var coords = _transform.GetMapCoordinates(uid);

        component.SavedCoordinates = coords;
        _audio.PlayPvs(component.SaveCoordinatesSound, uid);

        var mapUid = _mapSystem.GetMapOrInvalid(coords.MapId);

        if (TryComp<PortalMapComponent>(mapUid, out var portalMapComp))
            portalMapComp.Enabled = false;

        args.Handled = true;
    }

    private void OnUseInHand(EntityUid uid, PortalGunComponent component, UseInHandEvent args)
    {
        if (args.Handled || !component.CanSaveCoordinates)
            return;

        var doafterArgs = new DoAfterArgs(EntityManager, args.User, 1f, new PortalGunDoAfterEvent(), uid, used: uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            MovementThreshold = 0.5f,
        };

        _doafter.TryStartDoAfter(doafterArgs);

        args.Handled = true;
    }

    private void OnTrigger(Entity<SpawnCoordinatedPortalOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        if (ent.Comp.Coordinates == null)
            return;

        var portal = Spawn(ent.Comp.PortalPrototype, _transform.GetMapCoordinates(ent));
        var exitPortal = Spawn(ent.Comp.PortalPrototype, ent.Comp.Coordinates.Value);

        _link.TryLink(portal, exitPortal, true);

        args.Handled = true;
    }
}
