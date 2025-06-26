TechnicalAnalysis-ui-status-win = [bold]Статус: [/bold] [color=Green] Расшифровано [/color]
TechnicalAnalysis-ui-status-notwin = [bold]Статус: [/bold] [color=Red] Не расшифровано [/color]

TechnicalAnalysis-ui-attempts = [bold]Осталось попыток:[/bold] [color={$count ->
    [5]#00FF00
    [4]#66FF00
    [3]#FFFF00
    [2]#FF9900
    [1]#FF3300
    [0]#808080
    *[other]#FFFFFF
    }]{ $count }[/color]


TechnicalAnalysis-ui-itemname = [bold]Предмет:[/bold] { $item }
TechnicalAnalysis-ui-difficult = [bold]Сложность:[/bold] { $difficult ->
        [easy] [color=Green]Лёгкая[/color]
        [medium] [color=Red]Средняя[/color]
        [hard] [color=Red]Высокая[/color]
       *[other] [color=Orange]Неизвестна[/color]
    }
TechnicalAnalysis-ui-researchpoints-count = [bold]Очков изучения:[/bold] { $points }
TechnicalAnalysis-ui-server-connected =  [bold]Статус:[/bold] [color=Green]Сервер подключен.[/color]
TechnicalAnalysis-ui-server-notconnected =  [bold]Статус:[/bold] [color=Red]Сервер не подключен.[/color]

contraband-analyzer-analysis = АНАЛИЗ.
contraband-analyzer-analysis-noserver = Сервер не найден.
contraband-analyzer-analysis-NotContraband = Предмет не является вражеским снаряжением.
contraband-analyzer-analysis-error = ОШИБКА.
contraband-analyzer-analysis-accept = Обнаружены свойства вражеского снаряжения.
contraband-analyzer-radio-message = Отправлены новые данные для изучения. Проверьте А.К.Т.
stamp-component-stamped-name-contraband-analyzer = Анализатор контрабанды
contraband-analyzer-paper-content = [color=#DE3A3A]█▄ █ ▀█▀    [head=2]Анализ контрабанды[/head]
    █ ▀█     █       [italic]Слава NanoTrasen![/italic]
    ────────[bold]{ $station }[/bold]─────────[/color]
    Предмет: { $actualname }
    Свойства:[color=#D381C9] { $hiddendesc } [/color]
    Результат анализа: { $enemyTechnology ->
    [true] [color=Red]Обнаружены свойства вражеского снаряжения[/color]
    [false] [color=Orange]Обнаружены свойства контрабанды[/color]
    *[other] [color=Orange]Результат не определён[/color]
    }
    ──────────────────────────────────────────
    Отпечтки пальцев: {$fingerprints}
    Волокна: { $fibers }
    ДНК: { $touchDNAs }
    Остатки: { $residues }
    ──────────────────────────────────────────
                                место для печатей
