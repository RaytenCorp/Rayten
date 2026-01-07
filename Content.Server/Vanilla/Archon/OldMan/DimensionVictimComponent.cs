using Content.Shared.FixedPoint;
using Content.Shared.Damage;

using Robust.Shared.Audio;
using System.Reflection.Metadata;

namespace Content.Server.Vanilla.Archon.OldMan;

[RegisterComponent]
public sealed partial class DimensionVictimComponent : Component
{
    /// <summary>
    /// порталы заспавненные на эту жертву
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public HashSet<EntityUid> Portals = [];
    /// <summary>
    /// Грид карманного измерения
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public EntityUid DimensionGridUid = default;
    /// <summary>
    /// дедус
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly)]
    public Entity<OldManComponent> OldMan = default;

    [DataField]
    public string TeleportPrototype = "PocketDimensionExitTeleport";

    [DataField]
    public string FakeTeleportPrototype = "PocketDimensionExitTeleportFake";

    /// <summary>
    /// такое количество телепортов заспавнится на одну жертву
    /// </summary>
    [DataField]
    public int TeleportsAmount = 1;
    /// <summary>
    /// такое количество фейковых телепортов заспавнится на одну жертву
    /// </summary>
    [DataField]
    public int FakeTeleportsAmount = 6;

    [DataField]
    public SoundSpecifier DimensionEscapeSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/106laugh1.ogg");

    [DataField]
    public SoundSpecifier DamageSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/106/106decay.ogg");

    [DataField]
    public SoundSpecifier DimensionAmbient = new SoundPathSpecifier("/Audio/Vanilla/Ambience/106/106dimension.ogg");


    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan DamageInterval = TimeSpan.FromSeconds(40);

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextDamage;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Damage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            ["Cellular"] = 5,
            ["Caustic"] = 5
        }
    };

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier FinalDamage = new()
    {
        DamageDict = new Dictionary<string, FixedPoint2>
        {
            ["Cellular"] = 30,
            ["Caustic"] = 80
        }
    };
}
