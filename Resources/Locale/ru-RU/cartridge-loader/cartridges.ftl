device-pda-slot-component-slot-name-cartridge = Картридж
default-program-name = Программа
notekeeper-program-name = Заметки
nano-task-program-name = НаноТаск
news-read-program-name = Новости станции
crew-manifest-program-name = Манифест экипажа
crew-manifest-cartridge-loading = Загрузка...
net-probe-program-name = Зонд сетей
net-probe-scan = Просканирован { $device }!
net-probe-label-name = Название
net-probe-label-address = Адрес
net-probe-label-frequency = Частота
net-probe-label-network = Сеть
log-probe-program-name = Зонд логов
log-probe-scan = Загружены логи устройства { $device }!
log-probe-label-time = Время
log-probe-label-accessor = Использовано:
log-probe-label-number = #
log-probe-print-button = Распечатать логи
log-probe-printout-device = Сканировано: { $name }
log-probe-printout-header = Последние логи:
log-probe-printout-entry = #{ $number } / { $time } / { $accessor }
astro-nav-program-name = АстроНав
med-tek-program-name = МедТек
# Wanted list cartridge
wanted-list-program-name = Список разыскиваемых
nano-task-ui-heading-high-priority-tasks =
    { $amount ->
        [zero] нет задач с высоким приоритетом
        [one] 1 задача с высоким приоритетом
       *[other] { $amount } задач с высоким приоритетом
    }
nano-task-ui-heading-medium-priority-tasks =
    { $amount ->
        [zero] нет задач с средним приоритетом
        [one] 1 задача с средним приоритетом
       *[other] { $amount } задач с средним приоритетом
    }
nano-task-ui-heading-low-priority-tasks =
    { $amount ->
        [zero] нет задач с низким приоритетом
        [one] 1 задача с низким приоритетом
       *[other] { $amount } задач с низким приоритетом
    }
nano-task-ui-done = Выполнено
nano-task-ui-revert-done = Отменить
nano-task-ui-priority-low = Низкий
nano-task-ui-priority-medium = Средний
nano-task-ui-priority-high = Высокий
nano-task-ui-cancel = Закрыть
nano-task-ui-print = Распечатать
nano-task-ui-delete = Удалить
nano-task-ui-save = Сохранить
nano-task-ui-new-task = Новая задача
nano-task-ui-description-label = Описание:
nano-task-ui-description-placeholder = Что-то важное...
nano-task-ui-requester-label = Запрашивающий:
nano-task-ui-requester-placeholder = Джордж Хайтауэр
nano-task-ui-item-title = Редактировать
nano-task-printed-description = Описание: { $description }
nano-task-printed-requester = Запрашивающий: { $requester }
nano-task-printed-high-priority = Приоритет: высокий
nano-task-printed-medium-priority = Приоритет: средний
nano-task-printed-low-priority = Приоритет: низкий
wanted-list-label-no-records = Всё спокойно, ковбой.
wanted-list-search-placeholder = Поиск по имени и статусу
wanted-list-age-label = [color=darkgray]Возраст:[/color] [color=white]{ $age }[/color]
wanted-list-job-label = [color=darkgray]Должность:[/color] [color=white]{ $job }[/color]
wanted-list-species-label = [color=darkgray]Раса:[/color] [color=white]{ $species }[/color]
wanted-list-gender-label = [color=darkgray]Гендер:[/color] [color=white]{ $gender }[/color]
wanted-list-reason-label = [color=darkgray]Причина:[/color] [color=white]{ $reason }[/color]
wanted-list-unknown-reason-label = неизвестная причина
wanted-list-initiator-label = [color=darkgray]Инициатор:[/color] [color=white]{ $initiator }[/color]
wanted-list-unknown-initiator-label = неизвестный инициатор
wanted-list-status-label = [color=darkgray]статус:[/color] { $status ->
        [suspected] [color=yellow]подозревается[/color]
        [wanted] [color=red]разыскивается[/color]
        [detained] [color=#b18644]под арестом[/color]
        [paroled] [color=green]освобождён по УДО[/color]
        [discharged] [color=green]освобождён[/color]
       *[other] нет
    }
wanted-list-history-table-time-col = Время
wanted-list-history-table-reason-col = Преступление
wanted-list-history-table-initiator-col = Инициатор
