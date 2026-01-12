using Robust.Shared.GameStates;
namespace Content.Shared.Vanilla;
/// <summary>
/// компонент не дает засунуть предмет или моба в контейнер (шкаф сундук итд)
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PreventInsertingComponent : Component
{ }