using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Alert;

namespace Content.Shared.Vanilla.BloodSucker;


[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BloodSuckerComponent : Component
{

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextUpdate;

    /// <summary>
    /// Из каких источников сосем кровь? По умолчанию из следов и луж.
    /// </summary>
    [DataField]
    public string[] Solutions = new string[] { "puddle" };

    /// <summary>
    /// интервал, не трогать
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Сколько юнитов крови будет сосаться из лужи в интервал
    /// </summary>
    [DataField]
    public float UnitsPerInterval = 8f;

    /// <summary>
    /// Сколько юнитов крови будет сгорать просто так из хранилища при фулл хп в интервал
    /// </summary>
    [DataField]
    public float UnitsDecayPerInterval = 0.1f;

    /// <summary>
    /// Сколько юнитов крови из хранилища будет переводиться в отхилл в интервал
    /// </summary>
    [DataField]
    public float UnitsRestoreToHealPerInterval = 4f;

    /// <summary>
    /// Наше кровехранилище
    /// </summary>
    [DataField, AutoNetworkedField]
    public float BloodStorage = 100f;

    /// <summary>
    /// сколько в текущий момент крови находится внутри нас?
    /// </summary>
    [DataField, AutoNetworkedField]
    public float AmountOfBloodInStorage = 100f;
    
    /// <summary>
    /// Отхилл за 1 ед. крови
    /// </summary>
    [DataField("heal", required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Heal = new();

    /// <summary>
    /// Урон за отсутствие крови 
    /// </summary>
    [DataField("bloodlessPenalty", required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier BloodlessPenalty = new();

    /// <summary>
    /// Радиус сосания
    /// </summary>
    [DataField]
    public float Range = 1f;

    /// <summary>
    /// Звук всасывания крови
    /// </summary>
    [DataField]
    public SoundSpecifier ManualDrainSound = new SoundPathSpecifier("/Audio/Effects/Fluids/slosh.ogg");

    [DataField]
    public ProtoId<AlertPrototype> BloodAlert = "BloodR1";

}
