using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Content.Shared.Vanilla.Archon.OldMan.FemurBreaker;
namespace Content.Server.Vanilla.Archon.OldMan.FemurBreaker;

[RegisterComponent]
public sealed partial class OldManFoodComponent : Component
{
    [DataField]
    public SoundSpecifier FemurBreakSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/femur/femurbreaker-victim-scream.ogg",
        AudioParams.Default.WithVolume(15f).WithMaxDistance(20));
    [DataField]
    public SoundSpecifier EatSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/femur/106Bait.ogg");


    //байт
    public HashSet<EntityUid> BaitedOldMans = [];
    public bool OldMansBited = false;
    public TimeSpan OldManWillBiteAt = default;
    public TimeSpan WillKilledAt = default;
    public EntityUid? AudioStream = null;

    public DamageSpecifier EatenDamage = new()
    {
        DamageDict = new()
        {
            ["Cellular"] = 200
        }
    };
}

[RegisterComponent]
public sealed partial class FemurBreakerComponent : Component
{
    [DataField]
    public SoundSpecifier ActivateSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/femur/femurbreaker-activate.ogg",
        AudioParams.Default.WithVolume(-5f));

    [DataField]
    public TimeSpan ActivateTime = TimeSpan.FromSeconds(3);
    [DataField]
    public TimeSpan ActivateToFemurTime = TimeSpan.FromSeconds(1);
    [DataField]
    public TimeSpan FemurBreakTime = TimeSpan.FromSeconds(20);
    [DataField]
    public TimeSpan KillTime = TimeSpan.FromSeconds(14);
    [DataField]
    public DamageSpecifier FemurDamage = new()
    {
        DamageDict = new()
        {
            ["Blunt"] = 200
        }
    };
    public FemurBreakerState CurrentState = FemurBreakerState.Static;
    public TimeSpan SwitchStateAt = default;
    public TimeSpan NextActivateAt = TimeSpan.Zero;
    public TimeSpan? FemurTimeAt = null;

}
