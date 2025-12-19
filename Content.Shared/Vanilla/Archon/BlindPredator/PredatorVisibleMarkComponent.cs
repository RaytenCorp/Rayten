using Robust.Shared.GameStates;
namespace Content.Shared.Vanilla.Archon.BlindPredator;


[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class PredatorVisibleMarkComponent : Component
{
    /// <summary>
    /// Словарь предаторов которые нас видят или не видят
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<EntityUid, bool> Predators = new();
}
