## Survivor

roles-antag-survivor-name = Выживший
# Это ссылка на Halo
roles-antag-survivor-objective = Текущая цель: Выжить
survivor-role-greeting =
    Вы — Выживший.
    Главное — вернуться на ЦЕНТКОМ живым.
    Соберите как можно больше оружия, чтобы гарантировать своё выживание.
    Никому не доверяйте.
survivor-round-end-dead-count =
    { $deadCount ->
        [one] [color=red]{ $deadCount }[/color] выживший погиб.
       *[other] [color=red]{ $deadCount }[/color] выживших погибло.
    }
survivor-round-end-alive-count =
    { $aliveCount ->
        [one] [color=yellow]{ $aliveCount }[/color] выживший остался на станции.
       *[other] [color=yellow]{ $aliveCount }[/color] выживших остались на станции.
    }
survivor-round-end-alive-on-shuttle-count =
    { $aliveCount ->
        [one] [color=green]{ $aliveCount }[/color] выживший покинул станцию живым.
       *[other] [color=green]{ $aliveCount }[/color] выживших покинули станцию живыми.
    }

## Wizard

objective-issuer-swf = [color=turquoise]Конфедерация Магов[/color]
wizard-title = Волшебник
wizard-description = На станции Волшебник! Никогда не знаешь, что они могут сделать.
roles-antag-wizard-name = Волшебник
roles-antag-wizard-objective = Дать им урок, который они никогда не забудут.
wizard-role-greeting =
    ТЫ — ВОЛШЕБНИК!
    Между Конфедерацией Магов и NanoTrasen возникли напряжённости.
    Поэтому тебя выбрала Конфедерацией Магов для визита на станцию.
    Дай им хорошее демонстрационное шоу твоих сил.
    Что ты будешь делать — решать тебе, только помни, что Конфедерацией Магов хочет, чтобы ты вышел живым.
wizard-round-end-name = Волшебник

## TODO: Wizard Apprentice (Coming sometime post-wizard release)

