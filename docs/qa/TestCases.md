# Тест-кейсы (Test Cases) — Dream Analyzer

**Проект:** Dream Analyzer  
**Дата:** 2026-10-19  
**Инструмент:** Postman

## Таблица тест-кейсов

| ID | Приоритет | Требование | Модуль | Заглавие (Суть) | Исходные данные | Шаги | Ожидаемый результат |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC-01** | Высокий | FR-01, FR-03 | Аутентификация | Успешная регистрация с валидными данными | `Body (JSON):`<br>`{"username": "userA", "email": "{{test_email}}", "password": "Test123"}` | 1. Отправить POST-запрос на `/api/authentication/register` | 200 OK. В теле ответа `success: true`, присутствует JWT-токен. |
| **TC-02** | Высокий | FR-01 | Аутентификация | Регистрация с пустым паролем | `Body (JSON):`<br>`{"username": "user2", "email": "u2@test.com", "password": ""}` | 1. Отправить POST-запрос на `/api/authentication/register` | 400 Bad Request. Ошибка валидации. |
| **TC-03** | Высокий | FR-02 | Аутентификация | Регистрация с дублирующимся email | `Body (JSON):`<br>`{"username": "user3", "email": "{{test_email}}", "password": "Test123"}` | 1. Отправить POST-запрос на `/api/authentication/register` | 401 Unauthorized. Сообщение "User with this email already exists". |
| **TC-04** | Высокий | FR-04 | Аутентификация | Успешный логин (возврат JWT) | `Body (JSON):`<br>`{"email": "{{test_email}}", "password": "Test123"}` | 1. Отправить POST-запрос на `/api/authentication/login` | 200 OK. Токен получен и сохранен в `token_userA`. |
| **TC-05** | Высокий | FR-04 | Аутентификация | Логин с неверным паролем | `Body (JSON):`<br>`{"email": "{{test_email}}", "password": "Wrong"}` | 1. Отправить POST-запрос на `/api/authentication/login` | 401 Unauthorized. |
| **TC-06** | Высокий | FR-04 | Аутентификация | Доступ без токена | `Headers:`<br>*(Нет Authorization)* | 1. Отправить GET-запрос на `/api/Dreams` | 401 Unauthorized. |
| **TC-07** | Высокий | FR-06 | Управление снами | Создание сна с валидными данными | `Headers:`<br>`Bearer {{token_userA}}`<br>`Body:`<br>`{"title": "Сон", "content": "Мне снилась кошка", "dreamDate": "2023-10-18T12:00:00Z"}` | 1. Отправить POST-запрос на `/api/Dreams` | 200 OK. Сон создан, ID сохранен в `{{dreamId}}`. |
| **TC-08** | Высокий | FR-06 | Управление снами | Создание сна с пустым заголовком | `Headers:`<br>`Bearer {{token_userA}}`<br>`Body:`<br>`{"title": "", "content": "Текст", "dreamDate": "2023-10-18T12:00:00Z"}` | 1. Отправить POST-запрос на `/api/Dreams` | 400 Bad Request. |
| **TC-09** | Высокий | FR-07 | Управление снами | Получение своего сна по ID | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить GET-запрос на `/api/Dreams/{{dreamId}}` | 200 OK. Возвращается сон. |
| **TC-10** | Высокий | FR-07 | Управление снами | IDOR: Чтение чужого сна | `Headers:`<br>`Bearer {{token_userB}}` | 1. Отправить GET-запрос на `/api/Dreams/{{dreamId}}` | 403 Forbidden. Сон не возвращается. |
| **TC-11** | Высокий | FR-08 | Управление снами | Редактирование своего сна | `Headers:`<br>`Bearer {{token_userA}}`<br>`Body:`<br>`{"title": "Новый заголовок", "content": "Новый текст", "dreamDate": "2023-10-18T12:00:00Z"}` | 1. Отправить PUT-запрос на `/api/Dreams/{{dreamId}}` | 200 OK. Данные обновлены. |
| **TC-12** | Высокий | FR-08 | Управление снами | IDOR: Редактирование чужого сна | `Headers:`<br>`Bearer {{token_userB}}`<br>`Body:`<br>`{"title": "Взлом", "content": "Взлом", "dreamDate": "2023-10-18T12:00:00Z"}` | 1. Отправить PUT-запрос на `/api/Dreams/{{dreamId}}` | 403 Forbidden. Данные не изменены. |
| **TC-13** | Высокий | FR-09 | Анализ | Анализ «Словарь символов» | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить POST на `/api/Dreams/{{dreamId}}/analyze?strategyType=symbols` | 200 OK. Список символов (содержит "кошка"). |
| **TC-14** | Высокий | FR-10 | Анализ | Анализ «Фрейд» (AI) | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить POST на `/api/Dreams/{{dreamId}}/analyze?strategyType=freudian` | 200 OK. Есть поле Interpretation. |
| **TC-15** | Высокий | FR-11 | Анализ | Анализ «Юнг» (AI) | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить POST на `/api/Dreams/{{dreamId}}/analyze?strategyType=jungian` | 200 OK. Есть поле Interpretation. |
| **TC-16** | Высокий | FR-12 | Анализ | Анализ «КПТ» (AI) | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить POST на `/api/Dreams/{{dreamId}}/analyze?strategyType=cognitive` | 200 OK. Есть поле Interpretation. |
| **TC-17** | Высокий | FR-13 | Анализ | Анализ «Сонник» (AI) | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить POST на `/api/Dreams/{{dreamId}}/analyze?strategyType=dreambook&book=vanga` | 200 OK. Есть поле Interpretation. |
| **TC-18** | Высокий | FR-13 | Анализ | Запрос несуществующей стратегии | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить POST на `/api/Dreams/{{dreamId}}/analyze?strategyType=unknown` | 400 Bad Request. |
| **TC-19** | Высокий | FR-14 | Статистика | Получение статистики | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить GET-запрос на `/api/users/statistics` | 200 OK. `TotalDreams > 0`. |
| **TC-20** | Высокий | FR-16 | Админ-панель | Просмотр списка пользователей | `Headers:`<br>`Bearer {{token_admin}}` | 1. Отправить GET-запрос на `/api/admin/users` | 200 OK. Массив пользователей. |
| **TC-21** | Высокий | FR-05 | Админ-панель | Доступ к админке обычным юзером | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить GET-запрос на `/api/admin/users` | 403 Forbidden. |
| **TC-22** | Высокий | FR-17 | Админ-панель | Просмотр всех снов | `Headers:`<br>`Bearer {{token_admin}}` | 1. Отправить GET-запрос на `/api/admin/dreams` | 200 OK. Массив снов. |
| **TC-23** | Высокий | FR-08 | Управление снами | Удаление своего сна | `Headers:`<br>`Bearer {{token_userA}}` | 1. Отправить DELETE на `/api/Dreams/{{dreamId}}` | 200 OK. Повторный GET возвращает 404. |
| **TC-24** | Высокий | FR-20 | API | Обработка ошибок (битый JSON) | `Headers:`<br>`Bearer {{token_userA}}`<br>`Body:`<br>`{"title": "Сон", "content": ` | 1. Отправить POST на `/api/Dreams` | 400 Bad Request. Ответ в формате JSON. |
| **TC-25** | Высокий | FR-21 | API | Доступность Swagger UI | `URL:`<br>`{{baseUrl}}/swagger` | 1. Открыть в браузере. | 200 OK. Отображается Swagger UI. |
| **TC-26** | Высокий | FR-20 | API | Запрос к несуществующему эндпоинту | `URL:`<br>`{{baseUrl}}/api/nonexistent` | 1. Отправить GET-запрос. | 404 Not Found. |
| **TC-27** | Высокий | FR-18 | Админ-панель | Удаление сна Админом | `Headers:`<br>`Bearer {{token_admin}}` | 1. Отправить DELETE-запрос на `/api/admin/dreams/{{dreamId}}` | 200 OK. Сон удален из системы. |