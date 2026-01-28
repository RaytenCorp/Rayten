using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.DoAfter;
using Content.Shared.Damage;
using Content.Shared.Random;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Archon.PlagueDoctor;

/// <summary>
///     Компонент чумного доктора
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlagueDoctorComponent : Component
{
    #region звуки и анимации
    public TimeSpan NextUpdate;
    /// <summary>
    /// Длительность анимации
    /// </summary>
    [DataField]
    public TimeSpan RagingAnimationDuration = TimeSpan.FromSeconds(5.75);

    [AutoNetworkedField]
    public TimeSpan RagingAnimationEndAt;
    /// <summary>
    /// звук анонса
    /// </summary>
    [DataField]
    public SoundSpecifier RageAnnounceSound = new SoundPathSpecifier("/Audio/Vanilla/Announcements/049-announce.ogg");
    [DataField]
    public SoundSpecifier SurgerySound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/049/049surgery.ogg");
    /// <summary>
    /// звук во время анимации
    /// </summary>
    [DataField]
    public SoundSpecifier RagingSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/049/049raging.ogg");
    /// <summary>
    /// амбиент когда он находится в камере
    /// </summary>
    [DataField]
    public SoundSpecifier CageAmbient = new SoundPathSpecifier("/Audio/Vanilla/Ambience/049/Tension.ogg");
    /// <summary>
    /// амбиент когда он находится вне камеры
    /// </summary>
    [DataField]
    public SoundSpecifier FreeAmbient = new SoundPathSpecifier("/Audio/Vanilla/Ambience/049/SaveMeFrom.ogg");
    /// <summary>
    /// амбиент злого доктора
    /// </summary>
    [DataField]
    public SoundSpecifier RageAmbient = new SoundPathSpecifier("/Audio/Vanilla/Ambience/049/049Angry.ogg");
    [DataField]
    public SoundSpecifier HitSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/049/049hit.ogg");
    /// <summary>
    /// Аудиострим
    /// </summary>
    [ViewVariables]
    public EntityUid? Stream = null;
    #endregion
    #region поветрие

    [ViewVariables, AutoNetworkedField]
    public PlagueDoctorState State = PlagueDoctorState.Safe;
    /// <summary>
    /// Максимальный уровень поветрия
    /// </summary>
    [DataField]
    public float MaxPestilence = 100f;

    /// <summary>
    /// Текущий уровень поветрия
    /// </summary>
    [DataField, AutoNetworkedField]
    public float CurrentPestilence = 0;

    /// <summary>
    /// Скока поветрия будет добавляться в секунду?
    /// </summary>
    [DataField, AutoNetworkedField]
    public float PestilencePerSecond = 0.05f;
    [DataField]
    public ProtoId<AlertPrototype> PestilenceAlert = "Pestilence";
    #endregion

    #region хирургия
    /// <summary>
    /// хэшсет прототипов которые мы оперировали, нельзя проводить операции над одними и теми же
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public HashSet<string> OperatedProtos = [];
    //акшен
    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string ActionId = "Action049Surgery";
    [ViewVariables, AutoNetworkedField]
    public EntityUid? ActionEnt;
    /// <summary>
    /// Длительность проведения операции
    /// </summary>
    [DataField]
    public float SurgeryDoAfterTime = 16f;
    [DataField]
    public float PestilencePerSurgery = -20f;

    /// <summary>
    /// прототип весов исходов операций
    /// </summary>
    [DataField]
    public ProtoId<WeightedRandomPrototype> SurgeryResults = "049SurgeryResults";
    #endregion
    [DataField]
    public DamageSpecifier HitDamage = new()
    {
        DamageDict = new()
        {
            { "Cold", 228 }
        }
    };
}

[Serializable, NetSerializable]
public enum PlagueDoctorState : byte
{
    Safe,
    Raging,
    Rage,
}

//акшен-ивент проведения операция
public sealed partial class Surgery049Event : EntityTargetActionEvent
{
}
//ду-афтер операции
[Serializable, NetSerializable]
public sealed partial class Surgery049DoAfterEvent : SimpleDoAfterEvent
{
}