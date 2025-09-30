using Robust.Shared.Utility;

namespace Content.Shared.Vanilla.Eye.Components;

/// <summary>
///     Отмечает, что существо видимо для хищника, в основном для HTN. Есть баг, что если для одной особи виден объект, он виден для всех, короч фича типа стайная координация
/// </summary>
[RegisterComponent]
public sealed partial class PredatorVisibleMarkComponent : Component
{

}
