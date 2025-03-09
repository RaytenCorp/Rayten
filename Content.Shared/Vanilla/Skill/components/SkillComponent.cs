using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Skill;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SkillComponent : Component
{
    //очки навыков
    [DataField("SkillPoints"), AutoNetworkedField]
    public int SkillPoints { get; set; } = 0;

    //Стрельба
    [DataField("RangeWeaponLevel"), AutoNetworkedField]
    public SkillLevel RangeWeaponLevel { get; set; } = 0;
    [DataField("RangeWeaponExp"), AutoNetworkedField]
    public int RangeWeaponExp { get; set; } = 0;

    //Ближний бой
    [DataField("MeleeWeaponLevel"), AutoNetworkedField]
    public SkillLevel MeleeWeaponLevel { get; set; } = 0;
    [DataField("MeleeWeaponExp"), AutoNetworkedField]
    public int MeleeWeaponExp { get; set; } = 0;

    //Медицина
    [DataField("MedicineLevel"), AutoNetworkedField]
    public SkillLevel MedicineLevel { get; set; } = 0;
    [DataField("MedicineExp"), AutoNetworkedField]
    public int MedicineExp { get; set; } = 0;

    //Химия
    [DataField("ChemistryLevel"), AutoNetworkedField]
    public SkillLevel ChemistryLevel { get; set; } = 0;
    [DataField("ChemistryExp"), AutoNetworkedField]
    public int ChemistryExp { get; set; } = 0;

    //Инженерия
    [DataField("EngineeringLevel"), AutoNetworkedField]
    public SkillLevel EngineeringLevel { get; set; } = 0;
    [DataField("EngineeringExp"), AutoNetworkedField]
    public int EngineeringExp { get; set; } = 0;

    //Строительство
    [DataField("BuildingLevel"), AutoNetworkedField]
    public SkillLevel BuildingLevel { get; set; } = 0;
    [DataField("BuildingExp"), AutoNetworkedField]
    public int BuildingExp { get; set; } = 0;

    //Исследования
    [DataField("ResearchLevel"), AutoNetworkedField]
    public SkillLevel ResearchLevel { get; set; } = 0;
    [DataField("ResearchExp"), AutoNetworkedField]
    public int ResearchExp { get; set; } = 0;

    //Приборостроение
    [DataField("InstrumentationLevel"), AutoNetworkedField]
    public SkillLevel InstrumentationLevel { get; set; } = 0;
    [DataField("InstrumentationExp"), AutoNetworkedField]
    public int InstrumentationExp { get; set; } = 0;

    //Лёгкие навыки!!!

    //Пилотирование
    [DataField("Piloting"), AutoNetworkedField]
    public bool Piloting { get; set; } = false;
    [DataField("PilotingExp"), AutoNetworkedField]
    public int PilotingExp { get; set; } = 0;

    //Муз. инструменты
    [DataField("MusInstruments"), AutoNetworkedField]
    public bool MusInstruments { get; set; } = true;
    [DataField("MusInstrumentsExp"), AutoNetworkedField]
    public int MusInstrumentsExp { get; set; } = 0;

    //Ботаника
    [DataField("Botany"), AutoNetworkedField]
    public bool Botany { get; set; } = false;
    [DataField("BotanyExp"), AutoNetworkedField]
    public int BotanyExp { get; set; } = 0;

    //Бюрократия
    [DataField("Bureaucracy"), AutoNetworkedField]
    public bool Bureaucracy { get; set; } = false;
    [DataField("BureaucracyExp"), AutoNetworkedField]
    public int BureaucracyExp { get; set; } = 0;

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
            skillType.Instrumentation => InstrumentationLevel,
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
            _ => null 
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
            skillType.Instrumentation => InstrumentationExp,
            skillType.Building => BuildingExp,
            skillType.Engineering => EngineeringExp,
            skillType.Piloting => PilotingExp,
            skillType.MusInstruments => MusInstrumentsExp,
            skillType.Botany => BotanyExp,
            skillType.Bureaucracy => BureaucracyExp,
            _ => -1
        };
    }

}

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
    Instrumentation = 8,
    MusInstruments = 9,
    Botany = 10,
    Bureaucracy = 11,
}

[Serializable, NetSerializable]
public enum SkillLevel
{
    None = 0,   
    Basic = 1,   
    Advanced = 2, 
    Expert = 3 
}