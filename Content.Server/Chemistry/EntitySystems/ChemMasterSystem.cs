using Content.Server.Chemistry.Components;
using Content.Server.Popups;
using Content.Server.Storage.EntitySystems;
using Content.Server.Materials;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Storage;
using Content.Shared.Materials;
using Content.Shared.Tag;
using JetBrains.Annotations;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Content.Server.Chemistry.EntitySystems
{

    /// <summary>
    /// Contains all the server-side logic for ChemMasters.
    /// <seealso cref="ChemMasterComponent"/>
    /// </summary>
    [UsedImplicitly]
    public sealed class ChemMasterSystem : EntitySystem
    {
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly AudioSystem _audioSystem = default!;
        [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
        [Dependency] private readonly UserInterfaceSystem _userInterfaceSystem = default!;
        [Dependency] private readonly StorageSystem _storageSystem = default!;
        [Dependency] private readonly LabelSystem _labelSystem = default!;
        [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
        [Dependency] private readonly MaterialStorageSystem _materialStorage = default!;
        [Dependency] private readonly TagSystem _tagSystem = default!;
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

        //rayten-start
        private static readonly Dictionary<string, int> MedipenRecipe = new()
        {
            { "Glass", 200 }, //Предположительно в инъекторе есть стеклянные детали
            { "Steel", 300 },
            { "Plastic", 400 },
        };
        //rayten-end

        [ValidatePrototypeId<EntityPrototype>]
        private const string PillPrototypeId = "Pill";
        private const string MedipenPrototypeId = "Autoinjector"; //rayten

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<ChemMasterComponent, ComponentStartup>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, SolutionContainerChangedEvent>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, EntInsertedIntoContainerMessage>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, EntRemovedFromContainerMessage>(SubscribeUpdateUiState);
            SubscribeLocalEvent<ChemMasterComponent, BoundUIOpenedEvent>(SubscribeUpdateUiState);
            //rayten-start
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSyncRequestMessage>(OnChemMasterRequestMessage);
            SubscribeLocalEvent<ChemMasterComponent, MaterialAmountChangedEvent>(OnMaterialAmountChanged);
            SubscribeLocalEvent<ChemMasterComponent, GetMaterialWhitelistEvent>(OnGetWhitelist);
            SubscribeLocalEvent<ChemMasterComponent, MapInitEvent>(OnMapInit);
            //rayten-end

            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSetModeMessage>(OnSetModeMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSortingTypeCycleMessage>(OnCycleSortingTypeMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterSetPillTypeMessage>(OnSetPillTypeMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterReagentAmountButtonMessage>(OnReagentButtonMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterCreatePillsMessage>(OnCreatePillsMessage);
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterCreateMedipensMessage>(OnCreateMedipensMessage); //rayten
            SubscribeLocalEvent<ChemMasterComponent, ChemMasterOutputToBottleMessage>(OnOutputToBottleMessage);
        }

        private void SubscribeUpdateUiState<T>(Entity<ChemMasterComponent> ent, ref T ev)
        {
            UpdateUiState(ent);
        }

        private void UpdateUiState(Entity<ChemMasterComponent> ent, bool updateLabel = false)
        {
            var (owner, chemMaster) = ent;
            if (!_solutionContainerSystem.TryGetSolution(owner, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                return;
            var inputContainer = _itemSlotsSystem.GetItemOrNull(owner, SharedChemMaster.InputSlotName);
            var outputContainer = _itemSlotsSystem.GetItemOrNull(owner, SharedChemMaster.OutputSlotName);

            //rayten-start
            var inputInfo = BuildInputContainerInfo(inputContainer);
            var outputInfo = BuildOutputContainerInfo(outputContainer);
            //rayten-end

            var bufferReagents = bufferSolution.Contents;
            var bufferCurrentVolume = bufferSolution.Volume;

            var state = new ChemMasterBoundUserInterfaceState(
                //rayten-start
                chemMaster.Mode, chemMaster.SortingType, inputInfo, outputInfo,
                bufferReagents, bufferCurrentVolume, chemMaster.PillType, chemMaster.PillDosageLimit, chemMaster.MedipenDosageLimit, updateLabel, outputInfo?.ContainsOnlyPills ?? false,
                outputInfo?.ContainsOnlyMedipens ?? false);
            //rayten-end

            _userInterfaceSystem.SetUiState(owner, ChemMasterUiKey.Key, state);
        }

        //rayten-start
        private void OnGetWhitelist(EntityUid uid, ChemMasterComponent component, ref GetMaterialWhitelistEvent args)
        {
            if (args.Storage != uid)
                return;

            var allowedMaterials = new List<ProtoId<MaterialPrototype>>
            {
                new ProtoId<MaterialPrototype>("Steel"),
                new ProtoId<MaterialPrototype>("Plastic"),
                new ProtoId<MaterialPrototype>("Glass"),
            };

            var combined = args.Whitelist.Union(allowedMaterials).ToList();
            args.Whitelist = combined;
        }

        private void OnMaterialAmountChanged(Entity<ChemMasterComponent> chemMaster, ref MaterialAmountChangedEvent args)
        {
            UpdateUiState(chemMaster);
        }

        private void OnMapInit(EntityUid uid, ChemMasterComponent component, MapInitEvent args)
        {
            _appearance.SetData(uid, ChemMasterVisuals.IsInserting, false);
            _materialStorage.UpdateMaterialWhitelist(uid);
        }
        //rayten-end

        private void OnSetModeMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSetModeMessage message)
        {
            // Ensure the mode is valid, either Transfer or Discard.
            if (!Enum.IsDefined(typeof(ChemMasterMode), message.ChemMasterMode))
                return;

            chemMaster.Comp.Mode = message.ChemMasterMode;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnCycleSortingTypeMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSortingTypeCycleMessage message)
        {
            chemMaster.Comp.SortingType++;
            if (chemMaster.Comp.SortingType > ChemMasterSortingType.Latest)
                chemMaster.Comp.SortingType = ChemMasterSortingType.None;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnSetPillTypeMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterSetPillTypeMessage message)
        {
            // Ensure valid pill type. There are 20 pills selectable, 0-19.
            if (message.PillType > SharedChemMaster.PillTypes - 1)
                return;

            chemMaster.Comp.PillType = message.PillType;
            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private void OnReagentButtonMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterReagentAmountButtonMessage message)
        {
            // Ensure the amount corresponds to one of the reagent amount buttons.
            if (!Enum.IsDefined(typeof(ChemMasterReagentAmount), message.Amount))
                return;

            switch (chemMaster.Comp.Mode)
            {
                case ChemMasterMode.Transfer:
                    TransferReagents(chemMaster, message.ReagentId, message.Amount.GetFixedPoint(), message.FromBuffer);
                    break;
                case ChemMasterMode.Discard:
                    DiscardReagents(chemMaster, message.ReagentId, message.Amount.GetFixedPoint(), message.FromBuffer);
                    break;
                default:
                    // Invalid mode.
                    return;
            }

            ClickSound(chemMaster);
        }

        private void TransferReagents(Entity<ChemMasterComponent> chemMaster, ReagentId id, FixedPoint2 amount, bool fromBuffer)
        {
            var container = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
            if (container is null ||
                !_solutionContainerSystem.TryGetFitsInDispenser(container.Value, out var containerSoln, out var containerSolution) ||
                !_solutionContainerSystem.TryGetSolution(chemMaster.Owner, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
            {
                return;
            }

            if (fromBuffer) // Buffer to container
            {
                amount = FixedPoint2.Min(amount, containerSolution.AvailableVolume);
                amount = bufferSolution.RemoveReagent(id, amount, preserveOrder: true);
                _solutionContainerSystem.TryAddReagent(containerSoln.Value, id, amount, out var _);
            }
            else // Container to buffer
            {
                amount = FixedPoint2.Min(amount, containerSolution.GetReagentQuantity(id));
                _solutionContainerSystem.RemoveReagent(containerSoln.Value, id, amount);
                bufferSolution.AddReagent(id, amount);
            }

            UpdateUiState(chemMaster, updateLabel: true);
        }

        private void DiscardReagents(Entity<ChemMasterComponent> chemMaster, ReagentId id, FixedPoint2 amount, bool fromBuffer)
        {
            if (fromBuffer)
            {
                if (_solutionContainerSystem.TryGetSolution(chemMaster.Owner, SharedChemMaster.BufferSolutionName, out _, out var bufferSolution))
                    bufferSolution.RemoveReagent(id, amount, preserveOrder: true);
                else
                    return;
            }
            else
            {
                var container = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.InputSlotName);
                if (container is not null &&
                    _solutionContainerSystem.TryGetFitsInDispenser(container.Value, out var containerSolution, out _))
                {
                    _solutionContainerSystem.RemoveReagent(containerSolution.Value, id, amount);
                }
                else
                    return;
            }

            UpdateUiState(chemMaster, updateLabel: fromBuffer);
        }

        private void OnCreatePillsMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterCreatePillsMessage message)
        {
            var user = message.Actor;
            var maybeContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.OutputSlotName);
            if (maybeContainer is not { Valid: true } container
                || !TryComp(container, out StorageComponent? storage))
            {
                return; // output can't fit pills
            }

            // Ensure the number is valid.
            if (message.Number == 0 || !_storageSystem.HasSpace((container, storage)))
                return;

            // Ensure the amount is valid.
            if (message.Dosage == 0 || message.Dosage > chemMaster.Comp.PillDosageLimit)
                return;

            // Ensure label length is within the character limit.
            if (message.Label.Length > SharedChemMaster.LabelMaxLength)
                return;

            var needed = message.Dosage * message.Number;
            if (!WithdrawFromBuffer(chemMaster, needed, user, out var withdrawal))
            {
                PlayDenySound(chemMaster); //rayten
                return;
            }

            _labelSystem.Label(container, message.Label);

            for (var i = 0; i < message.Number; i++)
            {
                var item = Spawn(PillPrototypeId, Transform(container).Coordinates);
                _storageSystem.Insert(container, item, out _, user: user, storage);
                _labelSystem.Label(item, message.Label);

                _solutionContainerSystem.EnsureSolutionEntity(item, SharedChemMaster.PillSolutionName, out var itemSolution, message.Dosage);
                if (!itemSolution.HasValue)
                    return;

                _solutionContainerSystem.TryAddSolution(itemSolution.Value, withdrawal.SplitSolution(message.Dosage));

                var pill = EnsureComp<PillComponent>(item);
                pill.PillType = chemMaster.Comp.PillType;
                Dirty(item, pill);

                // Log pill creation by a user
                _adminLogger.Add(LogType.Action, LogImpact.Low,
                    $"{ToPrettyString(user):user} printed {ToPrettyString(item):pill} {SharedSolutionContainerSystem.ToPrettyString(itemSolution.Value.Comp.Solution)}");
            }

            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        //rayten-start
        private void OnCreateMedipensMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterCreateMedipensMessage message)
        {
            var user = message.Actor;
            var maybeContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.OutputSlotName);
            if (maybeContainer is not { Valid: true } container
                || !TryComp(container, out StorageComponent? storage))
            {
                return;
            }

            if (message.Number == 0 || !_storageSystem.HasSpace((container, storage)))
                return;

            if (message.Dosage == 0 || message.Dosage > chemMaster.Comp.MedipenDosageLimit)
                return;

            if (message.Label.Length > SharedChemMaster.LabelMaxLength)
                return;

            foreach (var (mat, recipe) in MedipenRecipe)
            {
                var requiredAmount = recipe * message.Number;
                if (_materialStorage.GetMaterialAmount(chemMaster, mat) < requiredAmount)
                {
                    PlayDenySound(chemMaster);
                    _popupSystem.PopupCursor(Loc.GetString("Недостаточно материалов!"), user);
                    return;
                }
            }

            var needed = message.Dosage * message.Number;
            if (!WithdrawFromBuffer(chemMaster, needed, user, out var withdrawal))
            {
                PlayDenySound(chemMaster);
                return;
            }

            foreach (var (mat, recipe) in MedipenRecipe)
            {
                var requiredAmount = recipe * message.Number;
                _materialStorage.TryChangeMaterialAmount(chemMaster, mat, -(int)requiredAmount);
            }

            _labelSystem.Label(container, message.Label);

            for (var i = 0; i < message.Number; i++)
            {
                var item = Spawn(MedipenPrototypeId, Transform(container).Coordinates);
                _storageSystem.Insert(container, item, out _, user: user, storage);
                _labelSystem.Label(item, message.Label);

                _solutionContainerSystem.EnsureSolutionEntity(item, SharedChemMaster.MedipenSolutionName, out var itemSolution, message.Dosage);
                if (!itemSolution.HasValue)
                    return;

                _solutionContainerSystem.TryAddSolution(itemSolution.Value, withdrawal.SplitSolution(message.Dosage));

                _adminLogger.Add(LogType.Action, LogImpact.Low,
                    $"{ToPrettyString(user):user} printed {ToPrettyString(item):medipen} {SharedSolutionContainerSystem.ToPrettyString(itemSolution.Value.Comp.Solution)}");
            }

            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }
        //rayten-end

        private void OnOutputToBottleMessage(Entity<ChemMasterComponent> chemMaster, ref ChemMasterOutputToBottleMessage message)
        {
            var user = message.Actor;
            var maybeContainer = _itemSlotsSystem.GetItemOrNull(chemMaster, SharedChemMaster.OutputSlotName);
            if (maybeContainer is not { Valid: true } container
                || !_solutionContainerSystem.TryGetSolution(container, SharedChemMaster.BottleSolutionName, out var soln, out var solution))
            {
                return; // output can't fit reagents
            }

            // Ensure the amount is valid.
            if (message.Dosage == 0 || message.Dosage > solution.AvailableVolume)
                return;

            // Ensure label length is within the character limit.
            if (message.Label.Length > SharedChemMaster.LabelMaxLength)
                return;

            if (!WithdrawFromBuffer(chemMaster, message.Dosage, user, out var withdrawal))
            {
                PlayDenySound(chemMaster); //rayten
                return;
            }

            _labelSystem.Label(container, message.Label);
            _solutionContainerSystem.TryAddSolution(soln.Value, withdrawal);

            // Log bottle creation by a user
            _adminLogger.Add(LogType.Action, LogImpact.Low,
                $"{ToPrettyString(user):user} bottled {ToPrettyString(container):bottle} {SharedSolutionContainerSystem.ToPrettyString(solution)}");

            UpdateUiState(chemMaster);
            ClickSound(chemMaster);
        }

        private bool WithdrawFromBuffer(
            Entity<ChemMasterComponent> chemMaster,
            FixedPoint2 neededVolume, EntityUid? user,
            [NotNullWhen(returnValue: true)] out Solution? outputSolution)
        {
            outputSolution = null;

            if (!_solutionContainerSystem.TryGetSolution(chemMaster.Owner, SharedChemMaster.BufferSolutionName, out _, out var solution))
            {
                return false;
            }

            if (solution.Volume == 0)
            {
                if (user.HasValue)
                    _popupSystem.PopupCursor(Loc.GetString("chem-master-window-buffer-empty-text"), user.Value);
                return false;
            }

            // ReSharper disable once InvertIf
            if (neededVolume > solution.Volume)
            {
                if (user.HasValue)
                    _popupSystem.PopupCursor(Loc.GetString("chem-master-window-buffer-low-text"), user.Value);
                return false;
            }

            outputSolution = solution.SplitSolution(neededVolume);
            return true;
        }

        private void ClickSound(Entity<ChemMasterComponent> chemMaster)
        {
            _audioSystem.PlayPvs(chemMaster.Comp.ClickSound, chemMaster, AudioParams.Default.WithVolume(-2f));
        }

        //rayten-start
        private void PlayDenySound(Entity<ChemMasterComponent> chemMaster)
        {
            if (_timing.CurTime >= chemMaster.Comp.NextDenySoundTime)
            {
                chemMaster.Comp.NextDenySoundTime = _timing.CurTime + chemMaster.Comp.DenySoundDelay;
                _audioSystem.PlayPvs(chemMaster.Comp.ErrorSound, chemMaster, AudioParams.Default.WithVolume(-3f));
            }
        }
        //rayten-end

        private ContainerInfo? BuildInputContainerInfo(EntityUid? container)
        {
            if (container is not { Valid: true })
                return null;

            if (!TryComp(container, out FitsInDispenserComponent? fits)
                || !_solutionContainerSystem.TryGetSolution(container.Value, fits.Solution, out _, out var solution))
            {
                return null;
            }

            return BuildContainerInfo(Name(container.Value), solution);
        }

        private ContainerInfo? BuildOutputContainerInfo(EntityUid? container)
        {
            if (container is not { Valid: true })
                return null;

            var name = Name(container.Value);
            {
                if (_solutionContainerSystem.TryGetSolution(
                        container.Value, SharedChemMaster.BottleSolutionName, out _, out var solution))
                {
                    return BuildContainerInfo(name, solution);
                }
            }

            if (!TryComp(container, out StorageComponent? storage))
                return null;

            //rayten-start
            var pillsCanister = _tagSystem.HasTag(container.Value, "PillCanister");
            var medipenCase = _tagSystem.HasTag(container.Value, "MedipenCase");

            var entities = new List<(string Id, FixedPoint2 Quantity)>();

            foreach (var entity in storage.Container.ContainedEntities)
            {
                if (_solutionContainerSystem.TryGetSolution(entity, SharedChemMaster.PillSolutionName, out _, out var pillSolution))
                {
                    entities.Add((Name(entity), pillSolution?.Volume ?? FixedPoint2.Zero));
                    continue;
                }

                if (_solutionContainerSystem.TryGetSolution(entity, SharedChemMaster.MedipenSolutionName, out _, out var medipenSolution))
                {
                    entities.Add((Name(entity), medipenSolution?.Volume ?? FixedPoint2.Zero));
                    continue;
                }
            }

            return new ContainerInfo(name, _storageSystem.GetCumulativeItemAreas((container.Value, storage)),
                storage.Grid.GetArea(),
                containsOnlyPills: pillsCanister && !medipenCase,
                containsOnlyMedipens: medipenCase && !pillsCanister)
            {
                Entities = entities
            };
            //rayten-end
        }

        private static ContainerInfo BuildContainerInfo(string name, Solution solution)
        {
            return new ContainerInfo(name, solution.Volume, solution.MaxVolume)
            {
                Reagents = solution.Contents
            };
        }

        //rayten-start
        private void OnChemMasterRequestMessage(EntityUid uid, ChemMasterComponent comp, ChemMasterSyncRequestMessage args)
        {
            UpdateUiState(new Entity<ChemMasterComponent>(uid, comp));
        }
        //rayten-end
    }
}
