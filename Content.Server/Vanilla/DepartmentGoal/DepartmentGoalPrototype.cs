using Robust.Shared.Prototypes;

namespace Content.Server.Vanilla.DepartmentGoal;

[Serializable, Prototype("departmentgoal")]
public sealed partial class DepartmentGoalPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string Text { get; private set; } = string.Empty;

    [DataField("department")]
    public Department Department;

    [DataField("weight")]
    public float Weight { get; private set; } = 1.0f;
}


[Serializable]
public enum Department
{
    RnD = 0,
    MED = 1,
    CARGO = 2,
    ENG = 3,
    SEC = 4,
    SRV = 5
}