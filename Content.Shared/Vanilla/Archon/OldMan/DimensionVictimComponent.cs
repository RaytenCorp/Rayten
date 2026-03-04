using Content.Shared.FixedPoint;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Random;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Archon.OldMan;

[RegisterComponent, NetworkedComponent]
public sealed partial class DimensionVictimComponent : Component
{
    [DataField]
    public string TeleportPrototype = "PocketDimensionExitTeleport";
    [DataField]
    public string FakeTeleportPrototype = "PocketDimensionExitTeleportFake";
    [DataField]
    public ProtoId<WeightedRandomPrototype> DeadResults = "DimnsionVictimResults";
    [DataField]
    public int TeleportsAmount = 1;
    [DataField]
    public int FakeTeleportsAmount = 5;
    [DataField]
    public SoundSpecifier DimensionEscapeSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/106ExitPD.ogg");
    [DataField]
    public SoundSpecifier DimensionEnterSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/106EnterPD.ogg");
    [DataField]
    public SoundSpecifier DamageSound = new SoundCollectionSpecifier("106corrosion");
    [DataField]
    public SoundSpecifier BodyFallSound = new SoundCollectionSpecifier("BodyFall");
    [DataField]
    public SoundSpecifier DimensionAmbient = new SoundPathSpecifier("/Audio/Vanilla/Ambience/106/106dimension.ogg", AudioParams.Default.WithLoop(true));
    [DataField]
    public TimeSpan DamageInterval = TimeSpan.FromSeconds(10);
    [DataField]
    public TimeSpan NextDamage;
    [DataField]
    public DamageSpecifier Damage = new()
    {
        DamageDict = new()
        {
            ["Caustic"] = 5,
            ["Cellular"] = 0.1
        }
    };
    public HashSet<EntityUid> Portals = [];
    public EntityUid OldMan = default;
    public EntityUid StationGridUid = default;
    public EntityUid DimensionGridUid = default;
    public EntityUid? Stream = null;
    public bool ReturnableVictim = true;
}

[RegisterComponent]
public sealed partial class PDlushaComponent : Component
{
}
