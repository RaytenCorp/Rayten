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
    //Ближний бой
    [DataField("WeaponLevel")]
    private SkillLevel _weaponLevel = SkillLevel.None;
    private int _weaponExp = 0;

    [AutoNetworkedField]
    public SkillLevel WeaponLevel
    {
        get => _weaponLevel;
        set => SetSkill(ref _weaponLevel, value, skillType.Weapon);
    }

    [AutoNetworkedField]
    public int WeaponExp
    {
        get => _weaponExp;
        set => SetSkill(ref _weaponExp, value, skillType.Weapon);
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
            skillType.Medicine => MedicineLevel,
            skillType.Weapon => WeaponLevel,
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
            skillType.Research => true,
            _ => false
        };
    }
    //получить опыт навыка
    public int GetSkillExp(skillType skill)
    {
        return skill switch
        {
            skillType.Medicine => MedicineExp,
            skillType.Weapon => WeaponExp,
            skillType.Engineering => EngineeringExp,
            skillType.Piloting => PilotingExp,
            skillType.MusInstruments => MusInstrumentsExp,
            skillType.Botany => BotanyExp,
            skillType.Bureaucracy => BureaucracyExp,
            skillType.Research => ResearchExp,
            _ => -1
        };
    }
    // Перегрузка для основных навыков
    public void FuckSkills()
    {
        Piloting = true;
        MusInstruments = true;
        Botany = true;
        Bureaucracy = true;
        Research = true;
        WeaponLevel = SkillLevel.Expert;
        MedicineLevel = SkillLevel.Expert;
        EngineeringLevel = SkillLevel.Expert;
    }
    #endregion
}

#region типы

[Serializable, NetSerializable]
public enum skillType : byte
{
    Piloting = 0,
    Weapon = 1,
    Medicine = 2,
    Engineering = 3,
    Research = 4,
    MusInstruments = 5,
    Botany = 6,
    Bureaucracy = 7
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