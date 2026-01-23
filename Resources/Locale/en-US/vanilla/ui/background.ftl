background-ui-SkillsLabel-prefix = [color=#00d0ff][bold]Навыки:[/bold][/color] { $skills }
background-ui-EasySkills = [color={ $skilltype ->
        [Piloting] #85490c
        [Botany] #6db33f
        [MusInstruments] #355f44
        [Bureaucracy] #939794
        [Research] #c02dc8
       *[other] white
    }]+ { $skilltype ->
        [Piloting] Пилотирование
        [Botany] Ботаника
        [MusInstruments] Муз. инструменты
        [Bureaucracy] Бюрократия
        [Research] Исследование
       *[other] ???
    }[/color]
background-ui-Skills = [color={ $skilltype ->
        [Weapon] #85490c
        [Medicine] #005b53
        [Engineering] #ff6600
       *[other] white
    }]{ $skilltype ->
        [Weapon] Боевое оружие
        [Medicine] Медицина
        [Engineering] Инженерия
       *[other] ээ...? Что за?
    }[/color]: { $lvl }
background-ui-specials-header = [color=gold][bold]Особенности:[/bold][/color]
background-ui-SkillPoints = [color=#0073ff] • { $count } очков навыка[/color]
#ui rolebackground
rolebackground-ui-SkillPoints = [color=gold][bold]Свободных очков навыка:[/bold][/color] { $count }
rolebackground-ui-selectedbackgrounds-header = [bold][color=#0ec7ec]Выбранные предыстории:[/color][/bold]
rolebackground-ui-selectedbackgrounds-item = - { $name }
background-window = Навыки
