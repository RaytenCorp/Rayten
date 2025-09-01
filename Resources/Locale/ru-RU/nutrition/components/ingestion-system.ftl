### Сообщения взаимодействия


# Система


## Когда пытаешься съесть без нужного прибора… но нужно его держать

ingestion-you-need-to-hold-utensil = Нужно держать { INDEFINITE($utensil) } { $utensil }, чтобы это съесть!
ingestion-try-use-is-empty = { CAPITALIZE($entity) } пуст!
ingestion-try-use-wrong-utensil = Вы не можете { $verb } { $food } с помощью { INDEFINITE($utensil) } { $utensil }.
ingestion-remove-mask = Сначала нужно снять { $entity }.

## Неудачная попытка поесть

ingestion-you-cannot-ingest-any-more = Вы больше не можете { $verb }!
ingestion-other-cannot-ingest-any-more = { CAPITALIZE(SUBJECT($target)) } больше не может { $verb }!
ingestion-cant-digest = Вы не можете переварить { $entity }!
ingestion-cant-digest-other = { CAPITALIZE(SUBJECT($target)) } не может переварить { $entity }!

## Глаголы действий (не путать с Verbs)

ingestion-verb-food = Есть
ingestion-verb-drink = Пить

# Компонент еды

edible-nom = Ням. { $flavors }
edible-nom-other = Ням.
edible-slurp = Хлюп. { $flavors }
edible-slurp-other = Хлюп.
edible-swallow = Вы проглатываете { $food }
edible-gulp = Глоть. { $flavors }
edible-gulp-other = Глоть.
edible-has-used-storage = Вы не можете { $verb } { $food }, пока внутри есть предмет.

## Существительные

edible-noun-edible = съестное
edible-noun-food = еда
edible-noun-drink = напиток
edible-noun-pill = таблетка

## Глаголы

edible-verb-edible = проглотить
edible-verb-food = есть
edible-verb-drink = пить
edible-verb-pill = проглотить

## Принудительное кормление

edible-force-feed = { CAPITALIZE($user) } пытается заставить вас { $verb } что-то!
edible-force-feed-success = { CAPITALIZE($user) } заставил вас { $verb } что-то! { $flavors }
edible-force-feed-success-user = Вы успешно накормили { $target }
