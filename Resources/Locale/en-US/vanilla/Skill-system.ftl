Skill-issue-message-medicine-unskilled = Требуется { $lvl } ур. медицины
Skill-issue-message-chemistry-unskilled = Требуется { $lvl } ур. химии
Skill-issue-message-rangeweapon-unskilled = Требуется { $lvl } ур. стрельбы
Skill-issue-message-research-unskilled = Требуется { $lvl } ур. исследования
Skill-issue-message-instrumentation-unskilled = Требуется { $lvl } ур. приборостроения
Skill-issue-message-building-unskilled = Требуется { $lvl } ур. строительства
Skill-issue-message-engineering-unskilled = Требуется { $lvl } ур. инженерии
Skill-issue-easyskill-message-piloting-unskilled = Требуется навык пилотирования.
Skill-issue-easyskill-message-botany-unskilled = Требуется навык ботаники.
Skill-issue-easyskill-message-musinstruments-unskilled = Требуется навык музыкальных инструментов.
Skill-issue-easyskill-message-bureaucracy-unskilled = Требуется навык бюрократии.
Skill-issue-easyskill-message-atmosphere-unskilled = Требуется навык атмосферы.
Skill-issue-easyskill-message-crime-unskilled = Требуется навык преступности.
shared-solution-container-component-on-examine-main-text-skill-issue = { "" }
construction-menu-skill-building = [color=#FFBF00]Требуется { $lvl } ур. строительства[/color]
construction-menu-skill-engineering = [color=#ff6600]Требуется { $lvl } ур. инженерии[/color]
construction-menu-skill-atmosphere = [color=#4ebed7]Требуется знание атмосферы[/color]
Skill-train-overtrain-medicine = Ваш навык медицины больше, чем может дать эта книга.
Skill-train-overtrain-chemistry = Ваш навык химии больше, чем может дать эта книга.
Skill-train-overtrain-rangeweapon = Ваш навык стрельбы больше, чем может дать эта книга.
Skill-train-overtrain-research = Ваш навык исследований больше, чем может дать эта книга.
Skill-train-overtrain-instrumentation = Ваш навык приборостроения больше, чем может дать эта книга.
Skill-train-overtrain-building = Ваш навык строительства больше, чем может дать эта книга.
Skill-train-overtrain-engineering = Ваш навык инженерии больше, чем может дать эта книга.
Skill-train-overtrain-piloting = Ваш навык пилотирования больше, чем может дать эта книга.
Skill-train-overtrain-botany = Ваш навык ботаники больше, чем может дать эта книга.
Skill-train-overtrain-musinstruments = Ваш навык муз. инструментов больше, чем может дать эта книга.
Skill-train-overtrain-bureaucracy = Ваш навык бюрократии больше, чем может дать эта книга.
Skill-train-overtrain-atmosphere = Ваш навык атмосферы больше, чем может дать эта книга.
Skill-train-overtrain-crime = Ваш навык преступности больше, чем может дать эта книга.
skill-system-bonusskillpoints-message = Вы получили { $skillpoints } очков навыков, т.к. было менее 10 готовых игроков.
examine-skilltrainer-part-1 =
    Повышает навык [color={ $skilltype ->
        [Piloting] #85490c
        [RangeWeapon] #8f2121
        [MeleeWeapon] #8f2121
        [Medicine] #005b53
        [Chemistry] #AD4915
        [Engineering] #ff6600
        [Building] #FFBF00
        [Research] #7c0183
        [Instrumentation] #b03bd0
        [Botany] #6db33f
        [MusInstruments] #355f44
        [Bureaucracy] #939794
        [Atmosphere] #4ebed7
        [Crime] #ff0000
       *[other] white
    }]{ $skilltype ->
        [Piloting] пилотирования
        [RangeWeapon] стрельбы
        [MeleeWeapon] ближнего боя
        [Medicine] медицины
        [Chemistry] химии
        [Engineering] инженерии
        [Building] строительства
        [Research] исследований
        [Instrumentation] Приборостроения
        [Botany] Ботаники
        [MusInstruments] Муз. инструментов
        [Bureaucracy] Бюрократии
        [Atmosphere] Атмосферы
        [Crime] Преступности
       *[other] ???
    }[/color].
examine-skilltrainer-part-2 = доступно { $SkillExpToLearn } опыта.
skill-system-UI-SkillNameLabel = [color=#EFBF04]{ $skillname ->
        [Piloting] Пилотирование
        [RangeWeapon] Стрельба
        [MeleeWeapon] Ближний бой
        [Medicine] Медицина
        [Chemistry] Химия
        [Engineering] Инженерия
        [Building] Строительство
        [Research] Исследование
        [Instrumentation] Приборостроение
        [Botany] Ботаника
        [MusInstruments] Муз. инструменты
        [Bureaucracy] Бюрократия
        [Atmosphere] Атмосфера
        [Crime] Преступность
       *[other] ВТФ?
    }[/color]
skill-system-UI-Lvllabel = Ур. [color={ $color }]{ $lvl }[/color] [color=red]{ $amnesia }[/color]
skill-system-UI-ExpLabel = { $exp }/600[color=red]{ $amnesia }[/color]
skill-system-UI-ExpLabel-maxlvl = Макс.
skill-system-UI-Easy-SkillInfoLabel = Опыт: { $exp }/600
skill-system-UI-Easy-SkillInfoLabel-Have = Изучен.
skill-system-UI-Easy-AmnesiaLabel = [color=red]Амнезия: { $exptorestore }[/color]
skill-system-UI-Tooltip-RangeWeapon = Данный навык отвечает за разброс огнестрельного оружия.
skill-system-UI-Tooltip-MeleeWeapon =
    Данный навык отвечает за урон от ближнего боя, толкание, 
    заковывание в наручники, и длительность оглушения.
skill-system-UI-Tooltip-Medicine =
    Данный навык отвечает за скорость лечения, 
    импланты, шприцы, капсулу клонирования, криокапсулу.
skill-system-UI-Tooltip-Chemistry =
    Данный навык отвечает за использования химического оборудования 
    и возможность распознавания хим. веществ.
skill-system-UI-Tooltip-Engineering =
    Данный навык отвечает за взлом, 
    взаимодействие с предметами атмоса и элекроэнергии.
skill-system-UI-Tooltip-Building =
    Данный навык отвечает за скорость работы с инструментами, 
    сборку и разборку стен и окон.
skill-system-UI-Tooltip-Research =
    Данный навык отвечает за обучаемость другим навыкам, 
    а также изучение аномалий и артефактов.
skill-system-UI-Tooltip-Instrumentation =
    Данный навык отвечает за крафт различных приборов, предметов, 
    киборгов, экзоскелетов и т.д., а также за возможность их разборки.
skill-system-UI-Tooltip-Piloting = Данный навык отвечает за возможность управления шаттлом
skill-system-UI-Tooltip-Botany =
    Данный навык отвечает за возможность скрещивания растений, 
    использование секатора, биофабрикатора и анализатора растений
skill-system-UI-Tooltip-Bureaucracy = Данный навык позволяет писать отчёты приказы запросы и прочие бюрократические документы.
skill-system-UI-Tooltip-Atmosphere = Данный навык отвечает за возможность прокладывания труб и взаимодействие с атмосферными приборами.
skill-system-UI-Tooltip-Crime = будет добавлен в ближайшем патче.
