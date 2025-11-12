using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Entities.BlueSpaceSync;

[RegisterComponent]
public sealed partial class BlueSpaceSyncAbilityComponent : Component
{
    /// <summary>
    /// Прототип акшена позволяющий синхронизироваться с блюспейсом
    /// </summary>
    [DataField]
    public EntProtoId Action = "ActionBlueSpaceSync";

    [DataField]
    public EntityUid? ActionEntity;

    [DataField]
    public SoundSpecifier? EnterSound;

    // Сколько секунд длится
    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(2);
}
