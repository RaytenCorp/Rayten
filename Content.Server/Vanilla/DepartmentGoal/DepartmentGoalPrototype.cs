using Robust.Shared.Prototypes;

namespace Content.Server.Vanilla.DepartmentGoal;

[Serializable, Prototype("departmentgoal")]
public sealed class DepartmentGoalPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    [DataField]
    public string Text { get; set; } = string.Empty;

    [DataField("department")]
    public department Department;

    [DataField("weight")]
    public float Weight { get; set; } = 1.0f;
    // [DataField("benefits")] TODO
    // public List<BenefitPrototype> Benefits { get; set; } = new();
}

[Serializable, Prototype("benefit")]
public abstract class BenefitPrototype
{
    [DataField("type", required: true)]
    public string Type { get; set; } = default!;
}

[Serializable]
public enum department
{
    RnD = 0,   
    MED = 1,   
    CARGO = 2, 
    ENG = 3,
    SEC = 4,
    SRV = 5 
}