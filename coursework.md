---
title: "Техническая документация микросервиса MatchService"
lang: ru-RU
---

# Техническая документация микросервиса MatchService

**Проект:** VirtuSphere / Eventide.MatchService  
**Платформа:** .NET 8, ASP.NET Core, Entity Framework Core 8, PostgreSQL  
**Архитектура:** Domain-Driven Design и слоистая архитектура  
**Назначение:** управление матчами, пользователями и администраторами в составе платформы проведения турниров

## Содержание

1. Назначение проекта
2. Область ответственности
3. Технологический стек
4. Структура решения
5. Архитектура
6. Доменная модель
7. Правила жизненного цикла матча
8. Слой моделей и application-служб
9. REST API
10. Инфраструктура и база данных
11. Конфигурация и запуск
12. Тестирование и качество
13. Событийные контракты
14. Известные ограничения и несогласованности
15. Рекомендации по развитию
16. Список литературы

## 1. Назначение проекта

`MatchService` — серверное приложение для управления игровыми матчами платформы VirtuSphere. Сервис хранит сведения о двух участниках матча, турнире, турнирной сетке, администраторе, статусе встречи, времени проведения, карте, сервере и счёте.

Основные задачи сервиса:

- управление сущностями «пользователь» и «администратор»;
- создание матча по идентификаторам связанных сущностей;
- хранение состояния матча;
- применение доменных правил при старте, редактировании, планировании и завершении матча;
- предоставление REST-интерфейса;
- сохранение данных в PostgreSQL через Entity Framework Core;
- выполнение миграций базы данных при запуске приложения;
- поддержка модульных и интеграционных тестов.

Проект не реализует целиком платформу VirtuSphere. Турнирный и сеточный сервисы представлены в модели только минимальными доменными сущностями `Tournament` и `Bracket`. Для реальной асинхронной интеграции в проекте подготовлен контекст контрактов событий, но брокер сообщений в конфигурации приложения сейчас отсутствует.

## 2. Область ответственности

### MatchService отвечает за

- жизненный цикл отдельного матча;
- проверку допустимости операций над матчем;
- связь матча с пользователями, турниром, сеткой и администратором;
- сценарий оспаривания результата участниками;
- учёт победителя и счёта;
- CRUD-операции для пользователей и администраторов;
- создание и удаление матчей через application-службу;
- отображение доменных сущностей в DTO-модели API.

### Вне текущей реализации остаются

- аутентификация и авторизация;
- распределение прав на конкретные матч через JWT или роли;
- построение турнирной сетки;
- управление владельцем и форматом турнира;
- вычисление рейтинга или статистики;
- отправка событий в брокер сообщений;
- фоновые уведомления игроков;
- обработка платежей и внешние интеграции.

## 3. Технологический стек

| Компонент | Технология |
|---|---|
| Платформа | .NET 8 |
| Язык | C# |
| HTTP-хост | ASP.NET Core Web API |
| ORM | Entity Framework Core 8.0.8 |
| База данных | PostgreSQL |
| Драйвер PostgreSQL | Npgsql |
| Модели API | .NET record-типы |
| Сопоставление моделей | AutoMapper |
| Документация API | Swagger / OpenAPI |
| Unit-тестирование | xUnit |
| Моки | Moq |
| Интеграционные тесты БД | SQLite в тестовом проекте |
| Покрытие кода | Coverlet |
| CI | GitHub Actions |

Версия .NET задаётся каждым проектом через `TargetFramework net8.0`. Решение содержит проект тестирования и один доменный консольный образец `DomainApp`.

## 4. Структура решения

Корень решения: `Eventide.MatchService.sln`.

```text
MatchService/
├── Applivation/
│   ├── MatchService.Application.Models/
│   └── MatchService.Application.Services/
├── Contracts/
│   └── Events/
├── Domain/
│   ├── MatchService.Domain/
│   ├── MatchService.Domain.Repositories.Abstractions/
│   ├── MatchService.ValueObjects/
│   └── DomainApp/
├── Infrastructure/
│   └── MatchService.Infrastructure.EntityFramework/
├── Presentation/
│   └── MatchService.WebHost/
├── Tests/
│   └── MatchService.Tests/
└── .github/workflows/
```

### 4.1. Доменный проект

`Domain/MatchService.Domain` содержит сущности, перечисления, доменные исключения и базовый тип `Entity`.

Основные файлы:

- `Match.cs` — корень агрегата и правила жизненного цикла;
- `User.cs` — участник матча;
- `Administrator.cs` — владелец операций над матчами;
- `Tournament.cs` — минимальное представление турнира;
- `Bracket.cs` — минимальное представление турнирной сетки;
- `Enums/MatchStatus.cs` — допустимые статусы;
- `Exceptions/*.cs` — доменные ошибки.

### 4.2. Объекты-значения

`Domain/MatchService.ValueObjects` содержит объекты-значения и валидаторы:

- `Username`;
- `MapName`;
- `ServerInfo`;
- `PlayerScore`.

Общий базовый тип `ValueObject<T>` выполняет валидацию и хранит примитивное значение. Конкретные ограничения описаны в разделе 6.

### 4.3. Абстракции репозиториев

`Domain/MatchService.Domain.Repositories.Abstractions` отделяет домен от EF Core. Здесь находятся общий `IRepository<TEntity, TId>` и специализированные интерфейсы:

- `IUserRepository`;
- `IAdministratorRepository`;
- `IMatchRepository`;
- `ITournamentRepository`;
- `IBracketRepository`.

Специализированные репозитории позволяют найти пользователя или администратора по имени.

### 4.4. Application-модели

`Applivation/MatchService.Application.Models` содержит DTO-записи для запросов и ответов:

- `CreateMatchModel`;
- `MatchModel`;
- `CreateUserModel`;
- `UserModel`;
- `CreateAdministratorModel`;
- `AdministratorModel`;
- `BracketModel`;
- базовые интерфейсы `IModel` и `ICreateModel`.

### 4.5. Application-службы

`Applivation/MatchService.Application.Services` содержит сценарии приложения:

- `MatchApplicationService`;
- `UserApplicationService`;
- `AdministratorApplicationService`;
- AutoMapper-профиль `ApplicationProfile`;
- интерфейсы application-служб.

Application-слой объединяет репозитории и доменную модель, но не содержит HTML или HTTP-кода.

### 4.6. Инфраструктура

`Infrastructure/MatchService.Infrastructure.EntityFramework` содержит:

- `ApplicationDbContext`;
- конфигурации Fluent API;
- реализацию generic репозитория;
- специализированные реализации репозиториев;
- одну сгенерированную миграцию;
- снимок EF-модели.

### 4.7. Presentation

`Presentation/MatchService.WebHost` содержит:

- точку входа `Program.cs`;
- контроллеры;
- настройки запуска;
- конфигурацию логирования;
- строку подключения;
- расширение применения миграций.

### 4.8. Контракты

`Contracts/Eventide.MatchService.Contracts.csproj` — отдельная библиотека классов для межсервисных контрактов. На данный момент определён тип `MatchCompletedEvent`.

### 4.9. Тесты

`Tests/MatchService.Tests` содержит тесты доменной логики, application-служб, репозиториев, контроллеров, моделей и объектов-значений. Для большинства тестов используется SQLite и Moq.

## 5. Архитектура

Архитектура проекта построена по принципу слоёв и правила зависимостей:

```text
Presentation
    ↓
Application
    ↓
Domain
    ↑
Infrastructure реализует абстракции Domain
```

### Правило зависимостей

- доменный проект не должен ссылаться на EF Core или ASP.NET Core;
- application-слой зависит от доменных абстракций;
- инфраструктура реализует доменные интерфейсы;
- presentation-компонент является точкой композиции и ссылается на application-, domain- и infrastructure-проекты;
- репозиторий является границей сохранения данных.

Строка в коде может нарушать формальное правило чистой архитектуры тем, что реализация application-службы напрямую вызывает доменные конструкторы и методы. Это допустимо для доменно-ориентированной бизнес-логики: application-служба выступает координатором сценария, а доменный агрегат обеспечивает инварианты.

## 6. Доменная модель

### 6.1. Базовая сущность

`Entity<TId>` является базовым типом доменных сущностей. Он хранит идентификатор `Id`.

Корень агрегата `Match` создаётся с типом `Guid`, который генерируется доменным конструктором либо передаётся protected-конструктором для слоя ORM.

### 6.2. Match

`Match` — корень агрегата. Состояние матча нельзя изменять произвольным образом вне методов этого типа.

Свойства матча:

| Свойство | Назначение |
|---|---|
| `Tournament` | Турнир |
| `Bracket` | Турнирная сетка |
| `Administrator` | Администратор матча |
| `Player1` | Первый участник |
| `Player2` | Второй участник |
| `Winner` | Победитель, необязательное значение |
| `Status` | Текущий статус |
| `ScheduledTime` | Запланированное время |
| `StartedAt` | Фактическое время старта |
| `CompletedAt` | Время завершения |
| `MapName` | Название карты |
| `ServerInfo` | Сведения о сервере |
| `Player1Score` | Счёт первого игрока |
| `Player2Score` | Счёт второго игрока |
| `CreatedAt` | Время создания матча |

Конструктор публичного доменного агрегата создаёт матча со статусом `Scheduled` и временем создания в UTC.

Инвариант конструктора: идентификаторы `Player1` и `Player2` должны различаться. При совпадении вызывается `MatchPlayerSameException`.

### 6.3. Administrator

`Administrator` содержит имя пользователя и приватную коллекцию `_matches`. Публично коллекция возвращается как доступная только для чтения копия.

Методы администратора:

- `CreateMatch`;
- `EditMatch`;
- `DeleteMatch`;
- `RescheduleMatch`;
- `CancelMatch`;
- `DisputeMatch`;
- `StartMatch`;
- `SubmitMatchResult`;
- `ChangeUsername`.

Перед операциями каждый метод проверяет наличие матча в `_matches`. Если матча нет, вызывается `MatchNotFoundException`.

### 6.4. User

`User` содержит идентификатор и объект-значение `Username`. Игрок может изменить имя и оспорить матч.

Метод `DisputeMatch` проверяет статус `Completed`, затем вызывает перегруженный метод `Match.Dispute(this)`. Перегрузка дополнительно проверяет, что пользователь является одним из двух игроков.

### 6.5. Tournament и Bracket

Обе сущности в текущей реализации имеют только идентификатор `Guid`. Они нужны как независимые ссылки доменного агрегата.

В базе данных таблицы имеют одну ключевую колонку `Id`. Расширенная предметная логика турнира и сетки должна добавляться в отдельных ограниченных контекстах или в собственных типах платформы.

### 6.6. Объекты-значения

#### Username

- минимальная длина: 3;
- максимальная длина: 30;
- не допускается `null`, пустая строка или строка из пробелов.

#### MapName

- минимальная длина: 3;
- максимальная длина: 30;
- не допускается `null`, пустая строка или строка из пробелов.

#### ServerInfo

- максимальная длина: 120;
- не допускается `null`, пустая строка или строка из пробелов.

Дополнительной проверки IP-адреса или формата узла нет.

#### PlayerScore

- минимальное значение: 0;
- максимальное значение: 999.

Диапазон отражён валидатором и классом исключения `PlayerScoreOutOfRangeException`.

### 6.7. Доменные исключения

Проект содержит специализированные исключения для типовых нарушений доменной логики:

- `MatchPlayerSameException`;
- `MatchNotScheduledException`;
- `MatchNotInProgressException`;
- `MatchWinnerNotParticipantException`;
- `InvalidMatchStatusException`;
- `InvalidMatchWinnerException`;
- `MatchNotFoundException`;
- `ArgumentNullValueException`;
- `ArgumentNullOrWhiteSpaceException`;
- `ArgumentShortValueException`;
- `ArgumentLongValueException`;
- `ServerInfoNullOrWhiteSpaceException`;
- `ServerInfoTooLongException`;
- `PlayerScoreOutOfRangeException`.

## 7. Правила жизненного цикла матча

Перечисление `MatchStatus` содержит:

- `Scheduled`;
- `InProgress`;
- `Completed`;
- `Disputed`;
- `Cancelled`.

### Таблица допустимых операций

| Операция | Требуемый статус | Результат |
|---|---|---|
| `Schedule` | `Scheduled` | Записывает `ScheduledTime` |
| `Start` | `Scheduled` | Записывает карту и сервер, статус становится `InProgress`, заполняется `StartedAt` |
| `SubmitResult` | `InProgress` | Записывает победителя и счёт, статус становится `Completed`, заполняется `CompletedAt` |
| `Dispute` | Любой, кроме повторного `Disputed` | Статус становится `Disputed` |
| `Dispute(User)` | Повторное оспаривание запрещено; пользователь должен быть участником | Статус становится `Disputed` |
| `Cancel` | Повторная отмена запрещена | Статус становится `Cancelled` |
| `Edit` — победитель | `InProgress` | Изменяет победителя |
| `Edit` — время | `Scheduled` | Изменяет `ScheduledTime` |
| `Edit` — карта | `InProgress` | Изменяет `MapName` |
| `Edit` — сервер | `InProgress` | Изменяет `ServerInfo` |

### Схема состояний

```text
                      Schedule(time)
                      ┌──────────────┐
                      │  Scheduled   │
                      └──────┬───────┘
                             │ Start(server, map)
                             ▼
                      ┌──────────────┐
                      │  InProgress  │
                      └──────┬───────┘
                             │ SubmitResult
                             ▼
                      ┌──────────────┐
           Dispute ◄─────┤  Completed   │
                         └──────┬───────┘
                                │ Dispute
                                ▼
                         ┌──────────────┐
                         │  Disputed    │
                         └──────────────┘

Любое состояние, кроме повторно Cancelled, → Cancelled через Cancel()
```

Фактическое состояние отмены нельзя считать полностью ограниченным статусом `Scheduled`: метод `Match.Cancel()` не проверяет исходный статус. Ограничение описано только в XML-комментариях `Administrator.CancelMatch` и в учебных документах проекта, но не реализовано проверкой в агрегате.

Аналогично, метод `Match.Dispute()` позволяет перейти в спорное состояние из любого статуса, кроме уже `Disputed`. Более строгая проверка завершённости предусмотрена в `User.DisputeMatch`.

## 8. Слой моделей и application-служб

### 8.1. MatchModel

Ответный DTO матча содержит идентификаторы связей и все основные поля агрегата:

- `Id`;
- `TournamentId`;
- `BracketId`;
- `AdministratorId`;
- `Player1Id`;
- `Player2Id`;
- `WinnerId`;
- `Status`;
- `ScheduledTime`;
- `StartedAt`;
- `CompletedAt`;
- `MapName`;
- `ServerInfo`;
- `Player1Score`;
- `Player2Score`;
- `CreatedAt`.

### 8.2. CreateMatchModel

Запрос на создание содержит только идентификаторы:

```json
{
  "tournamentId": "guid",
  "bracketId": "guid",
  "administratorId": "guid",
  "player1Id": "guid",
  "player2Id": "guid"
}
```

Сервис последовательно загружает все связанные сущности. Если хотя бы одна отсутствует, ответом будет `400 Bad Request`.

### 8.3. Application-службы

`UserApplicationService` и `AdministratorApplicationService` предоставляют CRUD-сценарии. Имена проверяются на уникальность через специализированные репозитории.

`MatchApplicationService` реализует:

- получение всех матчей;
- получение матча по идентификатору;
- создание матча;
- заглушку логической проверки обновления;
- удаление матча.

Метод `UpdateModelAsync` в `MatchApplicationService` на данный момент проверяет только наличие сущности и не применяет изменения полей `MatchModel`. HTTP-метод обновления матча для контроллера не определён.

## 9. REST API

Swagger доступен в среде Development по адресу `/swagger`.

Базовый профиль запуска задаёт HTTP-порт `5117`.

### 9.1. Администраторы

| Метод | Маршрут | Назначение |
|---|---|---|
| `GET` | `/api/administrators` | Список всех администраторов |
| `GET` | `/api/administrators/{id}` | Администратор по Guid |
| `GET` | `/api/administrators/by-username/{username}` | Администратор по имени |
| `POST` | `/api/administrators` | Создание |
| `PUT` | `/api/administrators/{id}` | Обновление имени |
| `DELETE` | `/api/administrators/{id}` | Удаление |

`POST` возвращает `409 Conflict`, если имя занято. `PUT` возвращает `404`, если сущность отсутствует, и `409`, если новое имя занято другим администратором.

### 9.2. Пользователи

| Метод | Маршрут | Назначение |
|---|---|---|
| `GET` | `/api/users` | Список пользователей |
| `GET` | `/api/users/{id}` | Пользователь по Guid |
| `GET` | `/api/users/by-username/{username}` | Пользователь по имени |
| `POST` | `/api/users` | Создание |
| `PUT` | `/api/users/{id}` | Обновление имени |
| `DELETE` | `/api/users/{id}` | Удаление |

Пример запроса на создание:

```json
{
  "username": "PlayerOne"
}
```

Имя должно быть от 3 до 30 символов.

### 9.3. Матчи

| Метод | Маршрут | Назначение |
|---|---|---|
| `GET` | `/api/matches` | Список матчей |
| `GET` | `/api/matches/{id}` | Матч по Guid |
| `POST` | `/api/matches` | Создание матча |
| `DELETE` | `/api/matches/{id}` | Удаление матча |

Создание вызывает доменный метод `Administrator.CreateMatch`. Для сохранения репозиторий вызывает `SaveChangesAsync`.

На HTTP-уровне отсутствуют маршруты для планирования времени, старта, ввода счёта, редактирования и оспаривания матча. Эти сценарии реализованы в домене и application-интерфейсах репозиториев, но ещё не отображены контроллером.

### 9.4. Формат ошибок

Контроллеры в основном используют стандартные статусы ASP.NET Core:

- `200 OK`;
- `201 Created`;
- `204 No Content`;
- `400 Bad Request`;
- `404 Not Found`;
- `409 Conflict`.

Глобальный обработчик исключений не настроен. Доменные исключения поэтому могут потребовать явной обработки в будущих контроллерных методах.

## 10. Инфраструктура и база данных

### 10.1. ApplicationDbContext

`ApplicationDbContext` определяет наборы:

- `Users`;
- `Administrators`;
- `Matches`;
- `Tournaments`;
- `Brackets`.

Конфигурации всех типов собираются из текущей сборки через `modelBuilder.ApplyConfigurationsFromAssembly`.

### 10.2. Конфигурация репозиториев

В `Program.cs` зарегистрированы сервисы:

- `EfAdministratorRepository`;
- `EfUserRepository`;
- `EfMatchRepository`;
- `EfTournamentRepository`;
- `EfBracketRepository`;
- соответствующие application-службы.

Generic `EfRepository<TEntity, TId>` реализует получение, добавление, обновление, удаление и проверку существования сущности.

Специализированные репозитории используют запросы EF Core по объектам-значениям или примитивам.

### 10.3. Схема базы данных

Основные таблицы:

```text
Administrators
├── Id UUID, первичный ключ
└── Username varchar(30), обязательное поле

Users
├── Id UUID, первичный ключ
└── Username varchar(30), обязательное поле

Tournaments
└── Id UUID, первичный ключ

Brackets
└── Id UUID, первичный ключ

Matches
├── Id UUID
├── TournamentId UUID, FK, Restrict
├── BracketId UUID, FK, Restrict
├── AdministratorId UUID, FK, Restrict
├── Player1Id UUID, FK, Restrict
├── Player2Id UUID, FK, Restrict
├── WinnerId UUID, FK, SetNull
├── Status text
├── ScheduledTime timestamp with time zone
├── StartedAt timestamp with time zone
├── CompletedAt timestamp with time zone
├── CreatedAt timestamp with time zone
├── MapName varchar(30)
├── ServerInfo varchar(120)
├── Player1Score integer
└── Player2Score integer
```

Связи матча с турниром, сеткой, игроками и администратором запрещают каскадное удаление через `DeleteBehavior.Restrict`. Победитель при удалении пользователя может сбрасываться как `null` благодаря `DeleteBehavior.SetNull`.

Статус матча хранится строкой благодаря `.HasConversion<string>()`, а не числовым значением enum.

Временные поля переводятся в UTC-подобное представление на уровне конфигурации EF. Для корректной работы PostgreSQL также используется тип `timestamp with time zone`.

### 10.4. Миграции

В проекте есть миграция `20260513203108_InitialInfrastructureFix`.

При запуске хоста вызывается метод расширения:

```csharp
app.MigrateDatabase<ApplicationDbContext>();
```

Он создаёт область DI, получает `ApplicationDbContext` и вызывает `Database.Migrate()`. Поэтому база должна быть доступна уже при старте приложения.

Для ручного запуска миграций можно из корня решения использовать:

```powershell
dotnet ef database update `
  --project Infrastructure\MatchService.Infrastructure.EntityFramework `
  --startup-project Presentation\MatchService.WebHost
```

## 11. Конфигурация и запуск

### 11.1. Предварительные требования

- установленный .NET 8 SDK;
- локальный или удалённый PostgreSQL;
- доступ к порту базы данных;
- права пользователя БД на создание и изменение схемы;
- Visual Studio или VS Code с расширением C# — по желанию.

### 11.2. Настройка строки подключения

Основная строка находится в `Presentation/MatchService.WebHost/appsettings.json`:

```json
"ConnectionStrings": {
  "ApplicationDbContext": "Host=localhost;Port=5433;Database=MatchServiceDb;UserName=postgres;Password=1234"
}
```

Для реальной среды пароль не следует хранить в репозитории. Рекомендуется заменить значение на переменную среды или пользовательский Secret Manager:

```powershell
dotnet user-secrets init --project Presentation\MatchService.WebHost
dotnet user-secrets set "ConnectionStrings:ApplicationDbContext" "Host=localhost;Port=5432;Database=MatchServiceDb;UserName=postgres;Password=<пароль>" --project Presentation\MatchService.WebHost
```

При старте приложение завершится с исключением `InvalidOperationException`, если строка подключения отсутствует.

### 11.3. Сборка

```powershell
dotnet restore Eventide.MatchService.sln
dotnet build Eventide.MatchService.sln --configuration Release
```

### 11.4. Запуск

```powershell
dotnet run --project Presentation\MatchService.WebHost
```

Профиль `http` доступен по адресу:

```text
http://localhost:5117/swagger
```

Профиль `https` содержит также порт `7084`.

### 11.5. Docker и контейнеры

Docker-файлы в репозитории отсутствуют. Для развёртывания PostgreSQL можно использовать контейнер вручную, а параметр `Host` в строке подключения должен указывать имя контейнера или доступный хост сервиса.

## 12. Тестирование и качество

### 12.1. Запуск тестов

Базовая команда:

```powershell
dotnet test Tests\MatchService.Tests\MatchService.Tests.csproj --configuration Release
```

Полный пример с сбором покрытия:

```powershell
dotnet test Tests\MatchService.Tests\MatchService.Tests.csproj `
  --configuration Release `
  --settings Tests\MatchService.Tests\coverage.runsettings `
  --collect:"XPlat Code Coverage"
```

Тесты разделены по предметным областям:

- `MatchTests`;
- `AdministratorTests`;
- `ValueObjectTests`;
- `MatchApplicationServiceTests`;
- `UserApplicationServiceTests`;
- `AdministratorApplicationServiceTests`;
- `MatchRepositoryTests`;
- `AdministratorRepositoryTests`;
- `MatchesControllerTests`;
- `UsersControllerTests`;
- `AdministratorsControllerTests`;
- тесты дополнительного покрытия и DTO.

### 12.2. Покрытие

GitHub Actions требует не менее 95% line coverage. Текущее фактическое покрытие после расширения тестов составляет 98.52%. Конвейер:

1. проверяет ветки репозитория;
2. восстанавливает зависимости;
3. собирает решение в Release;
4. запускает тестовый проект;
5. собирает Cobertura-отчёт;
6. проверяет `line-rate`;
7. загружает отчёт как артефакт.

Файл отчёта определяется как первый найденный `coverage.cobertura.xml` в `TestResults`.

### 12.3. Рекомендуемый цикл изменений

Перед изменением доменной логики следует добавить тест, который фиксирует состояние до и после операции. После изменения запускается узкий тестовый класс, затем вся тестовая сборка и сборка решения.

## 13. Событийные контракты

Тип `MatchCompletedEvent` содержит:

- `MatchId`;
- `WinnerId`;
- `LoserId`;
- `WinnerScore`;
- `LoserScore`.

Контракт является полным только в смысле структуры типа. В проекте нет опубликованного вызова создания события в методе `SubmitResult`, нет издателя и нет подписчика. Поэтому класс нельзя считать уже работающей асинхронной интеграцией.

Для реального внедрения требуется:

- создать событие после сохранения завершённого матча;
- выбрать брокер;
- определить гарантии доставки;
- обеспечить идемпотентность обработчиков;
- настроить сериализацию и метаданные;
- покрыть сценарии Outbox-паттерном или эквивалентным механизмом.

## 14. Известные ограничения и несогласованности

Ниже приведены фактические расхождения или технические риски текущей ветки.

1. В доменном проекте есть `Match.Cancel()` и комментарий в `Administrator.CancelMatch`, но проверка статуса `Scheduled` отсутствует.
2. `Match.Dispute()` не проверяет исходный статус `Completed`; это проверяется косвенно через `User.DisputeMatch`.
3. В application-службе матча отсутствует полноценная реализация `UpdateModelAsync`.
4. Контроллер матча предоставляет только создание, чтение и удаление.
5. Нет HTTP-контроллеров для операций старта, ввода счёта, переноса времени и оспаривания.
6. Нет middleware глобальной обработки доменных исключений.
7. Нет аутентификации и авторизации.
8. В строке подключения репозитория указан пароль. Его следует вынести из исходного кода.
9. При запуске миграции выполняются автоматически для всех сред.
10. Библиотека контрактов не подключена к реальному брокеру.
11. В существующих документах ранее встречались неточные сведения о расположении и составе файлов. Настоящий документ сверен с фактической структурой репозитория.
12. Типы `Tournament` и `Bracket` минимальны и не отражают полный формат предметной области.

Эти пункты не следует скрывать в курсовой работе: они демонстрируют границы MVP и точки развития продукта.

## 15. Рекомендации по развитию

### 15.1. Домен

- ограничить `Cancel()` статусом `Scheduled`;
- проверять в `Dispute()` статус `Completed`;
- добавить явные методы переходов вместо общего `Edit`;
- опубликовать доменные события внутри агрегата;
- добавить проверку запрета выигрыша одного игрока.

### 15.2. API

- добавить `PATCH` или конкретные `POST`-маршруты для жизненного цикла;
- добавить DTO для старта, переноса и результата;
- добавить глобальный `ExceptionHandler`;
- вернуть содержательное описание ошибок в формате `ProblemDetails`;
- задокументировать обязательные поля с помощью атрибутов валидации.

### 15.3. Инфраструктура

- вынести миграции из автоматического запуска в CI или процесс развёртывания;
- вынести секреты во внешнюю конфигурацию;
- добавить индексы и уникальные ограничения;
- определить ограничения уникальности имён на уровне БД;
- настроить включение связанных сущностей в запросах репозиториев.

### 15.4. Безопасность

- внедрить аутентификацию через OpenID Connect или JWT;
- ограничить административные операции ролями;
- проверять право администратора над конкретным матчем;
- отключить служебные Swagger-узлы вне Development;
- включить HTTPS-хостинг в production-конфигурации.

### 15.5. Интеграции

- добавить издателя `MatchCompletedEvent`;
- использовать паттерн transactional outbox;
- определить контракт версионирования;
- добавить retry и идемпотентность;
- подключить health checks базы данных и внешних сервисов.

## 16. Список литературы

1. Microsoft. *ASP.NET Core documentation*. — https://learn.microsoft.com/aspnet/core/
2. Microsoft. *Entity Framework Core documentation*. — https://learn.microsoft.com/ef/core/
3. Microsoft. *Npgsql Entity Framework Core Provider*. — https://www.npgsql.org/efcore/
4. Microsoft. *Domain-Driven Design resource*. — https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/
5. Microsoft. *AutoMapper documentation*. — https://docs.automapper.org/
6. PostgreSQL Global Development Group. *PostgreSQL documentation*. — https://www.postgresql.org/docs/
7. xUnit.net. *Documentation*. — https://xunit.net/
8. GitHub. *GitHub Actions documentation*. — https://docs.github.com/actions
9. Martin Fowler. *Patterns of Enterprise Application Architecture*. Addison-Wesley, 2002.
10. Eric Evans. *Domain-Driven Design: Tackling Complexity in the Heart of Software*. Addison-Wesley, 2003.

## Приложение A. Краткая справочная последовательность разработки

```powershell
git clone <url-репозитория>
cd MatchService

dotnet restore Eventide.MatchService.sln
dotnet build Eventide.MatchService.sln --configuration Release

# Настроить PostgreSQL и строку подключения
dotnet run --project Presentation\MatchService.WebHost

# Swagger
# http://localhost:5117/swagger

# Тесты
dotnet test Tests\MatchService.Tests\MatchService.Tests.csproj --configuration Release
```
