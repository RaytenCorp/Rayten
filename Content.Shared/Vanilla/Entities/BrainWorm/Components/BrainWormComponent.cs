using Content.Shared.DoAfter;
using Content.Shared.Store;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Entities.BrainWorm;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BrainWormComponent : Component
{

    /// <summary>
    /// носитель мозгового червя
    /// </summary>
    [DataField]
    private EntityUid? _host;
    public bool TryGetHost(out EntityUid host)
    {
        if (_host.HasValue)
        {
            host = _host.Value;
            return true;
        }

        host = default;
        return false;
    }
    public void SetHost(EntityUid? host)
    {
        _host = host;
    }
    /// <summary>
    /// флаг неактивности червя (если хост сожрал сахар)
    /// </summary>
    [DataField]
    public bool IsSleep = false;
    [DataField]
    public bool IsMindController = false;

    public DoAfterId? EjectDoAfter;
    public DoAfterId? MindControlDoAfter;


    [DataField("evolutionPointsPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<CurrencyPrototype>))]
    public string EvolutionPointsPrototype = "EvolutionPoints";
    #region chemicals

    [Serializable, NetSerializable]
    public enum ChemicalsUiKey : byte
    {
        Key,
    }

    [DataField, AutoNetworkedField]
    public Dictionary<string, float> Reagents { get; set; } = new()
    {
        { "Epinephrine",   30f },
        { "Desoxyephedrine", 50f },
        { "Charcoal",       30f },
        { "Ethylredoxrazine",30f },
        { "Tricordrazine",  30f },
        { "Bruizine",       30f },
        { "Pyrazine",      30f },
        { "Necrosol",      50f }
    };

    [DataField, AutoNetworkedField]
    public float Chemicals = 0;

    [DataField, AutoNetworkedField]
    public float ChemicalsCup = 120f;

    [DataField]
    public float ChemicalsPerTick = 0.5f;

    [DataField(required: false)]
    public float ChemicalsTime = 1f;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextChemicalsTime = TimeSpan.Zero;
    public DamageSpecifier Heal = new()
    {
        DamageDict = new()
        {
            { "Slash", -0.4f },
            { "Piercing", -0.4f },
            { "Blunt", -0.4f },
            { "Heat", -0.4f },
            { "Shock", -0.4f },
            { "Cold", -0.4f },
            { "Caustic", -0.4f }
        }
    };

    #endregion

    #region actions
    public float InsertDoAfterTime = 5.0f;
    public float MindControlDoAfterTime = 30.0f;
    public bool HasReproduceUpgrade = false;
    public bool HasChemUpgrade = false;
    /// <summary>
    ///     Захват управления над телом
    /// </summary>
    [DataField("actionMindControl", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionMindControl = "ActionMindControl";
    public EntityUid? ActionMindControlEntity;

    /// <summary>
    ///     Захват мозга
    /// </summary>
    [DataField("actionInsertBrain", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionInsertBrain = "ActionInsertBrain";
    public EntityUid? ActionInsertBrainEntity;
    /// <summary>
    ///    Покидание мозга
    /// </summary>
    [DataField("actionEjectBrain", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionEjectBrain = "ActionEjectBrain";
    public EntityUid? ActionEjectBrainEntity;

    /// <summary>
    ///    Открытие меню эволюции
    /// </summary>
    public EntityUid? ActionWormShopEntity;
    /// <summary>
    ///    Открытие меню вводахимикатов
    /// </summary>
    public EntityUid? ActionWormChemicalsEntity;


    [Serializable, NetSerializable]
    public enum ForceSayUiKey : byte
    {
        Key,
    }
    #endregion
    [DataField]
    public BrainWormLifeStage Currentstage = BrainWormLifeStage.young;
    public int Reproducecount = 0;

    #region константы
    public const float EjectBrainTime = 30f;

    #endregion
}

[Serializable, NetSerializable]
public enum BrainWormLifeStage : byte
{
    young = 1,
    Mature = 2,
    Adult = 3,
    Elder = 4
}
