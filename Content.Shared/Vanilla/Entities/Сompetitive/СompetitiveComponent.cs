using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Competitive;

[RegisterComponent]
public sealed partial class CompetitiveComponent : Component
{
    [DataField]
    public CompetitiveDifficult Difficult = CompetitiveDifficult.easy;
    [DataField]
    public string ActualName = "Неизвестно";
    [DataField]
    public string HiddenDesc = "Неизвестно";
    [DataField]
    public bool EnemyTechnology = false;
}

[Serializable, NetSerializable]
public enum CompetitiveDifficult
{
    easy, // 4 символа
    medium, // 6 символов
    hard, // 6 символов, без сброса
}
