using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization;
using Robust.Shared.GameStates;
using Content.Shared.Damage;
using Robust.Shared.Utility;
using Robust.Shared.Audio;
using Robust.Shared.Map;

using Content.Shared.Actions;

namespace Content.Shared.Eye.Blinding.Components;

/// <summary>
///     Блокирует движение при зрительном контакте
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class BlockMovementOnEyeContactComponent : Component
{

    [DataField, AutoNetworkedField]
    public bool TeleportEnabled = false;

    [DataField, AutoNetworkedField]
    public EntityCoordinates? TeleportPos;

    [DataField(required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Damage = new();

    [DataField("teleportMaxRadius")]
    public float TeleportMaxRadius = 8f;

    [DataField, AutoNetworkedField]
    public SoundSpecifier? Sound { get; set; } = new SoundCollectionSpecifier("Alarm173");

    [DataField, AutoNetworkedField]
    public SoundSpecifier? DamageSound { get; set; } = new SoundCollectionSpecifier("Snap173");

}

public sealed partial class BlockMovementTeleportAttemptEvent : WorldTargetActionEvent
{

}
