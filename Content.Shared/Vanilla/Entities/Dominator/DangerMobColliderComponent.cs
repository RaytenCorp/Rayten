using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Dominator;
/// Данный компонент запрещает коллизию с опасными мобами, если их уровень угрозы меньше заданного
/// С сущностями, которые не имеют DangerMobComponent коллизия не запрещается

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DangerMobColliderComponent : Component
{
    ///Минимальное значение угрозы цели, чтобы сущность провзаимодействовала с опасным мобом
    [DataField, AutoNetworkedField]
    public int MinDanger = 10;
}