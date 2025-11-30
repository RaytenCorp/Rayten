using Content.Shared.Damage;

using System.Numerics;

namespace Content.Shared.Vanilla.TimeStop;

[RegisterComponent]
public sealed partial class TimeStoppedComponent : Component
{

    [DataField]
    public bool Enabled = true;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier StoredDamage = new();

    [DataField]
    public float StoredStaminaDamage;

    [DataField]
    public int TimeStops;
}
