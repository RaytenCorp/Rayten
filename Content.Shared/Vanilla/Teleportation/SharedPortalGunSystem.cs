using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Vanilla.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Trigger.Components.Effects;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Shared.Audio.Systems;
using Content.Shared.Trigger;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Robust.Shared.Timing;
using System.Numerics;

namespace Content.Shared.Vanilla.Teleportation;

public sealed class SharedPortalGunSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

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
            var lastMapUid = _mapManager.GetMapEntityId(component.SavedCoordinates.Value.MapId);

            if (TryComp<PortalMapComponent>(lastMapUid, out var lastPortalMapComp))
                lastPortalMapComp.Enabled = true;
        }

        var coords = _transform.GetMapCoordinates(uid);

        component.SavedCoordinates = coords;
        _audio.PlayPvs(component.SaveCoordinatesSound, uid);

        var mapUid = _mapManager.GetMapEntityId(coords.MapId);

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
