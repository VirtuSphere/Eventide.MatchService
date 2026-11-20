---
title: "Курсовая работа — микросервис MatchService"
lang: ru-RU
---

# КУРСОВАЯ РАБОТА

**Тема:** разработка микросервиса управления матчами **MatchService** в составе платформы VirtuSphere на базе .NET 8, PostgreSQL и Entity Framework Core с применением идей Domain-Driven Design и многослойной («чистой») архитектуры.

## 1. ТИТУЛЬНЫЙ ЛИСТ

Оформляется по локальному шаблону учебного заведения (полное наименование вуза и кафедры, вид работы, тема, ФИО студента, группа, направление подготовки, руководитель, подписи, город, год). Техническое наполнение курсовой излагается в электронном виде в настоящем файле и в производном документе `coursework.docx`.

## 2. РЕФЕРАТ

**Объём работы (электронный документ):** основное содержание сосредоточено в разделах 5–7; разделы 1–3 и 8–10 приведены в объёме, достаточном для включения в отчёт по требованиям методических указаний.

**Цель:** спроектировать и реализовать доменную модель и инфраструктурный слой хранения для микросервиса **MatchService**, обеспечивающего жизненный цикл матча в контексте турнира и турнирной сетки.

**Объект исследования:** программное обеспечение распределённой платформы проведения соревнований.

**Предмет исследования:** методы структурирования кода (DDD, разделение слоёв), объектно-реляционное отображение (EF Core), публикация контрактов для межсервисной интеграции.

**Результаты:** в репозитории реализованы проекты домена (`MatchService.Domain`, `MatchService.ValueObjects`, абстракции репозиториев), инфраструктуры на EF Core с миграциями PostgreSQL, хост Web API со Swagger и автоматическим применением миграций при старте, библиотека контрактов событий.

**Репозиторий исходного кода:** https://github.com/hyesos/VirtuSphere

## 3. СОДЕРЖАНИЕ

1. Титульный лист  
2. Реферат  
3. Содержание  
4. Введение  
5. Описание объекта автоматизации и требований к создаваемой программе  
   5.1. Платформа VirtuSphere и место MatchService  
   5.2. Краткая характеристика смежных микросервисов  
   5.3. Предметная область MatchService: сущности и роли  
   5.4. Сценарии и потоки работ (жизненный цикл матча)  
   5.5. Диаграмма прецедентов (текстовая)  
   5.6. Функциональные требования и трассировка к доменной модели  
   5.7. Нефункциональные требования  
   5.8. Концептуальная модель данных и интеграции  
   5.9. Замечания по согласованности правил (код против комментариев)  
6. Архитектурные основы разработки  
   6.1. Domain-Driven Design: стратегические и тактические паттерны  
   6.2. Clean Architecture и правило зависимостей  
   6.3. Entity Framework Core и роль инфраструктурного слоя  
   6.4. Контракты и событийная интеграция  
   6.5. Сводная таблица «теория — артефакт в проекте»  
   6.6. Библиография и официальные источники  
7. Программная реализация микросервиса «MatchService»  
   7.1. Состав решения и зависимости между проектами  
   7.2. Доменный слой: базовые абстракции  
   7.3. Агрегат матча и связанные типы  
   7.4. Объекты-значения и валидация  
   7.5. Администратор как фасад операций над матчами  
   7.6. Игрок и сценарий диспута  
   7.7. Инфраструктура: `ApplicationDbContext` и конфигурации EF  
   7.8. Репозиторий `EfRepository`  
   7.9. Слой представления: запуск, Swagger, миграции  
   7.10. Схема базы данных (по снимку модели)  
   7.11. Контракты событий  
8. Заключение  
9. Список литературы  
10. Приложение  

## 4. ВВЕДЕНИЕ

Киберспортивные и игровые турниры сопровождаются большим числом однотипных, но юридически и организационно значимых операций: составление сетки, назначение сервера, фиксация результата, обработка спорных ситуаций. Автоматизация на основе **микросервисов** позволяет масштабировать отдельные части платформы и развивать их независимо. При этом возрастает цена ошибки в **границах контекста**: неверно смоделированный жизненный цикл матча ведёт к противоречивым данным и к сложности интеграции с турнирным и пользовательским сервисами.

**Актуальность** темы связана с необходимостью:

- явно зафиксировать **инварианты** (разные участники, допустимость победителя, ограничения на счёт и строковые поля) в коде домена, а не только в UI или в SQL-триггерах;  
- обеспечить **устойчивость к смене технологий** хранения за счёт изоляции EF Core и Npgsql в инфраструктурном проекте;  
- подготовить **контракты** (`Eventide.MatchService.Contracts`) для асинхронной коммуникации с другими сервисами.

**Цель курсовой работы** — описать объект автоматизации и требования к ПО, обосновать теоретическую базу (DDD, многослойная архитектура, EF Core) и показать реализацию на примере текущей ветки репозитория MatchService.

**Задачи:** (1) проанализировать состав платформы и роль MatchService; (2) сформулировать функциональные и нефункциональные требования; (3) сопоставить их с реализацией в домене и инфраструктуре; (4) привести фрагменты листинга с пояснениями.

---

## 5. ОПИСАНИЕ ОБЪЕКТА АВТОМАТИЗАЦИИ И ТРЕБОВАНИЙ К СОЗДАВАЕМОЙ ПРОГРАММЕ

### 5.1. Платформа VirtuSphere и место MatchService

**VirtuSphere** (в логике курсового проекта) — распределённая платформа проведения турниров. Её можно представить как набор **ограниченных контекстов**, каждый из которых имеет собственную модель данных и публичный интерфейс (HTTP API, очередь сообщений, gRPC и т.д.). **MatchService** отвечает за сущность «матч» в смысле: кто играет, в рамках какого турнира и какой ячейки сетки, в каком **статусе** находится встреча, на какой **карте** и **сервере** она проходит, каков **итог** и был ли **диспут**.

Граница сервиса проходит по правилу: *всё, что относится к правилам изменения состояния одной игровой встречи и её атрибутов в рамках уже выбранного турнира/сетки, принадлежит MatchService*. Выбор победителя турнира целиком, рейтинговая математика, рассылка писем — **за пределами** детализируемой здесь модели, но **потребляют** результаты матча.

### 5.2. Краткая характеристика смежных микросервисов

Ниже — **сжатое** описание соседних контекстов. Детализация соответствует типичной декомпозиции и ссылкам в коде (идентификаторы турнира/сетки/пользователей), без утверждения, что все перечисленные сервисы уже развёрнуты в том же репозитории.

| Контекст | Функциональность (кратко) | Интеграция с MatchService |
|----------|---------------------------|---------------------------|
| **Identity / UserService** | Регистрация, аутентификация, профиль | MatchService хранит `User` с `Id` и `Username`; при полной платформе идентификаторы синхронизируются с внешним каталогом пользователей. |
| **TournamentService** | Создание турнира, статусы, расписание этапов | В матче хранится ссылка `TournamentId`; расширение атрибутов турнира не дублируется в MatchService (в текущем коде `Tournament` минимален). |
| **Bracket / Schedule** | Построение сетки, ячейки, продвижение | В матче хранится `BracketId`; логика «кому с кем в следующем раунде» может жить в отдельном сервисе. |
| **NotificationService** | Уведомления о старте, результате, диспуте | Подписывается на события вида `MatchCompletedEvent` (контракт в `Contracts`). |
| **Statistics** | Агрегаты по игрокам и турнирам | Читает завершённые матчи (победитель, счёт). |

### 5.3. Предметная область MatchService: сущности и роли

**Сущность `Match` (корень агрегата).** Объединяет:

- ссылки на **турнир** и **сетку** (`Tournament`, `Bracket`);  
- **администратора**, от имени которого в доменной модели создаётся матч;  
- двух **игроков** (`Player1`, `Player2`) и опционально **победителя**;  
- **статус** из перечисления `MatchStatus`: `Scheduled`, `InProgress`, `Completed`, `Disputed`, `Cancelled`;  
- временные метки `ScheduledTime`, `StartedAt`, `CompletedAt`;  
- игровые атрибуты: `MapName`, `ServerInfo` (заполняются при старте);  
- счёт `Player1Score`, `Player2Score` (заполняются при отправке результата).

**Сущность `User`.** Участник; несёт `Username` как объект-значение. Содержит поведение `DisputeMatch`, делегирующее матчу оспаривание с **предварительной** проверкой статуса `Completed`.

**Сущность `Administrator`.** В текущей модели выступает **оркестратором** операций: `CreateMatch`, `StartMatch`, `SubmitMatchResult`, `RescheduleMatch`, `EditMatch`, `CancelMatch`, `DisputeMatch`, `DeleteMatch`. Внутри используется коллекция `_matches` для проверки принадлежности матча администратору (логика «только свои матчи»).

**Сущности `Tournament` и `Bracket`.** На текущем этапе разработки представлены **минимальными** классами с идентификатором; предметная область зарезервирована под дальнейшее обогащение (название турнира, формат сетки и т.д.) без ломки связей в `Match`.

### 5.4. Сценарии и потоки работ (жизненный цикл матча)

**Сценарий A — создание.** Администратор вызывает `CreateMatch(tournament, bracket, player1, player2)`. В конструкторе `Match` проверяется, что `player1.Id != player2.Id`; при нарушении выбрасывается `MatchPlayerSameException`. Статус — `Scheduled`.

**Сценарий B — планирование времени.** `Match.Schedule(time)` допускается только в статусе `Scheduled`; иначе `MatchNotScheduledException`.

**Сценарий C — старт.** `Match.Start(serverInfo, mapName)` требует статус `Scheduled`, непустые `ServerInfo` и `MapName`; переводит в `InProgress`, фиксирует `StartedAt` в UTC.

**Сценарий D — правки в ходе игры и до неё.** Метод `Match.Edit(...)` по отдельным веткам: победитель может уточняться только в `InProgress`; время — только в `Scheduled`; карта и сервер — только в `InProgress`.

**Сценарий E — результат.** `SubmitResult(winner, player1Score, player2Score)` требует `InProgress`; проверяет, что `winner` совпадает по ссылке с `Player1` или `Player2`; выставляет счёт, статус `Completed`, `CompletedAt`.

**Сценарий F — диспут.** Два входа: `Match.Dispute()` (без проверки исходного статуса в теле метода) и `Match.Dispute(User user)` с проверкой участника. Снаружи `User.DisputeMatch` требует `Completed` до вызова `match.Dispute(this)`.

**Сценарий G — отмена.** `Match.Cancel()` возвращает `false`, если уже `Cancelled`; иначе выставляет `Cancelled`. Комментарий к `Administrator.CancelMatch` в коде утверждает ограничение «только Scheduled»; **фактическая реализация** `Match.Cancel` этого не проверяет — см. п. 5.9.

### 5.5. Диаграмма прецедентов (текстовая)

```
                    ┌──────────────┐
                    │  Игрок User  │
                    └──────┬───────┘
                           │
         ┌─────────────────┼─────────────────┐
         ▼                 ▼                 ▼
  Оспорить матч     (косвенно участие    Просмотр (через API,
  DisputeMatch       в матче как Player*)  вне домена в репозитории)

                    ┌──────────────────┐
                    │ Administrator    │
                    └────────┬─────────┘
                             │
    Создать матч ────────────┼─────────── Старт / результат / перенос / отмена
    CreateMatch              │           StartMatch, Submit..., Reschedule...
                             │
                    ┌────────▼─────────┐
                    │    Match (AR)    │
                    └──────────────────┘
```

\*В текущем решении HTTP-контроллеры не найдены в дереве `Presentation`; сценарии вызываются из приложения, тестов или будущего слоя приложения.

### 5.6. Функциональные требования и трассировка к доменной модели

| ID | Требование | Элемент реализации |
|----|------------|-------------------|
| ФТ-01 | Нельзя создать матч одного игрока с самим собой | `Match` protected ctor → `MatchPlayerSameException` |
| ФТ-02 | Планирование времени только для запланированного матча | `Match.Schedule` |
| ФТ-03 | Старт только из `Scheduled` с обязательной картой и сервером | `Match.Start` |
| ФТ-04 | Результат только из `InProgress`, победитель ∈ игроков | `Match.SubmitResult` |
| ФТ-05 | Оспаривание игроком только после завершения | `User.DisputeMatch` + `Match.Dispute(User)` |
| ФТ-06 | Точечное редактирование полей по статусам | `Match.Edit` |
| ФТ-07 | Операции администратора только над «своими» матчами | `Administrator.*` проверяет `_matches` |
| ФТ-08 | Сохранение сущностей в PostgreSQL | `ApplicationDbContext`, `EfRepository`, миграции |
| ФТ-09 | Подготовка данных для внешних подписчиков | `MatchCompletedEvent` и др. в `Contracts` |

**Объекты-значения и ограничения (по валидаторам в коде):**

- `PlayerScore`: целое **от 0 до 999** (`PlayerScoreValidator`).  
- `MapName`: строка **3–30** символов, не пустая (`MapNameValidator`; в конфигурации EF для поля также `HasMaxLength(MapNameValidator.MAX_LENGTH)`).  
- `ServerInfo`: непустая строка до **120** символов (`ServerInfoValidator`).  
- `Username`: в проекте задан `UsernameValidator.MAX_LENGTH` (используется в `UserConfiguration`).

### 5.7. Нефункциональные требования

**Производительность и масштабирование:** доступ к матчам через стандартные запросы EF; в снимке модели для `Matches` созданы индексы по внешним ключам и статусу — снижение стоимости типовых выборок по турниру, игроку, администратору.

**Надёжность и согласованность:** операции `AddAsync` / `UpdateAsync` / `DeleteAsync` в `EfRepository` завершаются `SaveChangesAsync`, что задаёт границу транзакции по умолчанию EF.

**Сопровождаемость:** доменные проекты не ссылаются на `Microsoft.EntityFrameworkCore`; конфигурация отображения сосредоточена в `Configurations/*.cs`.

**Безопасность эксплуатации:** в `ApplicationDbContext` не включается логирование чувствительных данных; строка подключения читается из конфигурации, при отсутствии — явное исключение в `Program.cs`.

**Наблюдаемость (перспектива):** публикация доменных событий в шину в текущем репозитории не показана в `Program.cs`; контракт `MatchCompletedEvent` готов к подключению.

### 5.8. Концептуальная модель данных и интеграции

**Связи «один-ко-многим / многие-к-одному» (логически по FK в снимке модели):**

- `Match` → `Tournament` (обязательно), `Bracket` (обязательно), `Administrator` (обязательно);  
- `Match` → `User` как `Player1`, `Player2` (обязательно), `Winner` (опционально, `ON DELETE SET NULL`).

**Интеграция наружу:** потребители могут подписаться на события с полями `MatchId`, `WinnerId`, `LoserId`, счёт победителя и проигравшего в `MatchCompletedEvent` — удобно для уведомлений и статистики без передачи всего агрегата.

### 5.9. Замечания по согласованности правил (код и комментарии)

При анализе исходников полезно зафиксировать расхождения — это часть инженерной зрелости требований:

1. **`Match.Cancel`** не ограничивает отмену статусом `Scheduled`, тогда как XML-комментарий у `Administrator.CancelMatch` описывает более строгое правило.  
2. **`Match.Dispute()`** не проверяет, что матч был `Completed`, в отличие от `User.DisputeMatch`. Для единообразия бизнес-правил имеет смысл централизовать проверку в `Match`.

Эти пункты не отменяют ценности курсовой, но показывают, что **трассировка требований к коду** должна опираться на фактическое поведение методов.

---

## 6. АРХИТЕКТУРНЫЕ ОСНОВЫ РАЗРАБОТКИ

### 6.1. Domain-Driven Design: стратегические и тактические паттерны

**Стратический уровень DDD** включает выделение **ограниченных контекстов** и языка предметной области (**ubiquitous language**). Для MatchService термины «матч», «старт», «результат», «диспут», «отмена» отражены в именах методов `Match` и `Administrator`.

**Тактические паттерны**, используемые в проекте:

- **Сущность (Entity)** — объекты с идентичностью (`Entity<TId>`, поле `Id`).  
- **Объект-значение (Value Object)** — `Username`, `MapName`, `ServerInfo`, `PlayerScore`; равенство по значению, валидация при создании.  
- **Агрегат и корень** — `Match` инкапсулирует переходы статуса и основные инварианты встречи.  
- **Доменные исключения** — `MatchNotInProgressException`, `MatchWinnerNotParticipantException` и др.  
- **Репозиторий (интерфейс)** — `IRepository<TEntity, TId>` в абстракциях домена; реализация в инфраструктуре.

**Учебная и классическая литература:**

- Evans E. *Domain-Driven Design: Tackling Complexity in the Heart of Software.* Addison-Wesley, 2003.  
- Vernon V. *Implementing Domain-Driven Design.* Addison-Wesley, 2013.

**Официальный обзор Microsoft по DDD в микросервисах:**  
https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/

### 6.2. Clean Architecture и правило зависимостей

**Clean Architecture** (R. C. Martin) требует, чтобы зависимости исходного кода направлялись **к абстракциям и политикам** в центре. В решении MatchService:

- **центр** — доменные типы и контракт репозитория;  
- **снаружи** — EF Core, Npgsql, ASP.NET Core.

Оригинальная схема и пояснения:  
https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html  

Книга: Martin R. C. *Clean Architecture: A Craftsman's Guide to Software Structure and Design.* Prentice Hall, 2017.

**Практическое следствие:** в домене отсутствуют атрибуты EF (`[Key]`, `Table` на сущностях); отображение задано в `IEntityTypeConfiguration<>`.

### 6.3. Entity Framework Core и роль инфраструктурного слоя

EF Core — ORM от Microsoft; документация:  
https://learn.microsoft.com/ef/core/

Ключевые темы, применённые в проекте:

- регистрация контекста и провайдера **Npgsql**;  
- **`ApplyConfigurationsFromAssembly`** для модульности Fluent API;  
- **value conversions** для VO и для приведения `DateTime` к UTC;  
- **миграции** как версионируемое описание схемы (`Infrastructure/.../Migrations`).

### 6.4. Контракты и событийная интеграция

Между командами и сервисами распространяют **контракты** — отдельные сборки без бизнес-логики. В решении: `Eventide.MatchService.Contracts` с типом `MatchCompletedEvent`. Публикация в брокер сообщений в коде может быть добавлена на уровне приложения без изменения домена.

### 6.5. Сводная таблица «теория — артефакт в проекте»

| Теория | Где проявлено в MatchService |
|--------|-------------------------------|
| Ubiquitous language | Имена методов `Start`, `SubmitResult`, `Dispute`, `Cancel` |
| Entity / VO | `Entity<Guid>`, `ValueObject<T>` + валидаторы |
| Aggregate | `Match` как центр инвариантов встречи |
| Repository | `IRepository<>` → `EfRepository<>` |
| Anti-corruption (задел) | DTO и контракты событий отдельно от домена |
| Clean Architecture | Нет ссылок домена на EF / ASP.NET |
| EF Core mappings | `Configurations/*.cs` |

### 6.6. Библиография и официальные источники (для раздела 9)

1. Evans E. *Domain-Driven Design* (2003).  
2. Vernon V. *Implementing Domain-Driven Design* (2013).  
3. Martin R. C. *Clean Architecture* (2017).  
4. Microsoft Learn — **EF Core**: https://learn.microsoft.com/ef/core/  
5. Microsoft Learn — **ASP.NET Core**: https://learn.microsoft.com/aspnet/core/  
6. Microsoft Learn — **Npgsql EF provider**: https://learn.microsoft.com/ef/core/providers/npgsql/  
7. Fowler M. *Patterns of Enterprise Application Architecture* (2002) — паттерн Repository, слои.  
8. Newman S. *Building Microservices* (2015) — границы сервисов, интеграции.

---

## 7. ПРОГРАММНАЯ РЕАЛИЗАЦИЯ МИКРОСЕРВИСА «MATCHSERVICE»

### 7.1. Состав решения и зависимости между проектами

Фактическая структура (по каталогам репозитория):

```
MatchService/
├── Domain/
│   ├── MatchService.Domain/                         — сущности, enum, исключения
│   │   ├── MatchService.Domain.csproj
│   │   ├── Match.cs
│   │   ├── User.cs
│   │   ├── Administrator.cs
│   │   ├── Tournament.cs
│   │   ├── Bracket.cs
│   │   ├── Base/
│   │   │   └── Entity.cs
│   │   ├── Enums/
│   │   │   └── MatchStatus.cs
│   │   └── Exceptions/
│   │       ├── DomainException.cs
│   │       ├── ArgumentNullValueException.cs
│   │       ├── InvalidMatchStatusException.cs
│   │       ├── InvalidMatchWinnerException.cs
│   │       ├── MatchNotFoundException.cs
│   │       ├── MatchNotInProgressException.cs
│   │       ├── MatchNotScheduledException.cs
│   │       ├── MatchPlayerSameException.cs
│   │       └── MatchWinnerNotParticipantException.cs
│   ├── MatchService.ValueObjects/                   — VO, валидаторы, базовый ValueObject
│   │   ├── MatchService.ValueObjects.csproj
│   │   ├── Username.cs
│   │   ├── MapName.cs
│   │   ├── ServerInfo.cs
│   │   ├── PlayerScore.cs
│   │   ├── Base/
│   │   │   ├── ValueObject.cs
│   │   │   └── IValidator.cs
│   │   ├── Validators/
│   │   │   ├── UsernameValidator.cs
│   │   │   ├── MapNameValidator.cs
│   │   │   ├── ServerInfoValidator.cs
│   │   │   └── PlayerScoreValidator.cs
│   │   └── Exceptions/
│   │       ├── ValidatorNullException.cs
│   │       ├── ArgumentNullOrWhiteSpaceException.cs
│   │       ├── ArgumentShortValueException.cs
│   │       ├── ArgumentLongValueException.cs
│   │       ├── PlayerScoreOutOfRangeException.cs
│   │       ├── ServerInfoNullOrWhiteSpaceException.cs
│   │       └── ServerInfoTooLongException.cs
│   ├── MatchService.Domain.Repositories.Abstractions/
│   │   ├── MatchService.Domain.Repositories.Abstractions.csproj
│   │   ├── Base/
│   │   │   └── IRepository.cs
│   │   └── Repositories/
│   │       ├── IUserRepository.cs
│   │       └── IAdministratorRepository.cs
│   └── DomainApp/                                   — консольное демо
│       ├── DomainApp.csproj
│       └── Program.cs
├── Infrastructure/
│   └── MatchService.Infrastructure.EntityFramework/
│       ├── MatchService.Infrastructure.EntityFramework.csproj
│       ├── ApplicationDbContext.cs
│       ├── Configurations/
│       │   ├── MatchConfiguration.cs
│       │   ├── UserConfiguration.cs
│       │   ├── AdministratorConfiguration.cs
│       │   ├── TournamentConfiguration.cs
│       │   └── BracketConfiguration.cs
│       ├── RepositoryEF/
│       │   ├── EFRepository.cs
│       │   ├── UserRepository.cs
│       │   └── AdministratorRepository.cs
│       └── Migrations/
│           ├── ApplicationDbContextModelSnapshot.cs
│           ├── 20260513203108_InitialInfrastructureFix.cs
│           └── 20260513203108_InitialInfrastructureFix.Designer.cs
├── Presentation/
│   └── MatchService.WebHost/                        — ASP.NET Core, Swagger, миграции при старте
│       ├── MatchService.WebHost.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── MatchService.WebHost.http
│       ├── Properties/
│       │   └── launchSettings.json
│       └── Heplers/
│           └── MigrationManager.cs
└── Contracts/
    ├── Eventide.MatchService.Contracts.csproj
    ├── Class1.cs                                    — заготовка шаблона (можно удалить)
    └── Events/
        └── MatchCompletedEvent.cs
```

*В каталогах сборки также появляются служебные файлы вроде `*.csproj.lscache` — в дереве не перечислены.*

**Направление зависимостей:** `Infrastructure` и `WebHost` зависят от домена и абстракций; `Contracts` не зависит от домена (удобно для внешних подписчиков).

### 7.2. Доменный слой: базовые абстракции

Базовый класс сущности задаёт **идентичность**:

```csharp
// Domain/MatchService.Domain/Base/Entity.cs (фрагмент)

public abstract class Entity<TId>(TId id) where TId : struct, IEquatable<TId>
{
    public TId Id { get; } = id;
    protected Entity() : this(default!) { }
}
```

Базовый **объект-значение** делегирует проверку валидатору и реализует равенство по `Value`:

```csharp
// Domain/MatchService.ValueObjects/Base/ValueObject.cs (фрагмент)

public abstract class ValueObject<T> : IEquatable<ValueObject<T>>
{
    public T Value { get; }
    protected ValueObject(IValidator<T> validator, T value)
    {
        if (validator == null)
            throw new ValidatorNullException(nameof(validator));
        validator.Validate(value);
        Value = value;
    }
    // Equals / GetHashCode / операторы == !=
}
```

### 7.3. Агрегат матча и связанные типы

Перечисление статусов:

```csharp
// Domain/MatchService.Domain/Enums/MatchStatus.cs

public enum MatchStatus
{
    Scheduled,
    InProgress,
    Completed,
    Disputed,
    Cancelled
}
```

**Ключевой фрагмент** жизненного цикла — старт и фиксация результата (инварианты участников и статуса):

```csharp
// Domain/MatchService.Domain/Match.cs (фрагмент)

public bool Start(ServerInfo serverInfo, MapName mapName)
{
    serverInfo = serverInfo ?? throw new ArgumentNullValueException(nameof(serverInfo));
    mapName = mapName ?? throw new ArgumentNullValueException(nameof(mapName));

    if (Status != MatchStatus.Scheduled)
        throw new MatchNotScheduledException(Status);

    ServerInfo = serverInfo;
    MapName = mapName;
    Status = MatchStatus.InProgress;
    StartedAt = DateTime.UtcNow;
    return true;
}

public bool SubmitResult(User winner, PlayerScore player1Score, PlayerScore player2Score)
{
    winner = winner ?? throw new ArgumentNullValueException(nameof(winner));
    player1Score = player1Score ?? throw new ArgumentNullValueException(nameof(player1Score));
    player2Score = player2Score ?? throw new ArgumentNullValueException(nameof(player2Score));

    if (Status != MatchStatus.InProgress)
        throw new MatchNotInProgressException(Status);

    if (winner != Player1 && winner != Player2)
        throw new MatchWinnerNotParticipantException(winner, Player1, Player2);

    Winner = winner;
    Player1Score = player1Score;
    Player2Score = player2Score;
    Status = MatchStatus.Completed;
    CompletedAt = DateTime.UtcNow;
    return true;
}
```

Конструирование с проверкой «не играет сам с собой»:

```csharp
// Domain/MatchService.Domain/Match.cs (фрагмент ctor)

if (player1.Id == player2.Id)
    throw new MatchPlayerSameException();
```

### 7.4. Объекты-значения и валидация

Пример **диапазона счёта** (важно для требований к ПО — не выдумывать «0–100», а смотреть код):

```csharp
// Domain/MatchService.ValueObjects/Validators/PlayerScoreValidator.cs (фрагмент)

public static int MIN_VALUE => 0;
public static int MAX_VALUE => 999;

public void Validate(int value)
{
    if (value < MIN_VALUE || value > MAX_VALUE)
        throw new PlayerScoreOutOfRangeException(nameof(value), value, MIN_VALUE, MAX_VALUE);
}
```

Определение счёта как VO:

```csharp
// Domain/MatchService.ValueObjects/PlayerScore.cs

public class PlayerScore(int value) : ValueObject<int>(new PlayerScoreValidator(), value);
```

### 7.5. Администратор как фасад операций над матчами

Ниже показано, как доменная модель выражает **политику доступа** администратора к матчу через коллекцию `_matches`:

```csharp
// Domain/MatchService.Domain/Administrator.cs (фрагмент)

public Match CreateMatch(Tournament tournament, Bracket bracket, User player1, User player2)
{
    // проверки на null ...
    var match = new Match(tournament, bracket, this, player1, player2);
    if (_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
    _matches.Add(match);
    return match;
}

public bool StartMatch(Match match, ServerInfo serverInfo, MapName mapName)
{
    if (match == null) throw new ArgumentNullValueException(nameof(match));
    if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
    return match.Start(serverInfo, mapName);
}

public bool SubmitMatchResult(Match match, User winner, PlayerScore player1Score, PlayerScore player2Score)
{
    if (match == null) throw new ArgumentNullValueException(nameof(match));
    if (!_matches.Contains(match)) throw new MatchNotFoundException(match.Id);
    // ...
    match.SubmitResult(winner, player1Score, player2Score);
    return true;
}
```

> **Примечание для разработки:** при чисто реляционном хранении матчей коллекция `_matches` в памяти не синхронизируется с БД автоматически; для продакшена обычно добавляют запрос «матчи по AdministratorId» или переносят проверку владельца в сервис прикладного слоя.

### 7.6. Игрок и сценарий диспута

```csharp
// Domain/MatchService.Domain/User.cs (фрагмент)

public bool DisputeMatch(Match match)
{
    if (match == null) throw new ArgumentNullValueException(nameof(match));
    if (match.Status != MatchStatus.Completed) throw new InvalidMatchStatusException(match.Id, match.Status);
    match.Dispute(this);
    return true;
}
```

### 7.7. Инфраструктура: `ApplicationDbContext` и конфигурации EF

```csharp
// Infrastructure/MatchService.Infrastructure.EntityFramework/ApplicationDbContext.cs

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Administrator> Administrators { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Bracket> Brackets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
```

**Особенность `MatchConfiguration`:** статус хранится как **строка** (`HasConversion<string>()`), для дат — конвертеры с нормализацией **UTC**, для `MapName`, `ServerInfo`, счёта — конвертеры между колонками и VO.

```csharp
// Infrastructure/.../Configurations/MatchConfiguration.cs (фрагмент)

builder.Property(m => m.Status).IsRequired().HasConversion<string>();

builder.Property(m => m.MapName)
    .IsRequired(false)
    .HasConversion(
        mapName => mapName != null ? mapName.Value : null,
        str => str != null ? new MapName(str) : null)
    .HasMaxLength(MapNameValidator.MAX_LENGTH);
```

**Пример VO в `User`:**

```csharp
// Infrastructure/.../Configurations/UserConfiguration.cs (фрагмент)

builder.Property(u => u.Username)
    .IsRequired()
    .HasConversion(username => username.Value, str => new Username(str))
    .HasMaxLength(UsernameValidator.MAX_LENGTH);
```

### 7.8. Репозиторий `EfRepository`

Интерфейс в домене (`IRepository`) объявляет `GetAllAsync`, `GetByIdAsync`, `ExistsAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync(entity)`.

Реализация выполняет **немедленное** сохранение после операций изменения:

```csharp
// Infrastructure/.../RepositoryEF/EFRepository.cs (фрагмент)

public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));
    await context.Set<TEntity>().AddAsync(entity, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);
}

public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(entity, nameof(entity));
    context.Set<TEntity>().Update(entity);
    await context.SaveChangesAsync(cancellationToken);
}
```

Дополнительно класс предоставляет `DeleteAsync(TId id)` — удобный метод, не объявленный в интерфейсе `IRepository` текущей версии.

### 7.9. Слой представления: запуск, Swagger, миграции

```csharp
// Presentation/MatchService.WebHost/Program.cs (фрагмент)

builder.Services.AddNpgsql<ApplicationDbContext>(connectionString, options =>
{
    options.MigrationsAssembly("MatchService.Infrastructure.EntityFramework");
});

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(/* ... */);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.MigrateDatabase<ApplicationDbContext>();
app.Run();
```

Расширение `MigrateDatabase` применяет миграции при старте:

```csharp
// Presentation/MatchService.WebHost/Heplers/MigrationManager.cs

public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
{
    var scope = host.Services.CreateScope();
    var appContext = scope.ServiceProvider.GetService<T>();
    appContext?.Database.Migrate();
    return host;
}
```

### 7.10. Схема базы данных (по `ApplicationDbContextModelSnapshot`)

| Таблица | Назначение | Заметки |
|---------|------------|---------|
| `Users` | Игроки | `Username` varchar(30) |
| `Administrators` | Администраторы | `Username` varchar(30) |
| `Tournaments` | Турниры | минимальная модель (Id) |
| `Brackets` | Сетки | минимальная модель (Id) |
| `Matches` | Матчи | FK на Users, Administrator, Tournament, Bracket; индексы по ключам |

Колонки `Matches` включают: `Status` (тип `text` в снимке), `MapName`, `ServerInfo`, nullable счёт и временные поля в `timestamp with time zone`.

### 7.11. Контракты событий

```csharp
// Contracts/Events/MatchCompletedEvent.cs

public class MatchCompletedEvent
{
    public Guid MatchId { get; init; }
    public Guid WinnerId { get; init; }
    public Guid LoserId { get; init; }
    public int WinnerScore { get; init; }
    public int LoserScore { get; init; }
}
```

---

## 8. ЗАКЛЮЧЕНИЕ

В работе рассмотрен объект автоматизации — проведение матчей в составе платформы VirtuSphere — и сформулированы требования к программе, опираясь на фактическую доменную модель и схему БД. Показано применение идей **DDD** (агрегат матча, объекты-значения, доменные исключения, репозиторий) и **разделения слоёв** с выносом EF Core в инфраструктуру. Реализация на **.NET 8** с **PostgreSQL** и автоматическими миграциями демонстрирует связку теории и практики. Направления развития: выравнивание бизнес-правил (`Cancel`, `Dispute`), прикладной слой с use-case сервисами, HTTP-эндпоинты поверх существующей модели, публикация контрактов в шину сообщений.

## 9. СПИСОК ЛИТЕРАТУРЫ

Оформление по ГОСТ 7.0.5 / требованиям кафедры; ниже — библиографические описания для переноса.

1. Evans E. Domain-Driven Design: Tackling Complexity in the Heart of Software. Boston: Addison-Wesley, 2003.  
2. Vernon V. Implementing Domain-Driven Design. Boston: Addison-Wesley, 2013.  
3. Martin R. C. Clean Architecture: A Craftsman's Guide to Software Structure and Design. Boston: Prentice Hall, 2017.  
4. Fowler M. Patterns of Enterprise Application Architecture. Boston: Addison-Wesley, 2002.  
5. Newman S. Building Microservices. Sebastopol: O’Reilly Media, 2015.  
6. Документация Microsoft Learn: Entity Framework Core URL: https://learn.microsoft.com/ef/core/  
7. Документация Microsoft Learn: ASP.NET Core URL: https://learn.microsoft.com/aspnet/core/  
8. Документация Microsoft Learn: Npgsql EF Core Provider URL: https://learn.microsoft.com/ef/core/providers/npgsql/  
9. Документация PostgreSQL URL: https://www.postgresql.org/docs/  

## 10. ПРИЛОЖЕНИЕ

**Приложение А.** Полные листинги файлов из репозитория (по необходимости объёма отчёта): `Match.cs`, `Administrator.cs`, `MatchConfiguration.cs`, файлы миграций.

**Приложение Б.** Скриншоты Swagger UI после запуска `MatchService.WebHost`.

**Приложение В.** Диаграммы (Use Case, ER, компоненты) в графическом редакторе по текстовым эскизам из п. 5.5 и 5.8.
