using Robust.Shared.Prototypes;
using Content.Shared.Verbs;

namespace Content.Shared.Vanilla.Bureaucracy;

[Serializable, Prototype("BureaucracyDocument")]
public sealed partial class BureaucracyDocumentPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string label { get; private set; } = "WTF";

    [DataField]
    public string Text { get; private set; } = "";

    [DataField]
    public int Priority { get; private set; } = 0;

    [DataField]
    public string Category { get; private set; } = "";

    public VerbCategory GetCategory()
    {
        // Пытаемся преобразовать строку в VerbCategory
        return Category switch
        {
            "Order" => VerbCategory.BureaucracyOrder,
            "Report" => VerbCategory.BureaucracyReports,
            "Request" => VerbCategory.BureaucracyRequest,
            _ => VerbCategory.Bureaucracy
        };
    }

}