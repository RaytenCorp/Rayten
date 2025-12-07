using Robust.Shared.GameStates;
namespace Content.Shared.Vanilla.Entities.DangerScanner;
/// <summary>
/// Компонент отмечает чубрика как отсканированного (чтобы его не сканировали повторно)
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DangerScannedComponent : Component
{
    /// <summary>
    /// следующий скан будет доступен в
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan NextScanIn = TimeSpan.Zero;
    /// <summary>
    /// куллдаун на новый скан
    /// </summary>
    public const float ScanCoolDown = 10f; //в минутах
}
