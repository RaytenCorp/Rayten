using Robust.Shared.Prototypes;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared.Vanilla.Bureaucracy;

[Serializable, Prototype("LawInfo")]
public sealed class LawInfoPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = default!;

    public LocId Name => $"law-name-{Code}";
    public LocId Desc => $"law-desc-{Code}";

    [DataField]
    public int Category { get; set; } = -1;

    [DataField]
    public int Danger { get; set; } = -1;
    public int BaseTime => Danger switch
    {
        1 => 5,
        2 => 10,
        3 => 20,
        4 => 30,
        _ => 0
    };
    public int Code => Danger * 100 + Category;
}