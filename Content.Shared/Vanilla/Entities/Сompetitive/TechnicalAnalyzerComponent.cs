using Robust.Shared.Serialization;
using Robust.Shared.GameStates;
using Robust.Shared.Audio;
namespace Content.Shared.Vanilla.Competitive;

[RegisterComponent, NetworkedComponent]
public sealed partial class TechnicalAnalyzerComponent : Component
{
    [DataField]
    public ContrabandAnalysisData? CurrentAnalysisData = null;
    [DataField]
    public int ResearchPoints = 0;
    [DataField]
    public SoundSpecifier? ExtractSound = new SoundPathSpecifier("/Audio/Effects/radpulse11.ogg");
    [DataField]
    public SoundSpecifier? WinSound = new SoundPathSpecifier("/Audio/Machines/scan_finish.ogg");
}


[Serializable, NetSerializable]
public enum TechnicalAnalyzerUiKey : byte
{
    Key
}


[Serializable, NetSerializable]
public sealed class TechnicalAnalyzerInterfaceState : BoundUserInterfaceState
{
    public List<List<CodonFeedBack>> History { get; }
    public int AttemptsCount { get; }
    public int ResearchPoints { get; }

    public TechnicalAnalyzerInterfaceState(
        List<List<CodonFeedBack>> history,
        int attemptsCount,
        int researchPoints)
    {
        History = history;
        AttemptsCount = attemptsCount;
        ResearchPoints = researchPoints;
    }
}



[Serializable, NetSerializable]
public sealed class TechnicalAnalyzerButtonPressedMessage : BoundUserInterfaceMessage
{
    public List<char> SubmittedGenome { get; }

    public TechnicalAnalyzerButtonPressedMessage(List<char> submittedGenome)
    {
        SubmittedGenome = submittedGenome;
    }
}
[Serializable, NetSerializable]
public sealed class TechnicalAnalyzerExtractMessage : BoundUserInterfaceMessage
{

}
