using Content.Server.NPC.HTN.Preconditions;
using Content.Server.NPC;
using Content.Shared.Buckle;
using Content.Shared.Vanilla.Archon.ShyGuy;

namespace Content.Server.Vanilla.NPC.HTN.Preconditions;

/// <summary>
/// Проверяет владелец в рейдже или нет
/// </summary>
public sealed partial class InRagePrecondition : HTNPrecondition
{
    private ShyGuySystem _shyGuy = default!;

    //нам нужно состояние рейджа или нам нужно сотояние антирейджа?
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("isRage")] public bool IsRage = true;

    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);
        _shyGuy = sysManager.GetEntitySystem<ShyGuySystem>();
    }

    public override bool IsMet(NPCBlackboard blackboard)
    {
        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        return IsRage && _shyGuy.IsRaged(owner) || !IsRage && !_shyGuy.IsRaged(owner);
    }
}