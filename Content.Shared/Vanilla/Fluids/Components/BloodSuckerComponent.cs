using Robust.Shared.GameStates;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Alert;
using Content.Shared.Actions;

namespace Content.Shared.Vanilla.BloodSucker.Components;


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
    public float UnitsDecayPerInterval = 0.45f;

    /// <summary>
    /// Сколько юнитов крови из хранилища будет переводиться в отхилл в интервал
    /// </summary>
    [DataField]
    public float UnitsRestoreToHealPerInterval = 4f;

    /// <summary>
    /// Наше кровехранилище
    /// </summary>
    [DataField, AutoNetworkedField]
    public float BloodStorage = 200f;

    /// <summary>
    /// сколько в текущий момент крови находится внутри нас?
    /// </summary>
    [DataField("amountOfBloodInStorage"), AutoNetworkedField]
    public float AmountOfBloodInStorage = 200f;
    
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
    public float Range = 2f;
    
    /// <summary>
    /// Должен ли переводить кровь в отхилл
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool CanHeal = false;

    [DataField]
    public ProtoId<AlertPrototype> BloodAlert = "BloodR1";

    [DataField]
    public EntityUid? HealingActionEntity;

    [DataField]
    public EntProtoId HealingAction = "ActionToggleHealing";

}

public sealed partial class ToggleHealingActionEvent : InstantActionEvent
{
}
