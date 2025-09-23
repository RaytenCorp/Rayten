background-ui-SkillsLabel-prefix = [color=#00d0ff][bold]Навыки:[/bold][/color] { $skills }
background-ui-EasySkills = [color={ $skilltype ->
        [Piloting] #85490c
        [Botany] #6db33f
        [MusInstruments] #355f44
        [Bureaucracy] #939794
        [Atmosphere] #4ebed7
       *[other] white
    }]+ { $skilltype ->
        [Piloting] Пилотирование
        [Botany] Ботаника
        [MusInstruments] Муз. инструменты
        [Bureaucracy] Бюрократия
        [Atmosphere] Атмосфера
       *[other] ???
    }[/color]
background-ui-Skills = [color={ $skilltype ->
        [Piloting] #85490c
        [RangeWeapon] #a90000
        [MeleeWeapon] #ed4646
        [Medicine] #005b53
        [Chemistry] #AD4915
        [Engineering] #ff6600
        [Building] #FFBF00
        [Research] #c02dc8
        [Instrumentation] #b03bd0
        [Botany] #6db33f
        [MusInstruments] #355f44
        [Bureaucracy] #939794
        [Atmosphere] #4ebed7
        [Crime] #ff0000
       *[other] white
    }]{ $skilltype ->
        [Piloting] пилотирование
        [RangeWeapon] стрельба
        [MeleeWeapon] ближний бой
        [Medicine] медицина
        [Chemistry] химия
        [Engineering] инженерия
        [Building] строительство
        [Research] исследование
        [Instrumentation] Приборостроение
        [Botany] Ботаника
        [MusInstruments] Муз. инструменты
        [Bureaucracy] Бюрократия
        [Atmosphere] Атмосфера
        [Crime] Преступность
       *[other] ???
    }[/color]: { $lvl }
background-ui-specials-header = [color=gold][bold]Особенности:[/bold][/color]
background-ui-SkillPoints = [color=#0073ff] • { $count } очков навыка[/color]
#ui rolebackground
rolebackground-ui-SkillPoints = [color=gold][bold]Свободных очков навыка:[/bold][/color] { $count }
rolebackground-ui-selectedbackgrounds-header = [bold][color=#0ec7ec]Выбранные предыстории:[/color][/bold]
rolebackground-ui-selectedbackgrounds-item = - { $name }
background-window = Навыки
