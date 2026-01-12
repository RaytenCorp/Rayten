using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.InvisibleHat;

/// <summary>
///     при надевании дает невидимость, мут и пацифизм
///     при снятии снимает
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class InvisibleHatComponent : Component
{
}
/// <summary>
///     блокирует взаимодействия
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BlockInteractionComponent : Component
{
}
