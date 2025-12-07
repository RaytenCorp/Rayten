using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.DoAfter;
using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Dominator;
/// Компонент доминатора, сам меняет режимы стрельбы, управляет гостролью и авторизацией
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DominatorComponent : Component
{
    ///Кем авторизовано
    [DataField, AutoNetworkedField]
    public EntityUid? AuthorizedID = null;
    ///Текущий режим стрельбы
    [DataField]
    public DominatorState CurrentState = DominatorState.Disabled;
    ///Список доступных режимов
    [DataField(required: true)]
    [AutoNetworkedField]
    public List<BatteryWeaponFireMode> FireModes = [];
    ///Радиус поиска опасных существ
    [DataField]
    public float ScanRange = 14.0f;
    ///КД сканирования
    [DataField]
    public float CheckDelay = 0.5f;

    public float Timer;
    public ProtoId<LocalizedDatasetPrototype> Dataset = "DominatorPhrases";
    public TimeSpan NextSpeechTime = TimeSpan.FromSeconds(0);
    [AutoNetworkedField]
    public bool AllowGhostTakeover = true;
}
[Serializable, NetSerializable]
public enum DominatorState : byte
{
    Disabled = 0,
    NonLethal = 1,
    Lethal = 2
}
[Serializable, NetSerializable]
public enum DominatorVisuals : byte
{
    firemod
}