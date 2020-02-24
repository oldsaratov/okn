# ОКН:Монитор

[![Build Status](https://oldsaratov.visualstudio.com/OKN/_apis/build/status/OKN-API?branchName=develop)](https://oldsaratov.visualstudio.com/OKN/_build/latest?definitionId=1&branchName=develop)

Веб-приложение для мониторинга жизненного цикла объектов культурного наследия Саратовской области. 

[![ОКН](https://oldsaratov.ru/okn.jpeg)](https://okn.oldsaratov.ru)

Цель проекта в том, чтобы в доступной и привлекательной форме визуализировать данные из реестра объектов культурного наследия. После заполнения базы фотографиями и описаниями вместо скупого "Дом жилой XIX век" можно будет увидеть фотографию, историческое описание, юридически документы и ленту событий, произошедших с памятником.

Легко адаптируется для использования в любом другом регионе.

[![ОКН](https://oldsaratov.ru/oknexample.jpeg)](https://okn.oldsaratov.ru)

# Реализованные функции

  - Карта объектов культурного наследия
  - История событий, произошедших с объектом (по сообщениям СМИ и других источников)
  - Добавление фотографий и документов

# Запланированные функции

  - История переходов между статусами (выявленный -> местного значения -> ...)
  - Теги и фильтрация по тегам
  - Подписка на обновления состояния объекта
  - **Визуализация охранной зоны**
  - **Тревожная кнопка**


## Технологии
- Single-page приложение на ReactJS 
- [Docker-контейнер](https://hub.docker.com/r/oldsaratov/okn) для API приложения
- [MongoDB](https://www.mongodb.com/cloud/atlas) в качестве хранилища данных
- [Mapbox](https://www.mapbox.com/) для карты
- [Uploadcare](https://uploadcare.com/) для загрузки фотографий и файлов

Исходный код распространяется под  лицензией.  
[API-документация](https://okn.oldsaratov.ru/swagger/index.html)
  
    
## Создано при поддержке

- Некомерческий краеведческий интернет-проект  
[![Build Status](https://oldsaratov.ru/sites/default/files/logo_1.png)](https://oldsaratov.ru)

- [Архнадзор Саратов](https://www.facebook.com/groups/545086345864091/)
