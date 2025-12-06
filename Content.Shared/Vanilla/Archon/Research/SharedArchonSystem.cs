using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

using Content.Shared.Archon.Components;
using Content.Shared.Examine;

namespace Content.Shared.Archon.Systems;

public sealed partial class SharedArchonResearchSystem : EntitySystem
{

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArchonScannerComponent, ExaminedEvent>(OnScannerExamine);
        SubscribeLocalEvent<ArchonBeaconComponent, ExaminedEvent>(OnBeaconExamine);

    }
    private void OnScannerExamine(EntityUid uid, ArchonScannerComponent comp, ref ExaminedEvent args)
    {

        ShowArchonID(uid, comp.LinkedArchon, ref args);

    }
    private void OnBeaconExamine(EntityUid uid, ArchonBeaconComponent comp, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        ShowArchonID(uid, comp.LinkedArchon, ref args);
    }

    private void ShowArchonID(EntityUid uid, EntityUid? linkedArchon, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        if (linkedArchon != null)
            args.PushMarkup($"Привязан архонт с сигнатурой: {linkedArchon.Value}");
    }
}
