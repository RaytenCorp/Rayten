using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Archon.ShyGuy;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class ShyGuyComponent : Component
{
    #region сетевые поля
    /// <summary>
    /// текущее состояние
    /// </summary>
    [DataField, AutoNetworkedField]
    public ShyGuyState State = ShyGuyState.Calm;
    /// <summary>
    /// когда он успокоится
    /// </summary>
    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan RageEndAt = TimeSpan.Zero;
    /// <summary>
    /// когда входим в рейдж
    /// </summary>
    [DataField, AutoNetworkedField, AutoPausedField]
    public TimeSpan RageStartAt = TimeSpan.Zero;
    #endregion

    /// <summary>
    /// Длительность подготовки
    /// </summary>
    [DataField]
    public TimeSpan PreparingTime = TimeSpan.FromSeconds(8);
    /// <summary>
    /// длительность рейджа
    /// </summary>
    [DataField]
    public TimeSpan RageTime = TimeSpan.FromSeconds(30);

    /// <summary>
    /// обновление состояний
    /// </summary>
    [DataField, AutoPausedField]
    public TimeSpan nextUpdate = TimeSpan.Zero;

    [DataField]
    public float WalkModifier = 3f;

    [DataField]
    public float SprintModifier = 5f;

    [DataField]
    public SoundSpecifier? StingerSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/096trigger.ogg");

    [DataField]
    public SoundSpecifier? ChaseSound = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/096chase.ogg");

    [DataField]
    public SoundSpecifier? RageAmbient = new SoundPathSpecifier("/Audio/Vanilla/Effects/Archon/096raging.ogg");

    [DataField]
    public SoundSpecifier? CalmAmbient = new SoundPathSpecifier("/Audio/Vanilla/Ambience/096cry.ogg");
}

public enum ShyGuyState : byte
{
    Calm,
    Preparing,
    Rage
}

public sealed class OutlineHoverEvent : EntityEventArgs
{
    public EntityUid User { get; set; }

    public OutlineHoverEvent(EntityUid user)
    {
        User = user;
    }
}

[Serializable, NetSerializable]
public sealed class ShyGuyGazeEvent : EntityEventArgs
{
    public NetEntity ShyGuy;
    public NetEntity User;

    public ShyGuyGazeEvent(NetEntity shyGuy, NetEntity user)
    {
        ShyGuy = shyGuy;
        User = user;
    }
}
[Serializable, NetSerializable]
public enum ShyGuyVisuals : byte
{
    State
}
