# Dream Analyzer

## Название и назначение

**Dream Analyzer** — веб-приложение для ведения дневника сновидений с функциями автоматического анализа, интерпретации и статистики.

**Назначение:** помочь пользователю записывать сны, получать их толкование с использованием нескольких психологических подходов (психоанализ Фрейда, архетипы Юнга, КПТ, словарь символов, сонники) и выявлять закономерности через статистику.

**Проблема, которую решает система:**
- Существующие дневники снов позволяют только записывать сны, но не анализировать их
- Онлайн-сонники дают толкование одного символа вне контекста всего сна
- Нет единого инструмента, объединяющего запись, анализ и статистику

## Целевые пользователи

- **Основной пользователь (User):** люди, интересующиеся снами и самоанализом
- **Администратор (Admin):** управление пользователями и контентом системы

## Стек технологий

| Слой | Технологии |
|------|-----------|
| **Backend** | C# 13, ASP.NET Core 10, Entity Framework Core |
| **Frontend** | React 18, Vite, React Router, Framer Motion |
| **СУБД** | PostgreSQL 16 |
| **AI** | OpenRouter API (meta-llama/llama-3.3-70b-instruct) |
| **Аутентификация** | JWT (HS256) + BCrypt |
| **Тестирование** | xUnit, Moq, FluentAssertions |
| **Обработка текста** | Iveonik.Stemmers (RussianStemmer) |

## Архитектура

Проект построен по принципам **чистой архитектуры (Clean Architecture)** с разделением на слои и правилом зависимостей «внутрь».

```
┌──────────────────────────────────────────────────────────┐
│                  DreamAnalyzer.Client                    │
│                   (React SPA - UI)                       │
└──────────────────────────┬───────────────────────────────┘
                           │ HTTP/JSON
                           ↓
┌──────────────────────────────────────────────────────────┐
│                  DreamAnalyzer.Server                    │
│           (ASP.NET Core Web API, Controllers)            │
└──────────────────────────┬───────────────────────────────┘
                           │
                           ↓
┌──────────────────────────────────────────────────────────┐
│              DreamAnalyzer.Application                   │
│      (Services, Strategies, DTOs, Builders, Interfaces)  │
└──────────────────────────┬───────────────────────────────┘
                           │
              ┌────────────┴────────────┐
              ↓                         ↓
┌──────────────────────────┐  ┌──────────────────────────┐
│  DreamAnalyzer.Domain    │  │ DreamAnalyzer.Shared     │
│ (Entities, ValueObjects, │  │ (ApiResponse,            │
│  Interfaces)             │  │  Exceptions, Validators) │
└──────────────────────────┘  └──────────────────────────┘
                           ↑
                           │
┌──────────────────────────────────────────────────────────┐
│            DreamAnalyzer.Infrastructure                  │
│  (Repositories, AppDbContext, JWT, PasswordHasher,       │
│   OpenRouterDreamClient)                                 │
└──────────────────────────────────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        ↓                  ↓                  ↓
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  PostgreSQL  │  │  OpenRouter  │  │  File System │
│              │  │     API      │  │   (static)   │
└──────────────┘  └──────────────┘  └──────────────┘
```

## Перечень модулей

| Модуль | Ответственность |
|--------|----------------|
| **DreamAnalyzer.Domain** | Бизнес-сущности (User, Dream, DreamAnalysis, DreamSymbol), Value Objects (Mood), интерфейсы репозиториев |
| **DreamAnalyzer.Application** | Бизнес-логика: сервисы (Auth, Dream, User, Analysis, Symbol), стратегии анализа, DTOs, Builder, Factory |
| **DreamAnalyzer.Infrastructure** | Работа с PostgreSQL (EF Core), репозитории, JWT-генератор, хеширование паролей, HTTP-клиент к OpenRouter |
| **DreamAnalyzer.Server** | Контроллеры REST API, middleware обработки исключений, DI-контейнер, Swagger |
| **DreamAnalyzer.Client** | React SPA: страницы, компоненты, формы, статистика, анимации |
| **DreamAnalyzer.Shared** | Общие DTO (ApiResponse), кастомные исключения (NotFoundException, ValidationException), валидаторы |
| **DreamAnalyzer.Tests** | Модульные тесты сервисов, стратегий, фабрики |

## Точки интеграции

| Точка | Тип | Описание |
|-------|-----|----------|
| **PostgreSQL** | БД | Основное хранилище (Users, Dreams, DreamAnalyses, DreamSymbols, AnalysisSymbols) |
| **OpenRouter API** | Внешний сервис | AI-анализ снов (HTTP/REST) |
| **JWT** | Внутренний сервис | Генерация/проверка токенов |
| **BCrypt** | Библиотека | Хеширование паролей |
| **Iveonik.Stemmers** | Библиотека | Стемминг русского текста для поиска символов |
| **Файловая система** | Локально | Статические файлы фронтенда (wwwroot) |

## Инструкция по сборке и запуску

### Требования
- .NET 10 SDK
- Node.js 20+
- PostgreSQL 16+
- API-ключ OpenRouter

### 1. Клонирование
```bash
git clone https://github.com/username/DreamAnalyzer2.git
cd DreamAnalyzer2
```
### 2. Настройка БД
```sql
CREATE DATABASE "DreamAnalyzerDB";
```
### 3. Настройка секретов (User Secrets)
```bash
cd DreamAnalyzer2.Server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=DreamAnalyzerDB;Username=postgres;Password=yourpassword"
dotnet user-secrets set "JwtSettings:Secret" "your-super-secret-key-at-least-32-chars"
dotnet user-secrets set "OpenRouterSettings:ApiKey" "sk-or-v1-..."
```
### 4. Миграции БД
```bash
dotnet ef database update --project DreamAnalyzer2.Infrastructure --startup-project DreamAnalyzer2.Server
```
### 5. Запуск backend
```bash
cd DreamAnalyzer2.Server
dotnet run
API будет доступно на https://localhost:7021, Swagger: https://localhost:7021/swagger
```
### 6. Запуск frontend
```bash
cd dreamanalyzer2.client
npm install
npm run dev
```
UI будет доступно на https://localhost:4050

### 7. Запуск тестов
```bash
dotnet test
```
