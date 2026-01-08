using Content.Server.NPC.HTN.Preconditions;
using Content.Server.NPC;

namespace Content.Server.Vanilla.NPC.HTN.Preconditions;

/// <summary>
/// Проверяет владелец в рейдже или нет
/// </summary>
public sealed partial class InRagePrecondition : HTNPrecondition
{
    //нам нужно состояние рейджа или нам нужно сотояние антирейджа?
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("isRage")] public bool IsRage = true;
    public override bool IsMet(NPCBlackboard blackboard)
    {
        return true;
    }
}