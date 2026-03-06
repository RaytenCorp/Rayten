using Content.Shared.FixedPoint;
using Content.Shared.Damage;
using Robust.Shared.Serialization;
using Robust.Shared.Animations;
using Robust.Shared.GameStates;
using System.Numerics;

namespace Content.Shared.Vanilla.Archon.OldMan;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class PDAnimationComponent : Component, IAnimationProperties
{
    [AutoNetworkedField]
    public bool IsOut = false;
    [AutoNetworkedField]
    public TimeSpan TeleportationEndAt = TimeSpan.Zero;
    public float TeleportDuration = 2.5f;

    [Animatable]
    public Vector2 InsertOffset { get; set; } = Vector2.Zero;
    public EntityUid PuddleEntity = default;
    void IAnimationProperties.SetAnimatableProperty(string name, object value)
    {
        AnimationHelper.SetAnimatableProperty(this, name, value);
    }
}

[Serializable, NetSerializable]
public sealed class FallAnimationEvent(NetEntity target) : EntityEventArgs
{
    public readonly NetEntity Target = target;
}
[RegisterComponent]
public sealed partial class DimensionEscapeTeleportComponent : Component
{
    [DataField]
    public bool IsFake = false;
}
