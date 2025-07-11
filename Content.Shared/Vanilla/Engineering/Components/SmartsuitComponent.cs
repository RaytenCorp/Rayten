using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Shared.Atmos;
using Robust.Shared.Timing;
using Robust.Shared.Prototypes;

namespace Content.Shared.Vanilla.Engineering.Components;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class SmartsuitComponent : Component
{

    /// <summary>
    /// Включён ли костюм
    /// </summary>
    [DataField("activated")]
    public bool Activated = true;

    /// <summary>
    /// Автовключение бутсов и джета
    /// </summary>
    [DataField("deviceConnection")]
    public bool DeviceConnection = true;

    /// <summary>
    /// Голосовая система
    /// </summary>
    [DataField("reporting")]
    public bool Reporting = true;

    /// <summary>
    /// При самовольном надевании
    /// </summary>
    [DataField]
    public bool ForceEquipped = false;

    /// <summary>
    /// Прототип шлема
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public EntProtoId HelmetPrototype = default!;

    /// <summary>
    /// Поглощаемый газ
    /// </summary>
    [DataField("gasType"), ViewVariables(VVAccess.ReadWrite)]
    public Gas GasType = Gas.Oxygen;

    /// <summary>
    /// Скорость поглощения
    /// </summary>
    [DataField("absorptionRate")]
    public float AbsorptionRate = 0.05f;

    [DataField("updateSpeed")]
    public TimeSpan UpdateSpeed = TimeSpan.FromSeconds(1);

    [DataField("nextUpdate", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;
}
