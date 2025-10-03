using Content.Server.Projectiles;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction.Events;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Vanilla.Teleportation.Components;
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
using Content.Server.Chat.Managers;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Content.Shared.Trigger;
using Robust.Server.Audio;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Content.Server.Administration;
using System.Numerics;

namespace Content.Server.Vanilla.Teleportation;

public sealed class PortalGunSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solutionSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly QuickDialogSystem _quickDialog = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;
    [Dependency] private readonly ProjectileSystem _projectile = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly LinkedEntitySystem _link = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly GunSystem _gunSystem = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PortalGunComponent, PortalGunShootEvent>(AttemptShoot);

        SubscribeLocalEvent<PortalGunComponent, GetVerbsEvent<ActivationVerb>>(AddVerb);
    }

    private void AttemptShoot(EntityUid uid, PortalGunComponent component, ref PortalGunShootEvent args)
    {

        if (!_solutionSystem.TryGetSolution(uid, component.SolutionName, out var solution, out var solutionComp))
            return;

        if (!TryComp<BatteryWeaponFireModesComponent>(uid, out var fireModes) ||
            fireModes.FireModes.Count == 0)
            return;

        var currentMode = fireModes.FireModes[fireModes.CurrentFireMode];

        if (currentMode.Prototype == component.CoordinatedPortalProjectile &&
            component.SavedCoordinates == null)
        {
            _audio.PlayPvs(component.EmptyShotSound, uid);
            return;
        }

        var amountToRemove = FixedPoint2.New(currentMode.FireCost);

        if (solutionComp.GetTotalPrototypeQuantity(component.ReagentName) < amountToRemove ||
            _solutionSystem.RemoveReagent(solution.Value, component.ReagentName, amountToRemove) <= FixedPoint2.Zero)
        {
            _audio.PlayPvs(component.EmptyShotSound, uid);
            return;
        }
        
        var projectile = Spawn(currentMode.Prototype, _transform.GetMapCoordinates(uid));
        _audio.PlayPvs(component.ShotSound, uid);

        if (TryComp<SpawnCoordinatedPortalOnTriggerComponent>(projectile, out var cordPortalComp) && component.SavedCoordinates != null)
            cordPortalComp.Coordinates = component.SavedCoordinates.Value;

        if (TryComp<ProjectileComponent>(projectile, out var projectileComp))
            projectileComp.Shooter = args.User;

        if (TryComp<PhysicsComponent>(projectile, out var physics) && TryComp<GunComponent>(uid, out var gun))
        {
            var direction = _transform.GetWorldRotation(args.User).ToWorldVec();
            _physics.SetLinearVelocity(projectile, direction * gun.ProjectileSpeed, body: physics);
        }
    }

    private void AddVerb(EntityUid uid, PortalGunComponent comp, GetVerbsEvent<ActivationVerb> args)
    {
        if (!TryComp(args.User, out ActorComponent? actor))
            return;

        if (!comp.CanTypeCoordinates)
            return;

        if (!args.CanInteract)
            return;

        int x = 0;
        int y = 0;

        var verb = new ActivationVerb
        {
            Text = "Ввести координаты",
            Act = () =>
            {
                _quickDialog.OpenDialog(actor.PlayerSession, "Ввести координаты", "Введите Y координату", (string message) =>
                {
                    if (!int.TryParse(message, out var yMes))
                        return;

                    if (yMes > 1000)
                        yMes = 1000;

                    if (yMes < -1000)
                        yMes = -1000;

                    y = yMes;
                    _audio.PlayPvs(comp.SaveCoordinatesSound, uid);

                    if (comp.SavedCoordinates == null)
                        x = 0;
                    else
                        x = (int)comp.SavedCoordinates.Value.Position.X;

                    comp.SavedCoordinates = new MapCoordinates(new Vector2(x, y), _transform.GetMapCoordinates(uid).MapId);
                });

                _quickDialog.OpenDialog(actor.PlayerSession, "Ввести координаты", "Введите X координату", (string message) =>
                {
                    if (!int.TryParse(message, out var xMes))
                        return;

                    if (xMes > 1000)
                        xMes = 1000;

                    if (xMes < -1000)
                        xMes = -1000;

                    x = xMes;
                    _audio.PlayPvs(comp.SaveCoordinatesSound, uid);

                    if (comp.SavedCoordinates == null)
                        y = 0;
                    else
                        y = (int)comp.SavedCoordinates.Value.Position.Y;

                    comp.SavedCoordinates = new MapCoordinates(new Vector2(x, y), _transform.GetMapCoordinates(uid).MapId);
                });
            },
        };

        args.Verbs.Add(verb);
    }
}
