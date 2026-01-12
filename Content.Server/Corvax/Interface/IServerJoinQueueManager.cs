namespace Content.Server.Corvax.Interface;

public interface IServerJoinQueueManager
{
    public bool IsEnabled { get; }
    public int PlayerInQueueCount { get; }
    public int ActualPlayersCount { get; }
    public void Initialize();
    public void PostInitialize();
}
