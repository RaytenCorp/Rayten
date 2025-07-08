# Commands


## Delay shuttle round end

cmd-delayroundend-desc = Останавливает таймер, который завершает раунд при выходе аварийного шаттла из гиперпространства.
cmd-delayroundend-help = Использование: delayroundend
emergency-shuttle-command-round-yes = Таймер остановлен
emergency-shuttle-command-round-no = Невозможно остановить таймер
cmd-dockemergencyshuttle-desc = Вызывает аварийный шаттл и стыкует его со станцией... если это возможно.
cmd-dockemergencyshuttle-help = Использование: dockemergencyshuttle

## Dock emergency shuttle

cmd-launchemergencyshuttle-desc = Ранний запуск аварийного шаттла, если это возможно.
cmd-launchemergencyshuttle-help = Использование: launchemergencyshuttle

## Launch emergency shuttle

# Emergency shuttle
emergency-shuttle-left = Эвакуационный шаттл покинул станцию. Расчётное время прибытия шаттла на станцию Центкома - { $transitTime } секунд.
emergency-shuttle-launch-time = Эвакуационный шаттл будет запущен через { $consoleAccumulator } секунд.
emergency-shuttle-docked = Эвакуационный шаттл пристыковался к станции { $location }, направление: { $direction }. Он улетит через { $time } секунд.{ $extended }
emergency-shuttle-good-luck = Эвакуационный шаттл не может найти станцию. Удачи.
emergency-shuttle-nearby = Эвакуационный шаттл не может найти подходящий стыковочный шлюз. Он дрейфует около станции, { $location }, направление: { $direction }. Он улетит через { $time } секунд.{ $extended }
emergency-shuttle-extended = { " " }Время до запуска было продлено в связи с непредвиденными обстоятельствами.
# Emergency shuttle console popup / announcement
emergency-shuttle-console-no-early-launches = Досрочный запуск отключён
emergency-shuttle-console-auth-left =
    { $remaining } { $remaining ->
        [one] авторизация осталась
        [few] авторизации остались
       *[other] авторизаций осталось
    } для досрочного запуска шаттла.
emergency-shuttle-console-auth-revoked =
    Авторизации на досрочный запуск шаттла отозваны, { $remaining } { $remaining ->
        [one] авторизация необходима
        [few] авторизации необходимы
       *[other] авторизаций необходимо
    }.
emergency-shuttle-console-denied = Доступ запрещён
# UI
emergency-shuttle-console-window-title = Консоль эвакуационного шаттла
emergency-shuttle-ui-engines = ДВИГАТЕЛИ:
emergency-shuttle-ui-idle = Простой
emergency-shuttle-ui-repeal-all = Отменить все
emergency-shuttle-ui-early-authorize = Разрешение на досрочный запуск
emergency-shuttle-ui-authorize = АВТОРИЗАЦИЯ
emergency-shuttle-ui-repeal = ОТМЕНА
emergency-shuttle-ui-authorizations = Авторизации
emergency-shuttle-ui-remaining = Осталось: { $remaining }
# Map Misc.
map-name-centcomm = Центральное командование
map-name-terminal = Терминал прибытия
