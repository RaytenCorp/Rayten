
using Content.Shared.Access;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared.Vanilla.Entities.ArsenalAuthorizator;

[RegisterComponent]
public sealed partial class ArsenalDoorComponent : Component
{
    /// <summary>
    /// Доступ будет добавляться и убираться у двери, запрещая проходить мобам через неё
    /// </summary>
    [DataField]
    public ProtoId<AccessLevelPrototype> BlockAccess = "Security";
    [DataField]
    public SoundSpecifier AccessDeniedSound = new SoundPathSpecifier("/Audio/Vanilla/Objects/forcefiled-touch.ogg");
}
