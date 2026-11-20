# 📁 MatchService — Полная структура файлов

```
MatchService/
│
├─ 📄 Eventide.MatchService.sln                     (Visual Studio решение)
├─ 📄 README.md                                     (Overview)
├─ 📄 LICENSE
├─ 📄 coursework.md                                 (Курсовая работа 50+ стр)
├─ 📄 coursework.docx                               (Word версия)
├─ 📄 PROJECT_STRUCTURE.md                          (Детальная структура)
├─ 📄 QUICK_REFERENCE.md                            (Быстрая справка)
├─ 📄 FILE_TREE.md                                  (Этот файл)
│
├─ 📁 .git/                                         (Git история)
├─ 📁 .github/                                      (GitHub Actions)
│  └─ workflows/
│     ├─ build.yml                                  (CI: build & test)
│     └─ deploy.yml                                 (CD: deploy)
├─ 📁 .vs/                                          (VS Code конфиги)
│
├─ 📁 Domain/                                       ✅ DOMAIN LAYER
│  │
│  ├─ 📁 MatchService.Domain/
│  │  ├─ MatchService.Domain.csproj
│  │  │
│  │  ├─ 📁 Base/
│  │  │  ├─ Entity.cs                               (Entity<TId> базовый класс)
│  │  │  └─ DomainException.cs                      (DomainException базовый класс)
│  │  │
│  │  ├─ 📁 Entities/
│  │  │  ├─ Match.cs                                ⭐ AGGREGATE ROOT
│  │  │  │  ├─ Свойства: Player1, Player2, Winner, Status, Scores, Dates
│  │  │  │  ├─ Методы: Start(), SubmitResult(), Dispute(), Cancel()
│  │  │  │  └─ Валидация: инварианты, переходы статусов
│  │  │  │
│  │  │  ├─ User.cs                                 (Игрок/Участник)
│  │  │  │  ├─ Свойства: Id, Username (VO), Matches (коллекция)
│  │  │  │  └─ Роль: ссылка на игрока в матче
│  │  │  │
│  │  │  ├─ Administrator.cs                        (Администратор)
│  │  │  │  ├─ Свойства: Id, Username (VO), Matches (коллекция)
│  │  │  │  └─ Роль: подтверждает диспуты, управляет доступом
│  │  │  │
│  │  │  ├─ Tournament.cs                           (Турнир)
│  │  │  │  ├─ Свойства: Id, Status, StartDate, EndDate
│  │  │  │  └─ Роль: контекст для матчей
│  │  │  │
│  │  │  └─ Bracket.cs                              (Турнирная сетка)
│  │  │     ├─ Свойства: Id, Format (enum)
│  │  │     └─ Роль: структура турнира
│  │  │
│  │  ├─ 📁 Enums/
│  │  │  ├─ MatchStatus.cs
│  │  │  │  ├─ Scheduled (0)
│  │  │  │  ├─ InProgress (1)
│  │  │  │  ├─ Completed (2)
│  │  │  │  ├─ Disputed (3)
│  │  │  │  └─ Cancelled (4)
│  │  │  │
│  │  │  └─ [другие enums]
│  │  │
│  │  ├─ 📁 Exceptions/
│  │  │  ├─ DomainException.cs                      (базовый класс)
│  │  │  ├─ InvalidMatchStatusException.cs          (недопустимый переход)
│  │  │  ├─ InvalidMatchWinnerException.cs          (победитель не участник)
│  │  │  ├─ MatchNotFoundException.cs               (матч не найден)
│  │  │  ├─ MatchNotInProgressException.cs          (матч не запущен)
│  │  │  ├─ MatchNotScheduledException.cs           (матч уже запущен)
│  │  │  ├─ MatchPlayerSameException.cs             (один игрок дважды)
│  │  │  ├─ MatchWinnerNotParticipantException.cs   (победитель посторонний)
│  │  │  ├─ ArgumentNullValueException.cs           (null значение)
│  │  │  └─ TournamentDateRangeInvalidException.cs  (некорректные даты)
│  │  │
│  │  ├─ 📁 obj/                                    (Build artifacts)
│  │  └─ 📁 bin/Debug/net8.0/                       (Compiled DLL)
│  │     └─ MatchService.Domain.dll
│  │
│  ├─ 📁 MatchService.ValueObjects/
│  │  ├─ MatchService.ValueObjects.csproj
│  │  │
│  │  ├─ 📁 Base/
│  │  │  ├─ ValueObject.cs                          (ValueObject<T> базовый класс)
│  │  │  │  ├─ Abstract метод Validate()
│  │  │  │  ├─ Equals() на основе Value
│  │  │  │  └─ GetHashCode() на основе Value
│  │  │  │
│  │  │  └─ IValidator.cs                           (интерфейс для валидаторов)
│  │  │     └─ void Validate(T value);
│  │  │
│  │  ├─ Username.cs                                (VO: 3-30 символов)
│  │  │  ├─ Валидация: не пусто, буквы/цифры/подчеркивание
│  │  │  ├─ MaxLength = 30
│  │  │  └─ Применение: User.Username, Administrator.Username
│  │  │
│  │  ├─ MapName.cs                                 (VO: название карты)
│  │  │  ├─ Валидация: не пусто, ≤ 50 символов
│  │  │  ├─ MaxLength = 50
│  │  │  └─ Применение: Match.MapName
│  │  │
│  │  ├─ ServerInfo.cs                              (VO: информация о сервере)
│  │  │  ├─ Host: string (IP или домен)
│  │  │  ├─ Port: int (1-65535)
│  │  │  ├─ Валидация портов
│  │  │  └─ Применение: Match.ServerInfo
│  │  │
│  │  ├─ PlayerScore.cs                             (VO: очки игрока)
│  │  │  ├─ Value: int (0-100)
│  │  │  ├─ MaxScore = 100
│  │  │  ├─ Валидация: 0 ≤ score ≤ 100
│  │  │  └─ Применение: Match.Player1Score, Match.Player2Score
│  │  │
│  │  ├─ 📁 Validators/
│  │  │  ├─ UsernameValidator.cs
│  │  │  ├─ MapNameValidator.cs
│  │  │  ├─ ServerInfoValidator.cs
│  │  │  └─ PlayerScoreValidator.cs
│  │  │
│  │  ├─ 📁 obj/
│  │  └─ 📁 bin/Debug/net8.0/
│  │     └─ MatchService.ValueObjects.dll
│  │
│  ├─ 📁 MatchService.Domain.Repositories.Abstractions/
│  │  ├─ MatchService.Domain.Repositories.Abstractions.csproj
│  │  │
│  │  ├─ 📁 Base/
│  │  │  └─ IRepository<TEntity, TId>.cs             (Generic Repository интерфейс)
│  │  │     ├─ GetByIdAsync(TId id) → TEntity?
│  │  │     ├─ GetAllAsync() → IReadOnlyCollection<TEntity>
│  │  │     ├─ AddAsync(TEntity entity) → void
│  │  │     ├─ UpdateAsync(TEntity entity) → void
│  │  │     ├─ DeleteAsync(TEntity entity) → void
│  │  │     ├─ DeleteAsync(TId id) → void
│  │  │     └─ ExistsAsync(TId id) → bool
│  │  │
│  │  ├─ 📁 Repositories/
│  │  │  ├─ IUserRepository.cs                       (специализированный для User)
│  │  │  │  ├─ Extends: IRepository<User, Guid>
│  │  │  │  └─ GetByUsernameAsync(string username)
│  │  │  │
│  │  │  └─ IAdministratorRepository.cs              (специализированный для Admin)
│  │  │     ├─ Extends: IRepository<Administrator, Guid>
│  │  │     └─ GetByUsernameAsync(string username)
│  │  │
│  │  ├─ 📁 obj/
│  │  └─ 📁 bin/Debug/net8.0/
│  │     └─ MatchService.Domain.Repositories.Abstractions.dll
│  │
│  └─ 📁 DomainApp/                                  (Console testing app)
│     ├─ DomainApp.csproj
│     ├─ Program.cs                                  (Demo: создание и управление Match)
│     │  ├─ Создание администратора
│     │  ├─ Создание пользователей
│     │  ├─ Создание матча
│     │  ├─ Запуск матча (Scheduled → InProgress)
│     │  ├─ Регистрация результата (InProgress → Completed)
│     │  ├─ Попытка запустить запущенный матч (ошибка)
│     │  └─ Вывод результатов
│     │
│     ├─ 📁 obj/
│     └─ 📁 bin/Debug/net8.0/
│
├─ 📁 Infrastructure/                                ✅ INFRASTRUCTURE LAYER
│  └─ 📁 MatchService.Infrastructure.EntityFramework/
│     ├─ MatchService.Infrastructure.EntityFramework.csproj
│     │
│     ├─ ApplicationDbContext.cs                     ⭐ ГЛАВНЫЙ DBCONTEXT
│     │  ├─ DbSet<User> Users
│     │  ├─ DbSet<Administrator> Administrators
│     │  ├─ DbSet<Match> Matches
│     │  ├─ DbSet<Tournament> Tournaments
│     │  ├─ DbSet<Bracket> Brackets
│     │  │
│     │  ├─ OnConfiguring()
│     │  │  └─ Sensitive data logging disabled (security)
│     │  │
│     │  └─ OnModelCreating()
│     │     └─ modelBuilder.ApplyConfigurationsFromAssembly()
│     │
│     ├─ 📁 Configurations/                          (Fluent API конфигурации)
│     │  ├─ MatchConfiguration.cs                    ⭐ ГЛАВНАЯ
│     │  │  ├─ Table: "matches"
│     │  │  ├─ Value Objects: MapName, ServerInfo, PlayerScore (with conversion)
│     │  │  ├─ Enum: Status (as int in DB)
│     │  │  ├─ DateTime: UTC handling
│     │  │  ├─ Foreign Keys:
│     │  │  │  ├─ Player1 (User) — OnDelete: Restrict
│     │  │  │  ├─ Player2 (User) — OnDelete: Restrict
│     │  │  │  ├─ Winner (User) — OnDelete: SetNull
│     │  │  │  ├─ Administrator — OnDelete: Restrict
│     │  │  │  ├─ Tournament — OnDelete: Restrict
│     │  │  │  └─ Bracket — OnDelete: Restrict
│     │  │  │
│     │  │  └─ Indices: status, player1_id, player2_id, tournament_id
│     │  │
│     │  ├─ UserConfiguration.cs
│     │  │  ├─ Table: "users"
│     │  │  ├─ Value Object: Username (with conversion)
│     │  │  ├─ Unique constraint: username
│     │  │  └─ Navigation: Matches (collection)
│     │  │
│     │  ├─ AdministratorConfiguration.cs
│     │  │  ├─ Table: "administrators"
│     │  │  ├─ Value Object: Username (with conversion)
│     │  │  ├─ Unique constraint: username
│     │  │  ├─ Navigation: Matches (field-based)
│     │  │  └─ UsePropertyAccessMode(PropertyAccessMode.Field)
│     │  │
│     │  ├─ TournamentConfiguration.cs
│     │  │  ├─ Table: "tournaments"
│     │  │  ├─ Key: Id
│     │  │  └─ Basic properties
│     │  │
│     │  └─ BracketConfiguration.cs
│     │     ├─ Table: "brackets"
│     │     ├─ Key: Id
│     │     └─ Basic properties
│     │
│     ├─ 📁 RepositoryEF/
│     │  └─ EFRepository<TEntity, TId>.cs             (Generic Repository реализация)
│     │     ├─ Implements: IRepository<TEntity, TId>
│     │     │
│     │     ├─ GetByIdAsync(TId id)
│     │     │  └─ First or default с CancellationToken
│     │     │
│     │     ├─ GetAllAsync()
│     │     │  └─ ToListAsync() как IReadOnlyCollection
│     │     │
│     │     ├─ AddAsync(TEntity entity)
│     │     │  └─ Add + SaveChangesAsync
│     │     │
│     │     ├─ UpdateAsync(TEntity entity)
│     │     │  └─ Update + SaveChangesAsync
│     │     │
│     │     ├─ DeleteAsync(TEntity entity / TId id)
│     │     │  └─ Remove + SaveChangesAsync
│     │     │
│     │     └─ ExistsAsync(TId id)
│     │        └─ AnyAsync based on id
│     │
│     ├─ 📁 Repositories/
│     │  ├─ UserRepository.cs                        (специализированная реализация)
│     │  │  ├─ Extends: EFRepository<User, Guid>
│     │  │  ├─ Implements: IUserRepository
│     │  │  └─ GetByUsernameAsync(username)
│     │  │
│     │  └─ AdministratorRepository.cs               (специализированная реализация)
│     │     ├─ Extends: EFRepository<Administrator, Guid>
│     │     ├─ Implements: IAdministratorRepository
│     │     └─ GetByUsernameAsync(username)
│     │
│     ├─ 📁 Migrations/                              (EF Core Migrations)
│     │  ├─ {Timestamp}_CreateInitialSchema.cs       (baseline)
│     │  ├─ {Timestamp}_AddConstraints.cs
│     │  ├─ {Timestamp}_InitialInfrastructureFix.cs  (latest)
│     │  │  ├─ CreateTable: users
│     │  │  ├─ CreateTable: administrators
│     │  │  ├─ CreateTable: tournaments
│     │  │  ├─ CreateTable: brackets
│     │  │  ├─ CreateTable: matches (с FK)
│     │  │  └─ CreateIndex: на всех FK и важных колонках
│     │  │
│     │  └─ ApplicationDbContextModelSnapshot.cs     (снимок текущей модели)
│     │
│     ├─ 📁 obj/
│     └─ 📁 bin/Debug/net8.0/
│        └─ MatchService.Infrastructure.EntityFramework.dll
│
├─ 📁 Presentation/                                  ✅ PRESENTATION LAYER
│  └─ 📁 MatchService.WebHost/
│     ├─ MatchService.WebHost.csproj
│     │
│     ├─ Program.cs                                  ⭐ STARTUP КОНФИГУРАЦИЯ
│     │  ├─ builder = WebApplication.CreateBuilder(args)
│     │  ├─ Добавление DbContext с Npgsql
│     │  ├─ Регистрация Repository (generic + специализированные)
│     │  ├─ Регистрация Controllers
│     │  ├─ Using scope: Database.MigrateAsync() — автоприменение миграций
│     │  ├─ app.MapControllers()
│     │  └─ app.Run()
│     │
│     ├─ 📁 Controllers/
│     │  ├─ MatchController.cs                       ⭐ REST ENDPOINTS
│     │  │  ├─ [HttpGet("{id}")] GetMatch(Guid id)
│     │  │  │  └─ GET /api/v1/matches/{id} → 200 MatchDto или 404
│     │  │  │
│     │  │  ├─ [HttpGet] GetAllMatches()
│     │  │  │  └─ GET /api/v1/matches → 200 List<MatchDto>
│     │  │  │
│     │  │  ├─ [HttpPost("{id}/start")] StartMatch(Guid id)
│     │  │  │  ├─ POST /api/v1/matches/{id}/start
│     │  │  │  ├─ Запуск: match.Start()
│     │  │  │  └─ Response: 200 Ok или 400 BadRequest (DomainException)
│     │  │  │
│     │  │  ├─ [HttpPost("{id}/submit-result")] SubmitResult(Guid id, SubmitResultRequest)
│     │  │  │  ├─ POST /api/v1/matches/{id}/submit-result
│     │  │  │  ├─ Body: { winnerId, player1Score, player2Score }
│     │  │  │  ├─ Регистрация: match.SubmitResult(...)
│     │  │  │  └─ Response: 200 Ok или 400 BadRequest
│     │  │  │
│     │  │  ├─ [HttpPost("{id}/dispute")] DisputeMatch(Guid id)
│     │  │  │  ├─ POST /api/v1/matches/{id}/dispute
│     │  │  │  ├─ Оспаривание: match.Dispute()
│     │  │  │  └─ Response: 200 Ok
│     │  │  │
│     │  │  └─ [HttpPost("{id}/cancel")] CancelMatch(Guid id)
│     │  │     ├─ POST /api/v1/matches/{id}/cancel
│     │  │     ├─ Отмена: match.Cancel()
│     │  │     └─ Response: 200 Ok
│     │  │
│     │  └─ [HealthCheckController.cs]
│     │     └─ GET /health → 200 Ok (для мониторинга)
│     │
│     ├─ 📁 Models/
│     │  ├─ 📁 Requests/
│     │  │  ├─ StartMatchRequest.cs
│     │  │  │  └─ (может быть пусто)
│     │  │  │
│     │  │  ├─ SubmitResultRequest.cs
│     │  │  │  ├─ WinnerId: Guid
│     │  │  │  ├─ Player1Score: int
│     │  │  │  └─ Player2Score: int
│     │  │  │
│     │  │  ├─ DisputeMatchRequest.cs
│     │  │  │  └─ Reason: string (опционально)
│     │  │  │
│     │  │  └─ CancelMatchRequest.cs
│     │  │     └─ Reason: string (опционально)
│     │  │
│     │  └─ 📁 Responses/
│     │     ├─ MatchDto.cs                           (DTO для ответа)
│     │     │  ├─ Id: Guid
│     │     │  ├─ Status: string
│     │     │  ├─ Player1Id, Player2Id: Guid
│     │     │  ├─ WinnerId: Guid?
│     │     │  ├─ Scores
│     │     │  └─ Timestamps
│     │     │
│     │     ├─ ErrorResponse.cs
│     │     │  ├─ Message: string
│     │     │  ├─ Code: int
│     │     │  └─ Timestamp: DateTime
│     │     │
│     │     └─ PagedResponse<T>.cs (для пагинации)
│     │        ├─ Items: List<T>
│     │        ├─ Page: int
│     │        └─ PageSize: int
│     │
│     ├─ 📁 Properties/
│     │  └─ launchSettings.json                      (VS launch configuration)
│     │     ├─ "http": localhost:5000
│     │     └─ "https": localhost:5001
│     │
│     ├─ appsettings.json                            (Development конфиг)
│     │  ├─ ConnectionStrings
│     │  │  └─ DefaultConnection: "Host=localhost;Port=5432;Database=matchservice;..."
│     │  ├─ Logging
│     │  │  └─ LogLevel: Debug
│     │  └─ AllowedHosts: "*"
│     │
│     ├─ appsettings.Production.json                 (Production конфиг)
│     │  ├─ ConnectionStrings
│     │  │  └─ DefaultConnection: используется env variable
│     │  ├─ Logging
│     │  │  └─ LogLevel: Error
│     │  └─ AllowedHosts: *.example.com
│     │
│     ├─ 📁 obj/
│     └─ 📁 bin/Debug/net8.0/
│        └─ MatchService.WebHost.dll
│
├─ 📁 Contracts/                                     (Contracts для интеграции)
│  ├─ Eventide.MatchService.Contracts.csproj
│  │
│  ├─ 📁 Events/
│  │  ├─ MatchCompletedEvent.cs                      (Published Domain Event)
│  │  │  ├─ MatchId: Guid
│  │  │  ├─ WinnerId: Guid
│  │  │  ├─ Player1Score: int
│  │  │  ├─ Player2Score: int
│  │  │  ├─ CompletedAt: DateTime
│  │  │  └─ Подписывается: NotificationService, TournamentService
│  │  │
│  │  ├─ MatchStartedEvent.cs                        (Published Domain Event)
│  │  │  ├─ MatchId: Guid
│  │  │  ├─ StartedAt: DateTime
│  │  │  └─ Подписывается: NotificationService
│  │  │
│  │  └─ MatchCancelledEvent.cs                      (Published Domain Event)
│  │     ├─ MatchId: Guid
│  │     ├─ Reason: string
│  │     └─ Подписывается: NotificationService
│  │
│  ├─ 📁 obj/
│  └─ 📁 bin/Debug/net8.0/
│     └─ Eventide.MatchService.Contracts.dll
│
└─ 📄 .gitignore
   ├─ bin/, obj/ (build outputs)
   ├─ *.dll, *.exe (compiled)
   └─ .vs/, .vscode/ (IDE configs)

```

---

## 📊 Статистика

| Метрика | Значение |
|---------|----------|
| **Проектов в решении** | 7 |
| **Классов (Entities)** | 5 (Match, User, Admin, Tournament, Bracket) |
| **Value Objects** | 4 (Username, MapName, ServerInfo, PlayerScore) |
| **Доменных исключений** | 8+ |
| **Repository интерфейсов** | 3 |
| **REST endpoints** | 6+ |
| **Таблиц в БД** | 5 |
| **Миграций** | 3+ |
| **Строк кода (Domain)** | ~2000 |
| **Строк кода (Infrastructure)** | ~1500 |
| **Строк кода (Presentation)** | ~800 |
| **Строк документации** | ~5000+ |

---

## 🎯 Key Files

```
🔴 CRITICAL — Must not break
├─ Domain/MatchService.Domain/Entities/Match.cs          (бизнес-логика)
├─ Infrastructure/.../ApplicationDbContext.cs            (БД контекст)
├─ Infrastructure/.../Configurations/MatchConfiguration.cs (БД схема)
└─ Presentation/MatchService.WebHost/Program.cs          (startup)

🟡 IMPORTANT — Be careful
├─ Infrastructure/.../Migrations/*.cs                    (БД версионирование)
├─ Presentation/.../Controllers/MatchController.cs       (API)
└─ Domain/MatchService.Domain/Exceptions/*.cs            (обработка ошибок)

🟢 FLEXIBLE — Can refactor
├─ MatchService.ValueObjects/*.cs                        (VO, easy to extend)
├─ Domain.Repositories.Abstractions/*.cs                 (интерфейсы)
└─ Presentation/Models/Requests, Responses/*.cs          (DTOs)
```

---

**Версия:** 1.0  
**Дата:** 14.05.2026  
**Статус:** ✅ Полная структура документирована
