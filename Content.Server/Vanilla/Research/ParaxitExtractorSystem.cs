using Content.Server.Research.Systems;
using Content.Shared.UserInterface;
using Content.Shared.Research;
using Content.Shared.Vanilla.Research;
using Content.Shared.Research.Components;
using Robust.Server.Audio;
using Robust.Server.GameObjects;

namespace Content.Server.Vanilla.Research;

public sealed class ParaxitExtractorSystem : EntitySystem
{
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly ResearchSystem _research = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ParaxitExtractorComponent, ParaxitExtractorExtractMessage>(OnExtract);
        SubscribeLocalEvent<ParaxitExtractorComponent, ResearchServerPointsChangedEvent>(OnPointsChanged);
        SubscribeLocalEvent<ParaxitExtractorComponent, ResearchRegistrationChangedEvent>(OnRegistrationChanged);
        SubscribeLocalEvent<ParaxitExtractorComponent, BeforeActivatableUIOpenEvent>(OnBeforeUiOpen);
    }

    private void OnExtract(EntityUid uid, ParaxitExtractorComponent component, ParaxitExtractorExtractMessage args)
    {
        if (!_research.TryGetClientServer(uid, out var server, out var serverComp))
            return;

        if (serverComp.AdvancedPoints < component.PricePerExtract)
            return;

        _research.ModifyServerAdvancedPoints(server.Value, -component.PricePerExtract, serverComp);
        _audio.PlayPvs(component.ExtractSound, uid);
        Spawn(component.ExtractPrototype, Transform(uid).Coordinates);
        UpdateUserInterface(uid, component);
    }

    public void UpdateUserInterface(EntityUid uid, ParaxitExtractorComponent? component = null)
    {
        if (!Resolve(uid, ref component, false))
            return;

        if (!_research.TryGetClientServer(uid, out _, out var server))
            return;

        var canPrint = server.AdvancedPoints >= component.PricePerExtract;

        var state = new ParaxitExtractorBoundUserInterfaceState(server.AdvancedPoints, component.PricePerExtract, canPrint);
        _ui.SetUiState(uid, ParaxitExtractorUiKey.Key, state);
    }

    private void OnPointsChanged(EntityUid uid, ParaxitExtractorComponent component, ref ResearchServerPointsChangedEvent args)
    {
        UpdateUserInterface(uid, component);
    }

    private void OnRegistrationChanged(EntityUid uid, ParaxitExtractorComponent component, ref ResearchRegistrationChangedEvent args)
    {
        UpdateUserInterface(uid, component);
    }

    private void OnBeforeUiOpen(EntityUid uid, ParaxitExtractorComponent component, BeforeActivatableUIOpenEvent args)
    {
        UpdateUserInterface(uid, component);
    }

    private void OnShutdown(EntityUid uid, ParaxitExtractorComponent component, ComponentShutdown args)
    {
        UpdateUserInterface(uid);
    }
}