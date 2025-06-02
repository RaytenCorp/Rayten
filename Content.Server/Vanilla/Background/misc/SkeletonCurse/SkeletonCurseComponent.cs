using Content.Shared.FixedPoint;
using Content.Shared.Damage;
using Robust.Shared.Audio;

namespace Content.Server.Vanilla.Background.SkeletonCurse;

[RegisterComponent]
public sealed partial class SkeletonCurseComponent : Component
{
    [DataField("lifetimeDamage")]
    public Dictionary<EntityUid, FixedPoint2> LifetimeDamage = new();
    [DataField("damage")]
    public DamageSpecifier Damage = new()
    {
        DamageDict = new()
        {
            { "Blunt", 1200 }
        }
    };

}
