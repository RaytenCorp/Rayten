<p align="center"> <img alt="Vanilla Station" width="880" height="300" src="https://raw.githubusercontent.com/space-wizards/asset-dump/de329a7898bb716b9d5ba9a0cd07f38e61f1ed05/github-logo.svg" /></p>

<h1 align="center">💮Vanilla Station</h1>

Space Station 14 — это ремейк SS13, который работает на собственном движке [Robust Toolbox](https://github.com/space-wizards/RobustToolbox), написанном на C#.

Это репозиторий русскоязычного сервера по Space Station 14, Vanilla Station, целью которого является держать баланс между функциями и РП, добится успеха проекта, а также создание лучшего игрового сообщества.

## Ссылки

[Наш Discord](https://discord.gg/W3Ep2esrzc) | [Наша Вики](https://vanilla-station.ru/) | [Официальный репозиторий](https://github.com/space-wizards/space-station-14)

## Документация

На официальном сайте с [документацией](https://docs.spacestation14.io/) имеется вся необходимая информация о контенте SS14, движке, дизайне игры и многом другом. Также имеется много информации для начинающих разработчиков.

## Контрибьют

Мы рады принять вклад от любого человека. Заходите в Discord и пишите, если хотите помочь. У нас есть [список проблем](https://github.com/VanillaStation14/VanillaStation/issues), которые нужно решить, и любой может за них взяться. Не бойтесь просить о помощи!
Только убедитесь, что ваши изменения и PRы соответствуют [руководству по контрибьюту](https://docs.spacestation14.com/en/general-development/codebase-info/pull-request-guidelines.html).

## Сборка

1. Склонируйте этот репозиторий локально.
2. Запустите `RUN_THIS.py` для инициализации подмодулей и скачивания движка.
3. [Скомпилируйте](https://docs.spacestation14.com/en/general-development/setup/server-hosting-tutorial.html#level-2-server-with-custom-code) проект.

### Консольные команды

```bash
# Скопировать репозиторий
git clone https://github.com/VanillaStation14/VanillaStation.git

# Перейти в скачанный репозиторий
cd Vanilla

# Запустить RUN_THIS.py
python ./RUN_THIS.py

# Скомпилировать проект под Windows
dotnet build Content.Packaging --configuration Release
```

[Более подробная инструкция по запуску проекта.](https://docs.spacestation14.com/en/general-development/setup.html)

## Лицензия
Весь код данного [pr](https://github.com/RaytenCorp/VanillaStation/pull/132) лицензирован под AGPLv3.

Весь прочий код репозитория лицензирован под [MIT](https://github.com/space-syndicate/space-station-14/blob/master/LICENSE.TXT), включая код Vanilla Station и их контрибутеров.

Большинство ассетов лицензированы под [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/), если не указано иное. Ассеты имеют свою лицензию и авторские права в файле метаданных. [Пример](https://github.com/space-syndicate/space-station-14/blob/master/Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).

Обратите внимание, что некоторые ассеты лицензированы на некоммерческой основе [CC-BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) или аналогичной некоммерческой лицензией, и их необходимо удалить, если вы хотите использовать этот проект в коммерческих целях.

P.S. Спасибо что выбрали Vanilla Station!
