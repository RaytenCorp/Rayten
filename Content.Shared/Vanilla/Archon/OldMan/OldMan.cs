using Content.Shared.Actions;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Archon.OldMan;

[Serializable, NetSerializable]
public enum OldManVisuals : byte
{
    teleport,
}

[Serializable, NetSerializable]
public enum TeleportState : byte
{
    In,//входим
    Out,//выходим
    NoTP//не в телепортации
}

public sealed partial class OldManTeleportEvent : InstantActionEvent {}
