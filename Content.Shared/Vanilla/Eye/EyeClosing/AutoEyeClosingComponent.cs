using Robust.Shared.Utility;

namespace Content.Shared.Eye.Blinding.Components;

/// <summary>
///     Существо будет закрывать глаза, если в N радиусе есть объект с компонентом BlockMovementOnEyeContactComponent
/// </summary>
[RegisterComponent]
public sealed partial class AutoEyeClosingComponent : Component
{
    //момент времени в который глаза будут закрыты
    [ViewVariables]
    public TimeSpan CloseEyeTime;

    //момент времени в который глаза будут открыты
    [ViewVariables]
    public TimeSpan OpenEyeTime;


    //длительность открытых глаз (в секундах)
    [DataField]
    public float OpenDuration = 6f;

    // Длительность закрытых глаз (в секундах).
    // Казалось бы, человек моргнул бы быстрее, но здесь не обычное мигание:
    // архонт подавляет автономные рефлексы, и человек залипает в "долгом моргании",
    // теряя контроль над веками. Поэтому глаза остаются закрытыми дольше нормы.
    [DataField]
    public float CloseDuration = 2.5f;
}
