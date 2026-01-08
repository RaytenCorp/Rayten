using Robust.Shared.Serialization;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Weapons.Ranged;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class ChargedWeaponComponent : Component
{

    [DataField, AutoNetworkedField]
    public TimeSpan ChargeDuration = TimeSpan.FromSeconds(7);

    [DataField, AutoNetworkedField]
    public float MaxRange = 5f;

    [DataField, AutoNetworkedField]
    public float FireRate = 4f;

    [DataField, AutoNetworkedField]
    public float EnergyPerShoot = 1500f;

    [DataField, AutoNetworkedField]
    public float StaminaDamagePerShoot = 600f;

    [DataField]
    public SoundSpecifier DownSound = new SoundPathSpecifier("/Audio/Vanilla/Weapons/Guns/Gunshots/SpinDown.ogg")
    {
        Params = AudioParams.Default.WithVolume(5f)
    };

    [DataField]
    public SoundSpecifier UpSound = new SoundPathSpecifier("/Audio/Vanilla/Weapons/Guns/Gunshots/SpinUp.ogg")
    {
        Params = AudioParams.Default.WithVolume(5f)
    };

    [ViewVariables, AutoNetworkedField]
    public bool IsShooting = false;

    [ViewVariables, AutoNetworkedField, AutoPausedField]
    public TimeSpan ShootingStopAt = TimeSpan.Zero;

    [ViewVariables, AutoNetworkedField, AutoPausedField]
    public TimeSpan NextShootAt = TimeSpan.Zero;

    public EntityUid? Stream;
}

[Serializable, NetSerializable]
public sealed class WeaponChargeShootRequestEvent : EntityEventArgs
{
    public readonly NetEntity Weapon;
    public readonly NetEntity? Target;

    public WeaponChargeShootRequestEvent(NetEntity weapon, NetEntity? target)
    {
        Weapon = weapon;
        Target = target;
    }
}

[Serializable, NetSerializable]
public sealed class WeaponChargeEvent : EntityEventArgs
{
    public readonly NetEntity Weapon;
    public readonly bool StartCharge;

    public WeaponChargeEvent(NetEntity weapon, bool start)
    {
        Weapon = weapon;
        StartCharge = start;
    }
}
