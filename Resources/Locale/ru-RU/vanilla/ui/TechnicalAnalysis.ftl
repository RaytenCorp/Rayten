TechnicalAnalysis-ui-status-win = [bold]Статус: [/bold] [color=Green] Расшифровано [/color]
TechnicalAnalysis-ui-status-notwin = [bold]Статус: [/bold] [color=Red] Не расшифровано [/color]
TechnicalAnalysis-ui-attempts = [bold]Осталось попыток:[/bold] [color={ $count ->
        [5] #00FF00
        [4] #66FF00
        [3] #FFFF00
        [2] #FF9900
        [1] #FF3300
        [0] #808080
       *[other] #FFFFFF
    }]{ $count }[/color]
TechnicalAnalysis-ui-researchpoints-count = [bold]Очков изучения:[/bold] { $points }
TechnicalAnalysis-ui-server-connected = [bold]Статус:[/bold] [color=Green]Сервер подключен.[/color]
TechnicalAnalysis-ui-server-notconnected = [bold]Статус:[/bold] [color=Red]Сервер не подключен.[/color]
