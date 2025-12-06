-create-3rd-person =
    { $chance ->
        [1] Создаёт
       *[other] создать
    }
-cause-3rd-person =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    }
-satiate-3rd-person =
    { $chance ->
        [1] Утоляет
       *[other] утолить
    }
entity-effect-guidebook-spawn-entity =
    { $chance ->
        [1] Создаёт
       *[other] создать
    } { $amount ->
        [1] { INDEFINITE($entname) }
       *[other] { $amount } { MAKEPLURAL($entname) }
    }
entity-effect-guidebook-destroy =
    { $chance ->
        [1] Уничтожает
       *[other] уничтожить
    } объект
entity-effect-guidebook-break =
    { $chance ->
        [1] Ломает
       *[other] сломать
    } объект
entity-effect-guidebook-explosion =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } взрыв
entity-effect-guidebook-emp =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } электромагнитный импульс
entity-effect-guidebook-flash =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } ослепляющую вспышку
entity-effect-guidebook-foam-area =
    { $chance ->
        [1] Создаёт
       *[other] создать
    } пену
entity-effect-guidebook-smoke-area =
    { $chance ->
        [1] Создаёт
       *[other] создать
    } дым
entity-effect-guidebook-satiate-thirst =
    { $chance ->
        [1] Утоляет
       *[other] утолить
    } жажду { $relative ->
        [1] со средней скоростью
       *[other] со скоростью в { NATURALFIXED($relative, 3) }× от средней
    }
entity-effect-guidebook-satiate-hunger =
    { $chance ->
        [1] Утоляет
       *[other] утолить
    } голод { $relative ->
        [1] со средней скоростью
       *[other] со скоростью в { NATURALFIXED($relative, 3) }× от средней
    }
entity-effect-guidebook-health-change =
    { $chance ->
        [1]
            { $healsordeals ->
                [heals] Исцеляет
                [deals] Наносит
               *[both] Изменить здоровье на
            }
       *[other]
            { $healsordeals ->
                [heals] исцелить
                [deals] нанести
               *[both] изменить здоровье на
            }
    } { $changes }
entity-effect-guidebook-even-health-change =
    { $chance ->
        [1]
            { $healsordeals ->
                [heals] Равномерно исцеляет
                [deals] Равномерно наносит
               *[both] Равномерно изменяет здоровье на
            }
       *[other]
            { $healsordeals ->
                [heals] равномерно исцелить
                [deals] равномерно нанести
               *[both] равномерно изменить здоровье на
            }
    } { $changes }
entity-effect-guidebook-status-effect-old =
    { $type ->
        [update]
            { $chance ->
                [1] Продлевает эффект { LOC($key) } как минимум на { NATURALFIXED($time, 3) } { MANY("second", $time) } без накопления
               *[other] продлить эффект { LOC($key) } как минимум на { NATURALFIXED($time, 3) } { MANY("second", $time) } без накопления
            }
        [add]
            { $chance ->
                [1] Накладывает эффект { LOC($key) } как минимум на { NATURALFIXED($time, 3) } { MANY("second", $time) } с накоплением
               *[other] наложить эффект { LOC($key) } как минимум на { NATURALFIXED($time, 3) } { MANY("second", $time) } с накоплением
            }
        [set]
            { $chance ->
                [1] Вызывает эффект { LOC($key) } на { NATURALFIXED($time, 3) } { MANY("second", $time) } без накопления
               *[other] вызвать эффект { LOC($key) } на { NATURALFIXED($time, 3) } { MANY("second", $time) } без накопления
            }
       *[remove]
            { $chance ->
                [1] Удаляет { NATURALFIXED($time, 3) } { MANY("second", $time) } эффекта { LOC($key) }
               *[other] удалить { NATURALFIXED($time, 3) } { MANY("second", $time) } эффекта { LOC($key) }
            }
    }
entity-effect-guidebook-status-effect =
    { $type ->
        [update]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } { LOC($key) } как минимум на { NATURALFIXED($time, 3) } с. без накопления
        [add]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } { LOC($key) } как минимум на { NATURALFIXED($time, 3) } с. с накоплением
        [set]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } { LOC($key) } как минимум на { NATURALFIXED($time, 3) } с. без накопления
       *[remove]
            { $chance ->
                [1] Убирает
               *[other] убрать
            } { NATURALFIXED($time, 3) } с. эффекта { LOC($key) }
    } { $delay ->
        [0] немедленно
       *[other] после задержки в { NATURALFIXED($delay, 3) } { MANY("секунду", $delay) }
    }
entity-effect-guidebook-status-effect-indef =
    { $type ->
        [update]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } постоянный эффект { LOC($key) }
        [add]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } постоянный эффект { LOC($key) }
        [set]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } постоянный эффект { LOC($key) }
       *[remove]
            { $chance ->
                [1] Убирает
               *[other] убрать
            } эффект { LOC($key) }
    } { $delay ->
        [0] немедленно
       *[other] после задержки в { NATURALFIXED($delay, 3) } { MANY("секунду", $delay) }
    }
entity-effect-guidebook-knockdown =
    { $type ->
        [update]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } { LOC($key) } как минимум на { NATURALFIXED($time, 3) } с. без накопления
        [add]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } опрокидывание как минимум на { NATURALFIXED($time, 3) } с. с накоплением
       *[set]
            { $chance ->
                [1] Вызывает
               *[other] вызвать
            } опрокидывание как минимум на { NATURALFIXED($time, 3) } с. без накопления
        [remove]
            { $chance ->
                [1] Убирает
               *[other] убрать
            } { NATURALFIXED($time, 3) } с. эффекта опрокидывания
    }
entity-effect-guidebook-set-solution-temperature-effect =
    { $chance ->
        [1] Устанавливает
       *[other] установить
    } температуру раствора точно на { NATURALFIXED($temperature, 2) }K
entity-effect-guidebook-adjust-solution-temperature-effect =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Добавляет
               *[-1] Удаляет
            }
       *[other]
            { $deltasign ->
                [1] добавить
               *[-1] удалить
            }
    } тепло из раствора, пока его температура не станет { $deltasign ->
        [1] не выше { NATURALFIXED($maxtemp, 2) }K
       *[-1] не ниже { NATURALFIXED($mintemp, 2) }K
    }
entity-effect-guidebook-adjust-reagent-reagent =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Добавляет
               *[-1] Удаляет
            }
       *[other]
            { $deltasign ->
                [1] добавить
               *[-1] удалить
            }
    } { NATURALFIXED($amount, 2) }u реагента { $reagent } { $deltasign ->
        [1] в
       *[-1] из
    } раствор
entity-effect-guidebook-adjust-reagent-group =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Добавляет
               *[-1] Удаляет
            }
       *[other]
            { $deltasign ->
                [1] добавить
               *[-1] удалить
            }
    } { NATURALFIXED($amount, 2) }u реагентов из группы { $group } { $deltasign ->
        [1] в растворе
       *[-1] из раствора
    }
entity-effect-guidebook-adjust-temperature =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Добавляет
               *[-1] Удаляет
            }
       *[other]
            { $deltasign ->
                [1] добавить
               *[-1] удалить
            }
    } { POWERJOULES($amount) } тепла { $deltasign ->
        [1] в теле, в котором находится
       *[-1] из тела, в котором находится
    }
entity-effect-guidebook-chem-cause-disease =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } заболевание { $disease }
entity-effect-guidebook-chem-cause-random-disease =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } заболевания: { $diseases }
entity-effect-guidebook-jittering =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } тряску
entity-effect-guidebook-clean-bloodstream =
    { $chance ->
        [1] Очищает
       *[other] очистить
    } кровеносную систему от других химических веществ
entity-effect-guidebook-cure-disease =
    { $chance ->
        [1] Исцеляет
       *[other] исцелить
    } заболевания
entity-effect-guidebook-eye-damage =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Накладывает
               *[-1] Исцеляет
            }
       *[other]
            { $deltasign ->
                [1] наложить
               *[-1] исцелить
            }
    } повреждения глаз
entity-effect-guidebook-vomit =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } рвоту
entity-effect-guidebook-create-gas =
    { $chance ->
        [1] Создаёт
       *[other] создать
    } { $moles } { $moles ->
        [1] моль
       *[other] молей
    } газа { $gas }
entity-effect-guidebook-drunk =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } опьянение
entity-effect-guidebook-electrocute =
    { $chance ->
        [1] Поражает электричеством
       *[other] поразить электричеством
    } цель на { NATURALFIXED($time, 3) } с.
entity-effect-guidebook-emote =
    { $chance ->
        [1] Заставит
       *[other] заставить
    } цель выполнить [bold][color=white]{ $emote }[/color][/bold]
entity-effect-guidebook-extinguish-reaction =
    { $chance ->
        [1] Тушит
       *[other] потушить
    } огонь
entity-effect-guidebook-flammable-reaction =
    { $chance ->
        [1] Повышает
       *[other] повысить
    } воспламеняемость
entity-effect-guidebook-ignite =
    { $chance ->
        [1] Поджигает
       *[other] поджечь
    } цель
entity-effect-guidebook-make-sentient =
    { $chance ->
        [1] Делает
       *[other] сделать
    } цель разумным
entity-effect-guidebook-make-polymorph =
    { $chance ->
        [1] Превращает
       *[other] превратить
    } цель в { $entityname }
entity-effect-guidebook-modify-bleed-amount =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Вызывает
               *[-1] Уменьшает
            }
       *[other]
            { $deltasign ->
                [1] вызвать
               *[-1] уменьшить
            }
    } кровотечение
entity-effect-guidebook-modify-blood-level =
    { $chance ->
        [1]
            { $deltasign ->
                [1] Повышает
               *[-1] Понижает
            }
       *[other]
            { $deltasign ->
                [1] повысить
               *[-1] понизить
            }
    } уровень крови
entity-effect-guidebook-paralyze =
    { $chance ->
        [1] Парализует
       *[other] парализовать
    } цель как минимум на { NATURALFIXED($time, 3) } с.
entity-effect-guidebook-movespeed-modifier =
    { $chance ->
        [1] Изменяет
       *[other] изменить
    } скорость движения на { NATURALFIXED($sprintspeed, 3) }x как минимум на { NATURALFIXED($time, 3) } с.
entity-effect-guidebook-reset-narcolepsy =
    { $chance ->
        [1] Временно подавляет
       *[other] временно подавить
    } нарколепсию
entity-effect-guidebook-wash-cream-pie-reaction =
    { $chance ->
        [1] Смывает
       *[other] смыть
    } крем-пай с лица
entity-effect-guidebook-cure-zombie-infection =
    { $chance ->
        [1] Исцеляет
       *[other] исцелить
    } текущую зомби-инфекцию
entity-effect-guidebook-cause-zombie-infection =
    { $chance ->
        [1] Заражает
       *[other] заразить
    } цель зомби-инфекцией
entity-effect-guidebook-innoculate-zombie-infection =
    { $chance ->
        [1] Исцеляет
       *[other] исцелить
    } текущую зомби-инфекцию и даёт иммунитет к будущим заражениям
entity-effect-guidebook-reduce-rotting =
    { $chance ->
        [1] Восстанавливает
       *[other] восстановить
    } { $time } с. гниение
entity-effect-guidebook-area-reaction =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } дымовую или пенную реакцию на { $duration } с.
entity-effect-guidebook-add-to-solution-reaction =
    { $chance ->
        [1] Вызывает
       *[other] вызвать
    } добавление { $reagent } в раствор
entity-effect-guidebook-artifact-unlock =
    { $chance ->
        [1] Помогает
       *[other] помочь
    } открыть инопланетный артефакт
entity-effect-guidebook-artifact-durability-restore = Восстанавливает { $restored } прочности в активных узлах инопланетного артефакта
entity-effect-guidebook-plant-attribute =
    { $chance ->
        [1] Изменяет
       *[other] изменить
    } { $attribute } на { $positive ->
        [true] [color=red]{ $amount }[/color]
       *[false] [color=green]{ $amount }[/color]
    }
entity-effect-guidebook-plant-cryoxadone =
    { $chance ->
        [1] Возвращает
       *[other] вернуть
    } растению возраст в зависимости от возраста растения и времени роста
entity-effect-guidebook-plant-phalanximine =
    { $chance ->
        [1] Восстанавливает
       *[other] восстановить
    } жизнеспособность растения, сделанного нежизнеспособным мутацией
entity-effect-guidebook-plant-diethylamine =
    { $chance ->
        [1] Увеличивает
       *[other] увеличить
    } продолжительность жизни и/или базовое здоровье растения с вероятностью 10% для каждого
entity-effect-guidebook-plant-robust-harvest =
    { $chance ->
        [1] Увеличивает
       *[other] увеличить
    } эффективность растения на { $increase } до максимума { $limit }. Растение теряет семена, когда эффективность достигает { $seedlesstreshold }. Попытка увеличить эффективность выше { $limit } может снизить урожай с вероятностью 10%.
entity-effect-guidebook-plant-seeds-add =
    { $chance ->
        [1] Восстанавливает
       *[other] восстановить
    } семена растения
entity-effect-guidebook-plant-seeds-remove =
    { $chance ->
        [1] Убирает
       *[other] убрать
    } семена растения
entity-effect-guidebook-plant-mutate-chemicals =
    { $chance ->
        [1] Мутирует
       *[other] мутировать
    } растение для производства { $name }
