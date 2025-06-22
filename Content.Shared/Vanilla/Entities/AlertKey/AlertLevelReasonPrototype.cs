using Robust.Shared.Prototypes;
namespace Content.Shared.Vanilla.AlertKey;

[Prototype("AlertLevelReason")]
public sealed partial class AlertLevelReasonPrototype : IPrototype
{
    /// <summary>
    /// айди карты
    /// </summary>
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// текст причины
    /// </summary>
    [DataField("text")]
    public string Text = "";

    /// <summary>
    /// Код, для которого существует эта причина
    ///
    /// </summary>
    [DataField("code")]
    public string Code = "green";
}
