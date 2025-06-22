using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Shared.Atmos;
using Robust.Shared.Timing;

namespace Content.Shared.Vanilla.Warmer;

[RegisterComponent]
public sealed partial class WarmerComponent : Component
{

    /// <summary>
    /// Нагревать только если предмет надет
    /// </summary>
    [DataField("onlyWeared")]
    public bool OnlyWeared = false;

    /// <summary>
    /// Сила нагрева тела того, кто держит энтити
    /// </summary>
    [DataField("bodyHeatStrength")]
    public float BodyHeatStrength = 1000f;

    /// <summary>
    /// Сила нагрева тайла
    /// </summary>
    [DataField("tileHeatStrength")]
    public float TileHeatStrength = 2f;

    /// <summary>
    /// Макс температура нагрева тайла
    /// </summary>
    [DataField("heatMaxTemp")]
    public float HeatMaxTemp = 313.15f;

    /// <summary>
    /// Газ, который будет выбрасываться в атмосферу
    /// </summary>
    [DataField("gasType"), ViewVariables(VVAccess.ReadWrite)]
    public Gas GasType = Gas.CarbonDioxide;

    /// <summary>
    /// Кол-во молей в секунду
    /// </summary>
    [DataField("moleRatio"), ViewVariables(VVAccess.ReadWrite)]
    public float MoleRatio = 0.025f;

    [DataField("heatSpeed")]
    public TimeSpan HeatSpeed = TimeSpan.FromSeconds(1);
    [DataField("nextUpdate", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;
}
