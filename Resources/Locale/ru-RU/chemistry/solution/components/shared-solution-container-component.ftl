shared-solution-container-component-on-examine-main-text = Содержит [color={ $color }]{ $desc }[/color] { $wordedAmount }
examinable-solution-recognized = [color={ $color }]{ $chemical }[/color]
examinable-solution-on-examine-volume = Содержимое раствора { $fillLevel ->
    [exact] содержит [color=white]{$current}/{$max}u[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-no-max = Содержимое раствора { $fillLevel ->
    [exact] содержит [color=white]{$current}u[/color].
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-puddle =
    Лужа { $fillLevel ->
        [exact] [color=white]{ $current }u[/color].
        [full] огромная и переполняющаяся!
        [mostlyfull] огромная и переполняющаяся!
        [halffull] глубокая и текущая.
        [halfempty] очень глубокая.
       *[mostlyempty] собирается в лужицы.
        [empty] образует несколько маленьких лужиц.
    }
-solution-vague-fill-level =
    { $fillLevel ->
        [full] [color=white]Полный[/color]
        [mostlyfull] [color=#DFDFDF]Почти полный[/color]
        [halffull] [color=#C8C8C8]Наполовину полный[/color]
        [halfempty] [color=#C8C8C8]Наполовину пустой[/color]
        [mostlyempty] [color=#A4A4A4]Почти пустой[/color]
       *[empty] [color=gray]Пустой[/color]
    }
examinable-solution-has-recognizable-chemicals = В этом растворе вы можете распознать { $recognizedString }.
