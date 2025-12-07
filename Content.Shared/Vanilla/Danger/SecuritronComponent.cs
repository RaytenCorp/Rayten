using Robust.Shared.Containers;
namespace Content.Shared.Vanilla.Dominator;

[RegisterComponent]
public sealed partial class SecuritronComponent : Component
{
    [ViewVariables]
    public ContainerSlot HandCuffContainer = default!;

}
