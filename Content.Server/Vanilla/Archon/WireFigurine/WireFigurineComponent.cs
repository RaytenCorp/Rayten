using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Vanilla.Archon.WireFigurine;
using Robust.Shared.Audio;

namespace Content.Server.Vanilla.Archon.WireFigurine;


[RegisterComponent]
public sealed partial class EatenMetalComponent : Component
{
}
//главарь фигурок
[RegisterComponent]
public sealed partial class WireFigurineMainComponent : Component
{
    [DataField]
    public int FigurineToNewStage = 16;
    [ViewVariables]
    public HashSet<EntityUid> FigurineCopies = [];

    [ViewVariables]
    public FigurineOrderType CurrentOrder = FigurineOrderType.Follow;
}


[RegisterComponent]
public sealed partial class WireFigurineComponent : Component
{
    [DataField]
    public SoundSpecifier SoundStructureDevour = new SoundPathSpecifier("/Audio/Machines/airlock_creaking.ogg", AudioParams.Default.WithVolume(-3f));

    [DataField]
    public float EatDoAfterTime = 10f;
    [DataField]
    public EntProtoId SpawnProto = "MobWireFigurineArchon068";

    [ViewVariables]
    public EntityUid Main = default;
    [ViewVariables]
    public EntityUid? EatenMetal = null;

    [ViewVariables]
    public int Stage = 1;

    [ViewVariables]
    public float DamageSum = 0;
    [ViewVariables]
    public float DamageToReproduce = 10f;
    [ViewVariables]
    public DamageSpecifier EatDamage = new()
    {
        DamageDict = new()
        {
            { "Structural", 10 }
        }
    };
    [ViewVariables]
    public float SpeedModifier = 1f;
    //база
    [DataField]
    public DamageSpecifier BaseEatDamage = new()
    {
        DamageDict = new()
        {
            { "Structural", 10 }
        }
    };
    [DataField]
    public float BaseDamageToReproduce = 10f;
}
