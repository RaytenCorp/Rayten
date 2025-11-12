namespace Content.Shared.Vanilla.Entities.BlueSpaceSync;
///маркер для того чтобы десинхронизировать челика обратно
[RegisterComponent]
public sealed partial class BlueSpaceSyncComponent : Component
{
    [DataField]
    public TimeSpan EscapeTime = TimeSpan.Zero;

    [DataField]
    public float WalkModifier = 1.65f;

    [DataField]
    public float SprintModifier = 1.65f;
}