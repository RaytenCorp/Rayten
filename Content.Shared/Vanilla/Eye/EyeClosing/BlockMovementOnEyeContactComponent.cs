using Content.Shared.Damage;
using Content.Shared.Actions;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;

namespace Content.Shared.Eye.Blinding.Components;

/// <summary>
///     Блокирует движение при зрительном контакте
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BlockMovementOnEyeContactComponent : Component
{

    [DataField(required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Damage = new();

    //яхз зачем это
    [DataField]
    public SoundSpecifier? Sound { get; set; } = new SoundCollectionSpecifier("Alarm173");

    [DataField]
    public SoundSpecifier? DamageSound { get; set; } = new SoundCollectionSpecifier("Snap173");
}

public sealed partial class ScragEvent : EntityTargetActionEvent
{
}
