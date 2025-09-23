tdm-preset-title = Командный дезматч
tdm-preset-description = Убей КРАСНЫХ! Если ты синий... ну и наоборот иначе.
tdm-firstblood = { $player } проливает первую кровь убив { $victim }!
tdm-announcer = Коментирует Ванилька
tdm-killstreak =
    { $player } убил { $victim }.
    { $streak ->
        [5] БУЙСТВО! КТО-НИБУДЬ ОСТАНОВИТЕ ЕГО! КТО-НИБУДЬ ВООБЩЕ ЖИВ?!
        [4] СЕРИЯ УБИЙСТВ! 4 ЖЕРТВА НА ЕГО СЧЕТУ!
        [3] Серия убийств! Тройное убийство!
        [2] Двойное убийство!
       *[1] { "" }
    }
tdm-gameover =
    Бой окончен!  { $winner ->
        [false] Победа синей команды!
        [true] Победа красной команды!
       *[other] Ничья :c
    }
    { $result }
TDM-NotAvailable = TDM
TDM-Available = TDM { $blueguys } VS { $redguys } ({ $timer})
accept-TDM-window-title = Приглашение в TDM
accept-TDM-window-prompt-text-part = TDM будет начат через 30 секунд, хотите принять участие?
accept-TDM-window-accept-button = Да!
accept-TDM-window-deny-button = Нет

## TTT

TTT-NotAvailable = TTT
TTT-Available = TTT { $players }/4 ({ $timer})

ttt-traitor-brief = [color={ $color }][font size=16]Вы Предатель.[/font] 
    Убейте невиновных!
    {"[bold]Вы не можете убивать других предателей[/bold].[/color]"}
ttt-detective-brief = [color={ $color }][font size=16]Вы Детектив.[/font]
    Вы должны убить предателей. 
    {"[bold]Вы не можете убивать просто так[/bold].[/color]"}
ttt-innocent-brief = [color={ $color }][font size=16]Вы Невиновный.[/font]
    Вы должны убить предателей. 
    {"[bold]Вы не можете убивать просто так[/bold].[/color]"}
ttt-awaitrole-brief = [font size=16][color=#7F00FF]Роли будут выданы через 30 секунд![/color][/font]
    Вооружайтесь и кооперируйтесь, но будьте бдительны, ведь кому-то достанется роль [color=#FF0000]предателя[/color].

ttt-timetoend-5 = Осталось 5 минут до конца игры!
ttt-timetoend-3 = Осталось 3 минуты до конца игры!
ttt-timetoend-1 = Осталась 1 минута до конца игры!
ttt-gameover = [font="Monospace" size= 20]{ $winner ->
        [false] Победа НЕВИНОВНЫХ!
        [true] Победа ПРЕДАТЕЛЕЙ!
       *[other] Ничья :c
    }[/font]
    { $result }
