using Content.Server.Vanilla.NPC.Systems;
using Content.Shared.Actions.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.Vanilla.NPC.Components;

/// <summary>
/// Будет использовать акшн если возможно
/// </summary>
[RegisterComponent, Access(typeof(NPCUseActionIfPossibleSystem))]
public sealed partial class NPCUseActionIfPossibleComponent : Component
{
    [DataField]
    public string? TargetKey; // Будет использовать акшн проверяя также присутствие таргета

    [DataField(required: true)]
    public EntProtoId<InstantActionComponent> ActionId;

    [DataField]
    public EntityUid? ActionEnt;
}
