using Robust.Shared.Serialization;
namespace Content.Shared.Vanilla.Archon.OldMan.FemurBreaker;

[Serializable, NetSerializable]
public enum FemurBreakerDeviceVisuals : byte
{
    State
}
[Serializable, NetSerializable]
public enum FemurBreakerState : byte
{
    Down,
    Up,
    Static
}
