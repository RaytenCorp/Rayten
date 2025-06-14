## Entity

chem-master-component-activate-no-hands = У вас нет рук.
chem-master-component-cannot-put-entity-message = Вы не можете поместить это в ХимМастер!

## Bound UI

chem-master-bound-user-interface-title = ХимМастер 4000

## UI

chem-master-window-input-tab = Вход
chem-master-window-output-tab = Выход
chem-master-window-container-label = Контейнер
chem-master-window-eject-button = Извлечь
chem-master-window-no-container-loaded-text = Контейнер не загружен.
chem-master-window-buffer-text = Буфер
chem-master-window-buffer-label = буфер:
chem-master-window-buffer-all-amount = Всё
chem-master-window-buffer-empty-text = Буфер пуст.
chem-master-window-buffer-low-text = Недостаточно раствора в буфере
chem-master-window-transfer-button = Перенести
chem-master-window-discard-button = Уничтожить
chem-master-window-packaging-text = Упаковка
chem-master-current-text-label = Метка:
chem-master-window-pills-label = Таблетка:
chem-master-window-pill-type-label = Тип таблеток:

## rayten-start

chem-master-window-pills-number-label = Кол-во:
chem-master-window-medipens-label = Авто-инъектор:

## rayten-ends

chem-master-window-medipens-number-label = Кол-во:
chem-master-window-dose-label = Дозировка (ед.):
chem-master-window-create-button = Создать
chem-master-window-bottles-label = Бутылочки:
chem-master-window-unknown-reagent-text = Неизвестный реагент
chem-master-window-sort-type-none = Сортировать по: Самому старому первому
chem-master-window-sort-type-alphabetical = Сортировать по: Алфавиту
chem-master-window-sort-type-quantity = Сортировать по: Количеству
chem-master-window-sort-type-latest = Сортировать по: Недавнему первому

## rayten-start

chem-master-window-tooltip-display = { $amount } { $material }
chem-master-window-material-amount =
    { $required ->
        [1] { NATURALFIXED($amount, 2) } ({ $unit })
       *[other] { NATURALFIXED($required, 2) } ({ $unit })
    }
chem-master-window-material-amount-missing =
    { $required ->
        [1] { NATURALFIXED($required, 2) } { $unit } { $material } ([color=red]{ NATURALFIXED($missing, 2) } { $unit } не хватает[/color])
       *[other] { NATURALFIXED($required, 2) } { $unit } { $material } ([color=red]{ NATURALFIXED($missing, 2) } { $unit } не хватает[/color])
    }

## rayten-end

