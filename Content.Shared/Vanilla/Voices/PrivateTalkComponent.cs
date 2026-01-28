namespace Content.Shared.Vanilla.Voices;

[RegisterComponent]
public sealed partial class PrivateTalkComponent : Component
{
    /// <summary>
    /// Сущность которая будет слышать любое чат-сообщение,
    /// туду переделать на массив.
    /// </summary>
    [DataField]
    public EntityUid? receiver;
}
