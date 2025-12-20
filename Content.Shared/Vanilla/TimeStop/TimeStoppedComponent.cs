using Content.Shared.Damage;

namespace Content.Shared.Vanilla.TimeStop;

[RegisterComponent]
public sealed partial class TimeStoppedComponent : Component
{

    [ViewVariables]
    public DamageSpecifier StoredDamage = new();

    [ViewVariables]
    public float StoredStaminaDamage;

    [ViewVariables]
    public int TimeStops = 0;
}
