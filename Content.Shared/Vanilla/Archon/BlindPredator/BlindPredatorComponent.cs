using Robust.Shared.GameStates;

namespace Content.Shared.Vanilla.Archon.BlindPredator;
/// <summary>
/// сущность с данным компонентом будет видеть мобов только при условии что у них есть
/// <see cref="PredatorVisibleMarkComponent"/> и что в словаре этого компонента есть эта сущность с флагом true
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class BlindPredatorComponent : Component
{
    [DataField]
    public bool CanSeeOthers = false;
}
