using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry
{
    /// <summary>
    /// This class holds constants that are shared between client and server.
    /// </summary>
    public sealed class SharedChemMaster
    {
        public const uint PillTypes = 20;
        public const string BufferSolutionName = "buffer";
        public const string InputSlotName = "beakerSlot";
        public const string OutputSlotName = "outputSlot";
        public const string PillSolutionName = "food";
        public const string BottleSolutionName = "drink";
        public const string MedipenSolutionName = "pen";
        public const uint LabelMaxLength = 50;
    }

    //rayten-start
    [Serializable, NetSerializable]
    public sealed class ChemMasterSyncRequestMessage : BoundUserInterfaceMessage
    {

    }
    //rayten-end

    [Serializable, NetSerializable]
    public sealed class ChemMasterSetModeMessage : BoundUserInterfaceMessage
    {
        public readonly ChemMasterMode ChemMasterMode;

        public ChemMasterSetModeMessage(ChemMasterMode mode)
        {
            ChemMasterMode = mode;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterSetPillTypeMessage : BoundUserInterfaceMessage
    {
        public readonly uint PillType;

        public ChemMasterSetPillTypeMessage(uint pillType)
        {
            PillType = pillType;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterReagentAmountButtonMessage : BoundUserInterfaceMessage
    {
        public readonly ReagentId ReagentId;
        public readonly ChemMasterReagentAmount Amount;
        public readonly bool FromBuffer;

        public ChemMasterReagentAmountButtonMessage(ReagentId reagentId, ChemMasterReagentAmount amount, bool fromBuffer)
        {
            ReagentId = reagentId;
            Amount = amount;
            FromBuffer = fromBuffer;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterCreatePillsMessage : BoundUserInterfaceMessage
    {
        public readonly uint Dosage;
        public readonly uint Number;
        public readonly string Label;

        public ChemMasterCreatePillsMessage(uint dosage, uint number, string label)
        {
            Dosage = dosage;
            Number = number;
            Label = label;
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterOutputToBottleMessage : BoundUserInterfaceMessage
    {
        public readonly uint Dosage;
        public readonly string Label;

        public ChemMasterOutputToBottleMessage(uint dosage, string label)
        {
            Dosage = dosage;
            Label = label;
        }
    }

    //rayten-start
    [Serializable, NetSerializable]
    public sealed class ChemMasterCreateMedipensMessage : BoundUserInterfaceMessage
    {
        public readonly uint Dosage;
        public readonly uint Number;
        public readonly string Label;

        public ChemMasterCreateMedipensMessage(uint dosage, uint number, string label)
        {
            Dosage = dosage;
            Number = number;
            Label = label;
        }
    }
    //rayten-end

    public enum ChemMasterMode
    {
        Transfer,
        Discard,
    }

    public enum ChemMasterSortingType : byte
    {
        None = 0,
        Alphabetical = 1,
        Quantity = 2,
        Latest = 3,
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterSortingTypeCycleMessage : BoundUserInterfaceMessage;


    public enum ChemMasterReagentAmount
    {
        U1 = 1,
        U5 = 5,
        U10 = 10,
        U15 = 15,
        U20 = 20,
        U25 = 25,
        U30 = 30,
        U50 = 50,
        U100 = 100,
        All,
    }

    public static class ChemMasterReagentAmountToFixedPoint
    {
        public static FixedPoint2 GetFixedPoint(this ChemMasterReagentAmount amount)
        {
            if (amount == ChemMasterReagentAmount.All)
                return FixedPoint2.MaxValue;
            else
                return FixedPoint2.New((int)amount);
        }
    }

    /// <summary>
    /// Information about the capacity and contents of a container for display in the UI
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class ContainerInfo
    {
        /// <summary>
        /// The container name to show to the player
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// The currently used volume of the container
        /// </summary>
        public readonly FixedPoint2 CurrentVolume;

        /// <summary>
        /// The maximum volume of the container
        /// </summary>
        public readonly FixedPoint2 MaxVolume;

        /// <summary>
        /// A list of the entities and their sizes within the container
        /// </summary>
        public List<(string Id, FixedPoint2 Quantity)>? Entities { get; init; }

        public List<ReagentQuantity>? Reagents { get; init; }

        //rayten-start
        public bool ContainsOnlyPills { get; init; }
        public bool ContainsOnlyMedipens { get; init; }

        public ContainerInfo(string displayName, FixedPoint2 currentVolume, FixedPoint2 maxVolume, bool containsOnlyPills = false,
        bool containsOnlyMedipens = false)
        //rayten-end
        {
            DisplayName = displayName;
            CurrentVolume = currentVolume;
            MaxVolume = maxVolume;
            //rayten-start
            ContainsOnlyPills = containsOnlyPills;
            ContainsOnlyMedipens = containsOnlyMedipens;
            //rayten-end
        }
    }

    [Serializable, NetSerializable]
    public sealed class ChemMasterBoundUserInterfaceState : BoundUserInterfaceState
    {
        public readonly ContainerInfo? InputContainerInfo;
        public readonly ContainerInfo? OutputContainerInfo;

        /// <summary>
        /// A list of the reagents and their amounts within the buffer, if applicable.
        /// </summary>
        public readonly IReadOnlyList<ReagentQuantity> BufferReagents;

        public readonly ChemMasterMode Mode;

        public readonly ChemMasterSortingType SortingType;

        public readonly FixedPoint2? BufferCurrentVolume;
        public readonly uint SelectedPillType;

        public readonly uint PillDosageLimit;

        public readonly uint MedipenDosageLimit; //rayten

        public readonly bool UpdateLabel;

        //rayten-start
        public readonly bool ContainsOnlyPills;
        public readonly bool ContainsOnlyMedipens;
        //rayten-end

        public ChemMasterBoundUserInterfaceState(
            ChemMasterMode mode, ChemMasterSortingType sortingType, ContainerInfo? inputContainerInfo, ContainerInfo? outputContainerInfo,
            IReadOnlyList<ReagentQuantity> bufferReagents, FixedPoint2 bufferCurrentVolume,
            //rayten-start
            uint selectedPillType, uint pillDosageLimit, uint medipenDosageLimit, bool updateLabel, bool containsOnlyPills, bool containsOnlyMedipens)
        //rayten-end
        {
            InputContainerInfo = inputContainerInfo;
            OutputContainerInfo = outputContainerInfo;
            BufferReagents = bufferReagents;
            Mode = mode;
            SortingType = sortingType;
            BufferCurrentVolume = bufferCurrentVolume;
            SelectedPillType = selectedPillType;
            PillDosageLimit = pillDosageLimit;
            MedipenDosageLimit = medipenDosageLimit; //rayten
            UpdateLabel = updateLabel;
            //rayten-start
            ContainsOnlyPills = containsOnlyPills;
            ContainsOnlyMedipens = containsOnlyMedipens;
            //rayten-end
        }
    }

    //rayten-start
    [Serializable, NetSerializable]
    public enum ChemMasterVisuals : byte
    {
        IsInserting,
        InsertingColor
    }
    //rayten-end

    [Serializable, NetSerializable]
    public enum ChemMasterUiKey
    {
        Key
    }
}
