# Отчет о выполнении тест-кейсов (Test Execution Report)

**Дата выполнения:** 2026-10-18  
**Инструмент:** Postman  
**Окружение:** Localhost, PostgreSQL 16.  

## Результаты выполнения

| ID | Название проверки | Статус | Фактический результат | Доказательство (Скриншот) |
| :--- | :--- | :--- | :--- | :--- |
| **TC-01** | Успешная регистрация | Passed | 200 OK, токен получен. | ![tc-01](screens/tc-01.png) |
| **TC-02** | Регистрация с пустым паролем | Passed | 400 Bad Request. | ![tc-02](screens/tc-02.png) |
| **TC-03** | Регистрация с дублирующимся email | Passed | 401 Unauthorized. | ![tc-03](screens/tc-03.png) |
| **TC-04** | Успешный логин (получение JWT) | Passed | 200 OK, скрипт сохранил токен. | ![tc-04](screens/tc-04.png) |
| **TC-05** | Логин с неверным паролем | Passed | 401 Unauthorized. | ![tc-05](screens/tc-05.png) |
| **TC-06** | Доступ без токена (401) | Passed | 401 Unauthorized. | ![tc-06](screens/tc-06.png) |
| **TC-07** | Создание сна (валидные данные) | Passed | 200 OK, ID сна создан. | ![tc-07](screens/tc-07.png) |
| **TC-08** | Создание сна с пустым заголовком | Passed | 400 Bad Request. | ![tc-08](screens/tc-08.png) |
| **TC-09** | Чтение своего сна по ID | Passed | 200 OK, сон получен. | ![tc-09](screens/tc-09.png) |
| **TC-10** | IDOR (чтение чужого сна) | Passed | 403 Forbidden. | ![tc-10](screens/tc-10.png) |
| **TC-11** | Редактирование своего сна | Passed | 200 OK, данные обновлены. | ![tc-11](screens/tc-11.png) |
| **TC-12** | IDOR (редактирование чужого сна) | Passed | 403 Forbidden. | ![tc-12](screens/tc-12.png) |
| **TC-13** | Анализ сна (Словарь символов) | Passed | 200 OK, символы найдены. | ![tc-13](screens/tc-13.png) |
| **TC-14** | Анализ сна (Фрейд, AI) | Passed | 200 OK, текст получен. | ![tc-14](screens/tc-14.png) |
| **TC-15** | Анализ сна (Юнг, AI) | Passed | 200 OK, текст получен. | ![tc-15](screens/tc-15.png) |
| **TC-16** | Анализ сна (КПТ, AI) | Passed | 200 OK, текст получен. | ![tc-16](screens/tc-16.png) |
| **TC-17** | Анализ сна (Сонник, AI) | Passed | 200 OK, текст получен. | ![tc-17](screens/tc-17.png) |
| **TC-18** | Неверная стратегия анализа | Passed | 400 Bad Request. | ![tc-18](screens/tc-18.png) |
| **TC-19** | Получение статистики | Passed | 200 OK, статистика верна. | ![tc-19](screens/tc-19.png) |
| **TC-20** | Просмотр всех юзеров (Админ) | Passed | 200 OK, список юзеров. | ![tc-20](screens/tc-20.png) |
| **TC-21** | Доступ к админке обычным юзером | Passed | 403 Forbidden. | ![tc-21](screens/tc-21.png) |
| **TC-22** | Просмотр всех снов (Админ) | Passed | 200 OK, список снов. | ![tc-22](screens/tc-22.png) |
| **TC-23** | Удаление своего сна | Passed | 200 OK, сон удален. | ![tc-23](screens/tc-23.png) |
| **TC-24** | Обработка ошибок (битый JSON) | Passed | 400 Bad Request (JSON). | ![tc-24](screens/tc-24.png) |
| **TC-25** | Доступность Swagger UI | Passed | 200 OK, Swagger открыт. | ![tc-25](screens/tc-25.png) |
| **TC-26** | Запрос к несуществующему эндпоинту | Passed | 404 Not Found. | ![tc-26](screens/tc-26.png) |
| **TC-27** | Удаление сна Админом | Passed | 200 OK, сон удален. | ![tc-27](screens/tc-27.png) |

---
**Итоговая статистика:**
* Всего тест-кейсов: 27
* Passed: 27
* Failed: 0
* Покрытие (T_E): 100%
* Успешность (T_SP): 100%