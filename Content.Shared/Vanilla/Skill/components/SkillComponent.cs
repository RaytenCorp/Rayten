using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Skill;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SkillComponent : Component
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
//очки навыков
    [DataField("SkillPoints"), AutoNetworkedField]
    public int SkillPoints { get; set; } = 0;

#region НАВЫКИ

// Стрельба
    [DataField("RangeWeaponLevel")]
    private SkillLevel _rangeWeaponLevel = SkillLevel.None;
    private int _rangeWeaponExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel RangeWeaponLevel
    {
        get => _rangeWeaponLevel;
        set => SetSkill(ref _rangeWeaponLevel, value, skillType.RangeWeapon);
    }

    [AutoNetworkedField]
    public int RangeWeaponExp
    {
        get => _rangeWeaponExp;
        set => SetSkill(ref _rangeWeaponExp, value, skillType.RangeWeapon);
    }

//Ближний бой
    [DataField("MeleeWeaponLevel")]
    private SkillLevel _meleeWeaponLevel = SkillLevel.None;
    private int _meleeWeaponExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel MeleeWeaponLevel
    {
        get => _meleeWeaponLevel;
        set => SetSkill(ref _meleeWeaponLevel, value, skillType.MeleeWeapon);
    }

    [AutoNetworkedField]
    public int MeleeWeaponExp
    {
        get => _meleeWeaponExp;
        set => SetSkill(ref _meleeWeaponExp, value, skillType.MeleeWeapon);
    }

//Медицина
    [DataField("MedicineLevel")]
    private SkillLevel _medicineLevel = SkillLevel.None;
    private int _medicineExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel MedicineLevel
    {
        get => _medicineLevel;
        set => SetSkill(ref _medicineLevel, value, skillType.Medicine);
    }

    [AutoNetworkedField]
    public int MedicineExp
    {
        get => _medicineExp;
        set => SetSkill(ref _medicineExp, value, skillType.Medicine);
    }

//Химия
    [DataField("ChemistryLevel")]
    private SkillLevel _chemistryLevel = SkillLevel.None;
    private int _chemistryExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel ChemistryLevel
    {
        get => _chemistryLevel;
        set => SetSkill(ref _chemistryLevel, value, skillType.Chemistry);
    }

    [AutoNetworkedField]
    public int ChemistryExp
    {
        get => _chemistryExp;
        set => SetSkill(ref _chemistryExp, value, skillType.Chemistry);
    }

//Инженерия
    [DataField("EngineeringLevel")]
    private SkillLevel _engineeringLevel = SkillLevel.None;
    private int _engineeringExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel EngineeringLevel
    {
        get => _engineeringLevel;
        set => SetSkill(ref _engineeringLevel, value, skillType.Engineering);
    }

    [AutoNetworkedField]
    public int EngineeringExp
    {
        get => _engineeringExp;
        set => SetSkill(ref _chemistryExp, value, skillType.Engineering);
    }
//Строительство
    [DataField("BuildingLevel")]
    private SkillLevel _buildingLevel = SkillLevel.None;
    private int _buildingExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel BuildingLevel
    {
        get => _buildingLevel;
        set => SetSkill(ref _buildingLevel, value, skillType.Building);
    }

    [AutoNetworkedField]
    public int BuildingExp
    {
        get => _buildingExp;
        set => SetSkill(ref _buildingExp, value, skillType.Building);
    }

//Исследование
    [DataField("ResearchLevel")]
    private SkillLevel _researchLevel = SkillLevel.None;
    private int _researchExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel ResearchLevel
    {
        get => _researchLevel;
        set => SetSkill(ref _researchLevel, value, skillType.Research);
    }

    [AutoNetworkedField]
    public int ResearchExp
    {
        get => _researchExp;
        set => SetSkill(ref _researchExp, value, skillType.Research);
    }
//Преступность
    [DataField("CrimeLevel")]
    private SkillLevel _crimeLevel = SkillLevel.None;
    private int _crimeExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public SkillLevel CrimeLevel
    {
        get => _crimeLevel;
        set => SetSkill(ref _crimeLevel, value, skillType.Crime);
    }

    [AutoNetworkedField]
    public int CrimeExp
    {
        get => _crimeExp;
        set => SetSkill(ref _crimeExp, value, skillType.Crime);
    }
//Пилотирование
    [DataField("Piloting")]
    private bool _ezpiloting = false;
    private int _ezpilotingExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool Piloting
    {
        get => _ezpiloting;
        set => SetSkill(ref _ezpiloting, value, skillType.Piloting);
    }

    [AutoNetworkedField]
    public int PilotingExp
    {
        get => _ezpilotingExp;
        set => SetSkill(ref _ezpilotingExp, value, skillType.Piloting);
    }

//Муз. Инструменты
    [DataField("MusInstruments")]
    private bool _ezmusInstruments = false;
    private int _ezmusInstrumentsExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool MusInstruments
    {
        get => _ezmusInstruments;
        set => SetSkill(ref _ezmusInstruments, value, skillType.MusInstruments);
    }

    [AutoNetworkedField]
    public int MusInstrumentsExp
    {
        get => _ezmusInstrumentsExp;
        set => SetSkill(ref _ezmusInstrumentsExp, value, skillType.MusInstruments);
    }
//Ботаника
    [DataField("Botany")]
    private bool _ezbotany = false;
    private int _ezbotanyExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool Botany
    {
        get => _ezbotany;
        set => SetSkill(ref _ezbotany, value, skillType.Botany);
    }

    [AutoNetworkedField]
    public int BotanyExp
    {
        get => _ezbotanyExp;
        set => SetSkill(ref _ezbotanyExp, value, skillType.Botany);
    }

//Бюрократия
    [DataField("Bureaucracy")]
    private bool _ezbureaucracy = false;
    private int _ezbureaucracyExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool Bureaucracy
    {
        get => _ezbureaucracy;
        set => SetSkill(ref _ezbureaucracy, value, skillType.Bureaucracy);
    }

    [AutoNetworkedField]
    public int BureaucracyExp
    {
        get => _ezbureaucracyExp;
        set => SetSkill(ref _ezbureaucracyExp, value, skillType.Bureaucracy);
    }

//Атмосфера
    [DataField("Atmosphere")]
    private bool _ezatmosphere = false;
    private int _ezatmosphereExp = 0;

    [ViewVariables(VVAccess.ReadOnly), AutoNetworkedField]
    public bool Atmosphere
    {
        get => _ezatmosphere;
        set => SetSkill(ref _ezbureaucracy, value, skillType.Atmosphere);
    }

    [AutoNetworkedField]
    public int AtmosphereExp
    {
        get => _ezatmosphereExp;
        set => SetSkill(ref _ezatmosphereExp, value, skillType.Atmosphere);
    }

#endregion
#region Методы
    // Перегрузка для основных навыков
    private void SetSkill(ref SkillLevel field, SkillLevel value, skillType type)
    {
        if (field == value)
            return;

        field = value;
        var ev = new SkillLevelChangedEvent(type, false);
        _entityManager.EventBus.RaiseLocalEvent(Owner, ev);
    }
    // Перегрузка для лёгких навыков
    private void SetSkill(ref bool field, bool value, skillType type)
    {
        if (field == value)
            return;

        field = value;
        var ev = new SkillLevelChangedEvent(type, false);
        _entityManager.EventBus.RaiseLocalEvent(Owner, ev);
    }
    // Перегрузка для опыта
    private void SetSkill(ref int field, int value, skillType type)
    {
        if (field == value)
            return;

        field = value;
        var ev = new SkillLevelChangedEvent(type, true);
        _entityManager.EventBus.RaiseLocalEvent(Owner, ev);
    }

    

    //получить уровень навыка
    public SkillLevel? GetSkillLevel(skillType skill)
    {
        return skill switch
        {
            skillType.Chemistry => ChemistryLevel,
            skillType.Medicine => MedicineLevel,
            skillType.RangeWeapon => RangeWeaponLevel,
            skillType.MeleeWeapon => MeleeWeaponLevel,
            skillType.Research => ResearchLevel,
            skillType.Crime => CrimeLevel,
            skillType.Building => BuildingLevel,
            skillType.Engineering => EngineeringLevel,
            _ => null // Возвращаем null, если skillType неизвестен
        };
    }
    //получить лёгкий навык
    public bool? GetEasySkill(skillType skill)
    {
        return skill switch
        {
            skillType.Piloting => Piloting,
            skillType.MusInstruments => MusInstruments,
            skillType.Botany => Botany,
            skillType.Bureaucracy => Bureaucracy,
            skillType.Atmosphere => Atmosphere,
            _ => null 
        };
    }
    //Это лёгкий или обычный навык?
    public bool IsEasySkill(skillType skill)
    {
        return skill switch
        {
            skillType.Piloting => true,
            skillType.MusInstruments => true,
            skillType.Botany => true,
            skillType.Bureaucracy => true,
            skillType.Atmosphere => true,
            _ => false 
        };
    }
    //получить опыт навыка
    public int GetSkillExp(skillType skill)
    {
        return skill switch
        {
            skillType.Chemistry => ChemistryExp,
            skillType.Medicine => MedicineExp,
            skillType.RangeWeapon => RangeWeaponExp,
            skillType.MeleeWeapon => MeleeWeaponExp,
            skillType.Research => ResearchExp,
            skillType.Crime => CrimeExp,
            skillType.Building => BuildingExp,
            skillType.Engineering => EngineeringExp,
            skillType.Piloting => PilotingExp,
            skillType.MusInstruments => MusInstrumentsExp,
            skillType.Botany => BotanyExp,
            skillType.Bureaucracy => BureaucracyExp,
            skillType.Atmosphere => AtmosphereExp,
            _ => -1
        };
    }
#endregion
}

#region типы

[Serializable, NetSerializable]
public enum skillType : byte
{
    Piloting = 0,
    RangeWeapon = 1,
    MeleeWeapon = 2,
    Medicine = 3,
    Chemistry = 4,
    Engineering = 5,
    Building = 6,
    Research = 7,
    Crime = 8,
    MusInstruments = 9,
    Botany = 10,
    Bureaucracy = 11,
    Atmosphere = 12,
}

[Serializable, NetSerializable]
public enum SkillLevel
{
    None = 0,   
    Basic = 1,   
    Advanced = 2, 
    Expert = 3 
}

public sealed class SkillLevelChangedEvent : EntityEventArgs
{
    public skillType Skill { get; }
    public bool IsExp { get; }

    public SkillLevelChangedEvent(skillType skill, bool isExp)
    {
        Skill = skill;
        IsExp = isExp;
    }
}
#endregion
