
# О программе 
XmlEditor — визуальный (WYSIWYG) редактор XML. Предназначен для редактиирования и создания как документов так и таблиц базы данных в формате XML.

![Текстовый документ](https://raw.githubusercontent.com/KamilProgram/XmlEditor/master/img/screenshot1.png?token=APY2W7OTJGLJA5GG2WUI34DAAVR5G "TextDocument")


![Таблица](https://raw.githubusercontent.com/KamilProgram/XmlEditor/master/img/screenshot2.png?token=APY2W7LBBXIKMMLB254PXKTAAVSGC "Table")

# Функции 
- Отслеживание ошиибок в документе
- Настраиваемая подсветка синтаксиса (можно выбрать цвет тэгов, атрибутов, заначений атрубутов и коментариев) 
- Проферка орфографии (только русский язык)
- Часто используемые теги вынесены а в отдельную панель под текстовым поле
- Парсинг таблиц с уровнями вложенности
- Возможность переключаться между двумя видами xslt шаблонов tg_builder и serna
- Возможно работать сразу с несколькими документами, реализован механизм вкладок
- Drag&Drop
- Реализована боковая панель "Проводник" для более оперативного доступа к файлам

# Используемые библиотеки 
- AvalonEdit текстовое поле
- CefSharp браузер
- NHunspell проверка орфограии
- GongSolutions.WPF.DragDrop перетаскиивание вкладок, Drag&Drop
