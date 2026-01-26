markings-search = Поиск
-markings-selection =
    { $selectable ->
        [0] У вас не осталось доступных черт.
        [one] Вы можете выбрать ещё одну черту.
       *[other] Вы можете выбрать ещё { $selectable } черт.
    }
markings-limits = { $required ->
    [true] { $count ->
        [-1] Выберите как минимум одну черту.
        [0] Вы не можете выбрать ни одной черты, но почему-то должны? Это баг.
        [one] Выберите одну черту.
       *[other] Выберите как минимум одну и не более { $count } черт. { -markings-selection(selectable: $selectable) }
    }
   *[false] { $count ->
        [-1] Выберите любое количество черт.
        [0] Вы не можете выбрать ни одной черты.
        [one] Выберите не более одной черты.
       *[other] Выберите не более { $count } черт. { -markings-selection(selectable: $selectable) }
    }
}
markings-reorder = Изменить порядок черт
humanoid-marking-modifier-respect-limits = Учитывать ограничения
humanoid-marking-modifier-respect-group-sex = Учитывать ограничения группы и пола
humanoid-marking-modifier-base-layers = Базовые слои
humanoid-marking-modifier-enable = Включить
humanoid-marking-modifier-prototype-id = ID прототипа:

# Categories

markings-organ-Torso = Торс
markings-organ-Head = Голова
markings-organ-ArmLeft = Левая рука
markings-organ-ArmRight = Правая рука
markings-organ-HandRight = Правая кисть
markings-organ-HandLeft = Левая кисть
markings-organ-LegLeft = Левая нога
markings-organ-LegRight = Правая нога
markings-organ-FootLeft = Левая стопа
markings-organ-FootRight = Правая стопа
markings-organ-Eyes = Глаза
markings-layer-Special = Особое
markings-layer-Tail = Хвост
markings-layer-Tail-Moth = Крылья
markings-layer-Hair = Волосы
markings-layer-FacialHair = Лицевая растительность
markings-layer-UndergarmentTop = Нижнее бельё (верх)
markings-layer-UndergarmentBottom = Нижнее бельё (низ)
markings-layer-Chest = Грудь
markings-layer-Head = Голова
markings-layer-Snout = Морда
markings-layer-SnoutCover = Морда (покрытие)
markings-layer-HeadSide = Голова (бок)
markings-layer-HeadTop = Голова (верх)
markings-layer-Eyes = Глаза
markings-layer-RArm = Правая рука
markings-layer-LArm = Левая рука
markings-layer-RHand = Правая кисть
markings-layer-LHand = Левая кисть
markings-layer-RLeg = Правая нога
markings-layer-LLeg = Левая нога
markings-layer-RFoot = Правая стопа
markings-layer-LFoot = Левая стопа
markings-layer-Overlay = Оверлей
