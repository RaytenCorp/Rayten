using Robust.Shared.Utility;

namespace Content.Shared.Eye.Blinding.Components;

/// <summary>
///     Существо будет закрывать глаза, если в N радиусе есть объект с компонентом BlockMovementOnEyeContactComponent
/// </summary>
[RegisterComponent]
public sealed partial class AutoEyeClosingComponent : Component
{

}
