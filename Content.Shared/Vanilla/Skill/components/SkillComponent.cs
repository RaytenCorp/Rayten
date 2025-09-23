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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
    public SkillLevel EngineeringLevel
    {
        get => _engineeringLevel;
        set => SetSkill(ref _engineeringLevel, value, skillType.Engineering);
    }

    [AutoNetworkedField]
    public int EngineeringExp
    {
        get => _engineeringExp;
        set => SetSkill(ref _engineeringExp, value, skillType.Engineering);
    }

    //Преступность
    [DataField("CrimeLevel")]
    private SkillLevel _crimeLevel = SkillLevel.None;
    private int _crimeExp = 0;

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
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

    [AutoNetworkedField]
    public bool Atmosphere
    {
        get => _ezatmosphere;
        set => SetSkill(ref _ezatmosphere, value, skillType.Atmosphere);
    }

    [AutoNetworkedField]
    public int AtmosphereExp
    {
        get => _ezatmosphereExp;
        set => SetSkill(ref _ezatmosphereExp, value, skillType.Atmosphere);
    }
    //Исследования

    [DataField("Research")]
    private bool _ezresearch = false;
    private int _ezresearchExp = 0;

    [AutoNetworkedField]
    public bool Research
    {
        get => _ezresearch;
        set => SetSkill(ref _ezresearch, value, skillType.Research);
    }

    [AutoNetworkedField]
    public int ResearchExp
    {
        get => _ezresearchExp;
        set => SetSkill(ref _ezresearchExp, value, skillType.Research);
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
            skillType.Crime => CrimeLevel,
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
            skillType.Research => Research,
            _ => null
        };
    }
    //Это лёгкий или обычный навык?
    public static bool IsEasySkill(skillType skill)
    {
        return skill switch
        {
            skillType.Piloting => true,
            skillType.MusInstruments => true,
            skillType.Botany => true,
            skillType.Bureaucracy => true,
            skillType.Atmosphere => true,
            skillType.Research => true,
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
            skillType.Crime => CrimeExp,
            skillType.Engineering => EngineeringExp,
            skillType.Piloting => PilotingExp,
            skillType.MusInstruments => MusInstrumentsExp,
            skillType.Botany => BotanyExp,
            skillType.Bureaucracy => BureaucracyExp,
            skillType.Atmosphere => AtmosphereExp,
            skillType.Research => ResearchExp,
            _ => -1
        };
    }
    // Перегрузка для основных навыков
    public void FuckSkills(bool withCrime)
    {
        Piloting = true;
        MusInstruments = true;
        Botany = true;
        Bureaucracy = true;
        Atmosphere = true;
        Research = true;
        RangeWeaponLevel = SkillLevel.Expert;
        MeleeWeaponLevel = SkillLevel.Expert;
        MedicineLevel = SkillLevel.Expert;
        ChemistryLevel = SkillLevel.Expert;
        EngineeringLevel = SkillLevel.Expert;
        CrimeLevel = withCrime ? SkillLevel.Expert : CrimeLevel;
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
    Research = 6,
    Crime = 7,
    MusInstruments = 8,
    Botany = 9,
    Bureaucracy = 10,
    Atmosphere = 11,
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
