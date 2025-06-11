using Content.Server.Spawners.Components;
using Content.Server.Ghost.Roles.Raffles;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Server.Vanilla.EventTeam;

[Prototype("eventteam")]
public sealed partial class EventTeamPrototype : IPrototype
{
    /// <summary>
    /// айди, например CBURN итд
    /// </summary>
    [ViewVariables]
    [IdDataField]
    public string ID { get; } = default!;
    /// <summary>
    /// заголовок анонса по сути
    /// </summary>
    [DataField]
    public LocId? Sender;
    /// <summary>
    /// текст анонса
    /// </summary>
    [ViewVariables]
    [DataField("announcementText")]
    public string? AnnouncementText;
    /// <summary>
    /// звук появления
    /// </summary>
    [ViewVariables]
    [DataField("sound")]
    public SoundSpecifier? Sound;
    /// <summary>
    /// цвет оповещения
    /// </summary>
    [ViewVariables]
    [DataField("announcementColor")]
    public Color? AnnouncementColor;
    /// <summary>
    /// На какое количество игроков будет приходиться спавн одного обычного юнита
    /// </summary>
    [ViewVariables]
    [DataField("spawnPerPlayers")]
    public int SpawnPerPlayers = 10;
    /// <summary>
    /// массив ролей, который будет заспавнен всегда (не учитываются в при подсчёте онлайна)
    /// </summary>
    [ViewVariables]
    [DataField("specialUnits")]
    public Dictionary<EntProtoId<SpawnPointComponent>, string> SpecialUnits = new();

    /// <summary>
    /// Обычный юнит, который будет спавниться в зависимости от онлайна
    /// </summary>
    [ViewVariables]
    [DataField("regularUnit")]
    public string RegularUnit = string.Empty;
    /// <summary>
    /// максимум доп. ролей
    /// </summary>
    [ViewVariables]
    [DataField("maxRegularUnitAmount")]
    public int MaxRegularUnitAmount = 8;
}
