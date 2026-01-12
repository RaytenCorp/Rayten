using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Audio;
namespace Content.Shared.Vanilla.Skill;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class SkillComponent : Component
{
    /// <summary>
    /// очки навыков
    /// </summary>
    [DataField, AutoNetworkedField]
    public int SkillPoints { get; set; } = 0;

    [DataField, AutoNetworkedField]
    public SoundSpecifier UnSkillSound = new SoundPathSpecifier("/Audio/Vanilla/SkillSystem/meep-merp.ogg");
    [DataField, AutoNetworkedField]
    public SoundSpecifier HeadShotSound = new SoundPathSpecifier("/Audio/Vanilla/SkillSystem/headshot.ogg");
    [DataField, AutoNetworkedField]
    public SoundSpecifier LvlUpSound = new SoundPathSpecifier("/Audio/Vanilla/SkillSystem/levelup.ogg");

    /// <summary>
    /// основные навыки, которыми обладает сущность
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<SkillType, SkillLevel> BasicSkills = [];
    /// <summary>
    /// лёгкие навыки, которым обладает сущность
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<SkillType> EasySkills = [];

    /// <summary>
    /// Опыт навыков
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<SkillType, int> SkillExps = [];

}

/// <summary>
/// навыки
/// </summary>
[Serializable, NetSerializable]
public enum SkillType : byte
{
    Weapon,
    Medicine,
    Engineering,
    Piloting,
    Research,
    MusInstruments,
    Botany,
    Bureaucracy
}

/// <summary>
/// уровни навыков
/// </summary>
[Serializable, NetSerializable]
public enum SkillLevel
{
    None = 0,
    Basic = 1,
    Advanced = 2,
    Expert = 3
}
/// <summary>
/// Тип навыка
/// </summary>
[Serializable, NetSerializable]
public enum SkillKind : byte
{
    Basic,//базовый
    Easy //легкий
}
public static class SkillTypeExtensions
{
    private static readonly HashSet<SkillType> EasySkills =
    [
        SkillType.Piloting,
        SkillType.Botany,
        SkillType.MusInstruments,
        SkillType.Bureaucracy,
        SkillType.Research
    ];

    public static SkillKind GetKind(this SkillType skill)
        => EasySkills.Contains(skill) ? SkillKind.Easy : SkillKind.Basic;
}
