using Content.Server.Objectives.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared.Cuffs.Components;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Server.Mind;
using Content.Server.Objectives.Components;
using Content.Shared.Objectives.Components;
using Content.Server.Vanilla.GameTicking.Rules.WhiteOut;

namespace Content.Server.Objectives.Systems;

public sealed class EscapeLondonersConditionSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EscapeLondonersConditionComponent, ObjectiveGetProgressEvent>(OnGetProgress);
    }

    private void OnGetProgress(EntityUid uid, EscapeLondonersConditionComponent comp, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = GetProgress(args.MindId, comp);
    }

    private float GetProgress(EntityUid mindId, EscapeLondonersConditionComponent comp)
    {
        if (mindId == null)
            return 0f;

        var whiteoutQuery = EntityQueryEnumerator<WhiteoutRuleComponent>();
        while (whiteoutQuery.MoveNext(out var ruleUid, out var whiteout))
        {
            if (whiteout.LondonersMapUid == null)
                continue;

            int playersOnMap = 0;
            var playerQuery = EntityQueryEnumerator<MindContainerComponent, TransformComponent>();
            while (playerQuery.MoveNext(out var playerUid, out var playerMind, out var playerXform))
            {
                if (playerMind.Mind == null || playerXform.MapUid != whiteout.LondonersMapUid)
                    continue;

                playersOnMap++;
            }

            return Math.Clamp((float)playersOnMap / comp.RequiredPlayers, 0f, 1f);
        }

        return 0f;
    }

}
