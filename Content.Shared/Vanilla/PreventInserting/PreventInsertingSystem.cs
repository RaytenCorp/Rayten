using Content.Shared.Storage.Components;
namespace Content.Shared.Vanilla;

public sealed partial class PreventInsertingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PreventInsertingComponent, InsertIntoEntityStorageAttemptEvent>(OnInsertAttempt);
    }
    private void OnInsertAttempt(Entity<PreventInsertingComponent> ent, ref InsertIntoEntityStorageAttemptEvent args)
    {
        args.Cancelled = true;
    }
}