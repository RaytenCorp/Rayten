using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Research;

[Serializable, NetSerializable]
public enum ParaxitExtractorUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class ParaxitExtractorBoundUserInterfaceState : BoundUserInterfaceState
{
    public bool CanExtract;
    public int PointCost;
    public int ServerAdvancedPoints;

    public ParaxitExtractorBoundUserInterfaceState(int serverAdvancedPoints, int pointCost, bool canExtract)
    {
        CanExtract = canExtract;
        PointCost = pointCost;
        ServerAdvancedPoints = serverAdvancedPoints;
    }
}

[Serializable, NetSerializable]
public sealed class ParaxitExtractorExtractMessage : BoundUserInterfaceMessage
{

}