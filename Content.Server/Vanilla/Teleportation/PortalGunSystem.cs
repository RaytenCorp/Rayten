using Content.Server.Projectiles;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Projectiles;
using Content.Shared.Teleportation.Components;
using Content.Shared.Teleportation.Systems;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.Map;
using Content.Shared.Trigger;
using Content.Shared.Popups;
using Robust.Server.Audio;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;

namespace Content.Server.Teleportation;

public sealed class PortalGunSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly SharedPopupSystem _sharedPopupSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;
    [Dependency] private readonly ProjectileSystem _projectile = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SpawnCoordinatedPortalOnTriggerComponent, TriggerEvent>(OnTrigger);
        SubscribeLocalEvent<PortalGunComponent, OnEmptyGunShotEvent>(OnEmptyGunShot);

        SubscribeLocalEvent<PortalGunComponent, PortalGunDoAfterEvent>(OnDoAfter);
        SubscribeLocalEvent<PortalGunComponent, UseInHandEvent>(OnUseInHand);
    }

    private void OnEmptyGunShot(EntityUid uid, PortalGunComponent component, ref OnEmptyGunShotEvent args)
    {
        if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var solution, out var solutionComp))
            return;

        if (!TryComp<BatteryWeaponFireModesComponent>(uid, out var fireModes) ||
            fireModes.FireModes.Count == 0)
            return;

        var currentMode = fireModes.FireModes[fireModes.CurrentFireMode];

        if (currentMode.Prototype == component.CoordinatedPortalProjectile &&
            component.SavedCoordinates == null)
            return;

        var amountToRemove = FixedPoint2.New(currentMode.FireCost);

        if (solutionComp.GetTotalPrototypeQuantity(component.ReagentName) < amountToRemove ||
            _solutionSystem.RemoveReagent(solution.Value, component.ReagentName, amountToRemove) <= FixedPoint2.Zero)
        {
            return;
        }
        
        var projectile = Spawn(currentMode.Prototype, _transform.GetMapCoordinates(uid));

        if (TryComp<SpawnCoordinatedPortalOnTriggerComponent>(projectile, out var cordPortalComp) && component.SavedCoordinates != null)
            cordPortalComp.Coordinates = component.SavedCoordinates.Value;

        if (TryComp<ProjectileComponent>(projectile, out var projectileComp))
            projectileComp.Shooter = args.User;

        if (TryComp<PhysicsComponent>(projectile, out var physics))
        {
            var direction = _transform.GetWorldRotation(args.User).ToWorldVec();
            _physics.SetLinearVelocity(projectile, direction * component.ProjectileVelocity, body: physics);
        }
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

        _sharedPopupSystem.PopupClient("Координаты сохранены", uid, args.User);

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
