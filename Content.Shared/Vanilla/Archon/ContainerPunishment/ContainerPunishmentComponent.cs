using Content.Shared.Damage;
using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.ContainerPunishment;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ContainerPunishmentComponent : Component
{
    /// <summary>
    /// Словарь, хранящий счетчики всех кто брал предмет из контейнера
    /// </summary>
    [ViewVariables, AutoNetworkedField]
    public Dictionary<EntityUid, int> Counters = [];
    /// <summary>
    /// Предупреждение, которое появляется если взять предмет из контейнера
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId? PopupMessage = "archon330-warning";
    /// <summary>
    /// количество предметов которые можно вытащить
    /// </summary>
    [DataField, AutoNetworkedField]
    public int MaxItems = 2;

    /// <summary>
    /// Урон который будет нанесён
    /// </summary>
    [DataField, AutoNetworkedField]
    public DamageSpecifier Damage = new()
    {
        DamageDict = new()
        {
            { "Slash", 350 }
        }
    };
}