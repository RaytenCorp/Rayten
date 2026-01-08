## UI

injector-volume-transfer-label = Объём: [color=white]{$currentVolume}/{$totalVolume}ед. [/color]
    Режим: [color=white]{$modeString}[/color] ([color=white]{$transferVolume}ед.[/color])
injector-volume-label = Объём: [color=white]{$currentVolume}/{$totalVolume}ед.[/color]
    Режим: [color=white]{$modeString}[/color]
injector-toggle-verb-text = Переключить режим

## Entity

injector-component-inject-mode-name = введение
injector-component-draw-mode-name = забор
injector-component-dynamic-mode-name = динамический
injector-component-mode-changed-text = Теперь {$mode}
injector-component-transfer-success-message = Вы передали {$amount}ед. {$target}.
injector-component-transfer-success-message-self = Вы передали {$amount}u себе.
injector-component-inject-success-message = Вы ввели {$amount}u в {$target}!
injector-component-inject-success-message-self = Вы ввели {$amount}u себе!
injector-component-draw-success-message = Вы забрали {$amount}u из {$target}.
injector-component-draw-success-message-self = Вы забрали {$amount}u из себя.

## Сообщения об ошибках

injector-component-target-already-full-message = {CAPITALIZE($target)} уже заполнен!
injector-component-target-already-full-message-self = Вы уже полны!
injector-component-target-is-empty-message = {CAPITALIZE($target)} пуст!
injector-component-target-is-empty-message-self = Вы пусты!
injector-component-cannot-toggle-draw-message = Слишком полон для забора!
injector-component-cannot-toggle-inject-message = Нет чего вводить!
injector-component-cannot-toggle-dynamic-message = Невозможно переключить динамический режим!
injector-component-empty-message = {CAPITALIZE($injector)} пуст!
injector-component-blocked-user = Защитное снаряжение заблокировало вашу инъекцию!
injector-component-blocked-other = Броня {CAPITALIZE(POSS-ADJ($target))} заблокировала инъекцию {$user}!
injector-component-cannot-transfer-message = Вы не можете передать в {$target}!
injector-component-cannot-transfer-message-self = Вы не можете передать себе!
injector-component-cannot-inject-message = Вы не можете ввести в {$target}!
injector-component-cannot-inject-message-self = Вы не можете ввести себе!
injector-component-cannot-draw-message = Вы не можете забирать из {$target}!
injector-component-cannot-draw-message-self = Вы не можете забирать из себя!
injector-component-ignore-mobs = Этот инжектор может взаимодействовать только с контейнерами!

## Сообщения DoAfter для мобов

injector-component-needle-injecting-user = Вы начинаете вводить иглу.
injector-component-needle-injecting-target = {CAPITALIZE($user)} пытается ввести иглу в вас!
injector-component-needle-drawing-user = Вы начинаете забор иглой.
injector-component-needle-drawing-target = {CAPITALIZE($user)} пытается забрать с вас жидкость иглой!
injector-component-spray-injecting-user = Вы начинаете подготавливать инжектор.
injector-component-spray-injecting-target = {CAPITALIZE($user)} пытается поставить инжектор на вас!

## Popup-успех для цели
injector-component-feel-prick-message = Вы чувствуете лёгкий укол!
