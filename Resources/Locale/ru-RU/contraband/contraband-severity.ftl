contraband-examine-text-FirstLevel = [color=yellow]Это предмет [bold]1[/bold] класса опасности.[/color]
contraband-examine-text-SecondLevel = [color=orange]Это предмет [bold]2[/bold] класса опасности.[/color]
contraband-examine-text-ThirdLevel = [color=red]Это предмет [bold]3[/bold] класса опасности.[/color]
contraband-examine-text-Restricted-department =
    { $type ->
        *[item] [color=yellow]Этот предмет может носить: { $departments }[/color]
        [reagent] [color=yellow]Оборот этого вещества разрешен только: { $departments }[/color]
    }
contraband-examine-text-GrandTheft = [color=red]Это особо-ценный предмет![/color]
contraband-examine-text-avoid-carrying-around = [color=red][italic]Вы [bold]не[/bold] можете носить этот предмет.[/italic][/color]
contraband-examine-text-in-the-clear = [color=green][italic]Вы можете носить этот предмет.[/italic][/color]
contraband-examinable-verb-text = Легальность
contraband-examinable-verb-message = Проверить легальность этого предмета.
contraband-department-plural = { $department }
contraband-job-plural = { $job }
