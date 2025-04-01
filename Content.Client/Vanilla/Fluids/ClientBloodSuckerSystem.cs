using Content.Shared.Alert;
using Content.Shared.Vanilla.BloodSucker;
using Robust.Client.Player;

namespace Content.Shared.Vanilla.BloodSucker;

public sealed class ClientBloodSuckerSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BloodSuckerComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<BloodSuckerComponent, ComponentShutdown>(OnShutdown);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var attachedEnt = _player.LocalEntity;

        if (attachedEnt == null || !TryComp<BloodSuckerComponent>(attachedEnt, out var BloodSuckerComp))
            return;

        SetBloodStorageAlert(attachedEnt.Value, BloodSuckerComp);
    }

    private void OnStartup(EntityUid uid, BloodSuckerComponent component, ComponentStartup args)
    {
        SetBloodStorageAlert(uid, component);
    }

    private void OnShutdown(EntityUid uid, BloodSuckerComponent component, ComponentShutdown args)
    {
        _alerts.ClearAlert(uid, component.BloodAlert);
    }

    private void SetBloodStorageAlert(EntityUid uid, BloodSuckerComponent component)
    {
        // Проверяем, чтобы не делить на ноль
        if (component.BloodStorage <= 0)
            return;

        // Вычисляем процент заполненности (0.0 - 1.0)
        var fillPercentage = Math.Clamp(component.AmountOfBloodInStorage / component.BloodStorage, 0f, 1f);

        // Конвертируем процент в степень алерта (0 - 4)
        var severity = (short) MathF.Round(fillPercentage * 4);

        // Показываем алерт
        _alerts.ShowAlert(uid, component.BloodAlert, severity);
    }

}
