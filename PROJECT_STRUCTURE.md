# MatchService — Полная Структура Проекта

**Дата:** 14 мая 2026  
**Платформа:** .NET 8 (C# 12)  
**ORM:** Entity Framework Core 8.0.8  
**БД:** PostgreSQL 14  
**Архитектура:** Domain-Driven Design + Clean Architecture

---

## 📁 Структура репозитория

```
VirtuSphere/MatchService/
├── .git/                                    (Git история)
├── .github/                                 (GitHub Actions конфиги)
├── .gitignore
├── .vs/                                     (VS Code/Visual Studio конфиги)
│
├── 📄 Eventide.MatchService.sln             (Visual Studio решение)
├── 📄 README.md
├── 📄 LICENSE
│
├── 📄 coursework.md                         (Полная курсовая работа)
├── 📄 coursework.docx                       (Word версия курсовой)
├── 📄 PROJECT_STRUCTURE.md                  (Этот файл)
│
├── 📁 Domain/                               (Domain слой)
│   ├── MatchService.Domain/
│   │   ├── MatchService.Domain.csproj
│   │   │
│   │   ├── 📁 Base/
│   │   │   ├── Entity.cs                    (базовый класс для всех Entity)
│   │   │   └── DomainException.cs           (базовый класс для доменных исключений)
│   │   │
│   │   ├── 📁 Entities/
│   │   │   ├── Match.cs                     (Aggregate Root)
│   │   │   │   ├── Методы: Start(), SubmitResult(), Dispute(), Cancel()
│   │   │   │   ├── Свойства: Status, Player1, Player2, Winner, Scores
│   │   │   │   └── Валидация инвариантов
│   │   │   │
│   │   │   ├── User.cs                      (Игрок)
│   │   │   │   ├── Username: Username (VO)
│   │   │   │   └── Коллекция матчей
│   │   │   │
│   │   │   ├── Administrator.cs             (Администратор)
│   │   │   │   ├── Username: Username (VO)
│   │   │   │   └── Коллекция матчей
│   │   │   │
│   │   │   ├── Tournament.cs                (Турнир)
│   │   │   │   ├── Status: enum
│   │   │   │   ├── Dates (StartDate, EndDate)
│   │   │   │   └── Связь к Bracket
│   │   │   │
│   │   │   └── Bracket.cs                   (Турнирная сетка)
│   │   │       └── Format: enum
│   │   │
│   │   ├── 📁 Enums/
│   │   │   ├── MatchStatus.cs               (Scheduled, InProgress, Completed, Disputed, Cancelled)
│   │   │   └── [другие enums]
│   │   │
│   │   ├── 📁 Exceptions/
│   │   │   ├── DomainException.cs           (базовый класс)
│   │   │   ├── InvalidMatchStatusException.cs
│   │   │   ├── InvalidMatchWinnerException.cs
│   │   │   ├── MatchNotFoundException.cs
│   │   │   ├── MatchNotInProgressException.cs
│   │   │   ├── MatchNotScheduledException.cs
│   │   │   ├── MatchPlayerSameException.cs
│   │   │   ├── MatchWinnerNotParticipantException.cs
│   │   │   ├── ArgumentNullValueException.cs
│   │   │   └── TournamentDateRangeInvalidException.cs
│   │   │
│   │   └── 📁 bin/Debug/net8.0/             (откомпилированные DLL)
│   │
│   ├── MatchService.ValueObjects/           (Value Objects + Validators)
│   │   ├── MatchService.ValueObjects.csproj
│   │   │
│   │   ├── 📁 Base/
│   │   │   ├── ValueObject.cs               (базовый класс для VOs)
│   │   │   └── IValidator<T>.cs             (интерфейс для валидаторов)
│   │   │
│   │   ├── Username.cs                      (VO: имя пользователя)
│   │   │   ├── MaxLength: 30
│   │   │   └── Валидация: не пусто, только буквы/цифры/_
│   │   │
│   │   ├── MapName.cs                       (VO: название карты)
│   │   │   ├── MaxLength: 50
│   │   │   └── Валидация: не пусто
│   │   │
│   │   ├── ServerInfo.cs                    (VO: информация о сервере)
│   │   │   ├── Host: string (IP или домен)
│   │   │   ├── Port: int (1-65535)
│   │   │   └── Валидация портов
│   │   │
│   │   ├── PlayerScore.cs                   (VO: очки игрока)
│   │   │   ├── Value: int (0-100)
│   │   │   └── Валидация: 0 <= score <= 100
│   │   │
│   │   ├── 📁 Validators/
│   │   │   ├── UsernameValidator.cs
│   │   │   ├── MapNameValidator.cs
│   │   │   ├── ServerInfoValidator.cs
│   │   │   ├── PlayerScoreValidator.cs
│   │   │   └── [другие валидаторы]
│   │   │
│   │   └── 📁 bin/Debug/net8.0/
│   │
│   ├── MatchService.Domain.Repositories.Abstractions/
│   │   ├── MatchService.Domain.Repositories.Abstractions.csproj
│   │   │
│   │   ├── 📁 Base/
│   │   │   └── IRepository<TEntity, TId>.cs  (Generic Repository интерфейс)
│   │   │       ├── GetByIdAsync(id)
│   │   │       ├── GetAllAsync()
│   │   │       ├── AddAsync(entity)
│   │   │       ├── UpdateAsync(entity)
│   │   │       ├── DeleteAsync(entity / id)
│   │   │       └── ExistsAsync(id)
│   │   │
│   │   ├── 📁 Repositories/
│   │   │   ├── IUserRepository.cs            (расширение IRepository<User>)
│   │   │   │   └── GetByUsernameAsync(username)
│   │   │   │
│   │   │   └── IAdministratorRepository.cs   (расширение IRepository<Administrator>)
│   │   │       └── GetByUsernameAsync(username)
│   │   │
│   │   └── 📁 bin/Debug/net8.0/
│   │
│   └── DomainApp/                           (Console Application для тестирования)
│       ├── DomainApp.csproj
│       ├── Program.cs                       (демонстрация жизненного цикла Match)
│       └── 📁 bin/Debug/net8.0/
│
├── 📁 Infrastructure/                       (Infrastructure слой)
│   └── MatchService.Infrastructure.EntityFramework/
│       ├── MatchService.Infrastructure.EntityFramework.csproj
│       │
│       ├── ApplicationDbContext.cs           (EF Core DbContext)
│       │   ├── DbSet<User> Users
│       │   ├── DbSet<Administrator> Administrators
│       │   ├── DbSet<Match> Matches
│       │   ├── DbSet<Tournament> Tournaments
│       │   ├── DbSet<Bracket> Brackets
│       │   └── OnModelCreating() — применение конфигураций
│       │
│       ├── 📁 Configurations/               (Fluent API конфигурации)
│       │   ├── MatchConfiguration.cs        (IEntityTypeConfiguration<Match>)
│       │   │   ├── Ключ: Id
│       │   │   ├── Value Objects: MapName, ServerInfo, PlayerScore
│       │   │   ├── Enum: Status (as int)
│       │   │   ├── Отношения: Player1, Player2, Winner, Administrator, Tournament, Bracket
│       │   │   └── Foreign Keys с OnDelete
│       │   │
│       │   ├── UserConfiguration.cs         (IEntityTypeConfiguration<User>)
│       │   │   ├── Ключ: Id
│       │   │   ├── VO: Username
│       │   │   └── Коллекция: Matches
│       │   │
│       │   ├── AdministratorConfiguration.cs
│       │   │   ├── Ключ: Id
│       │   │   ├── VO: Username
│       │   │   ├── Field-based navigation для Matches
│       │   │   └── UsePropertyAccessMode(PropertyAccessMode.Field)
│       │   │
│       │   ├── TournamentConfiguration.cs
│       │   │   ├── Таблица: tournaments
│       │   │   ├── Ключ: Id
│       │   │   └── Базовые свойства
│       │   │
│       │   └── BracketConfiguration.cs
│       │       ├── Таблица: brackets
│       │       ├── Ключ: Id
│       │       └── Базовые свойства
│       │
│       ├── 📁 RepositoryEF/
│       │   └── EFRepository<TEntity, TId>.cs (Generic Repository реализация)
│       │       ├── Реализует: IRepository<TEntity, TId>
│       │       ├── GetByIdAsync()
│       │       ├── GetAllAsync()
│       │       ├── AddAsync() + SaveChanges
│       │       ├── UpdateAsync() + SaveChanges
│       │       ├── DeleteAsync()
│       │       └── ExistsAsync()
│       │
│       ├── 📁 Repositories/
│       │   ├── UserRepository.cs            (специализированная реализация)
│       │   │   └── GetByUsernameAsync()
│       │   │
│       │   └── AdministratorRepository.cs   (специализированная реализация)
│       │       └── GetByUsernameAsync()
│       │
│       ├── 📁 Migrations/                   (EF Core автогенерированные миграции)
│       │   ├── {Timestamp}_InitialCreate.cs (создание таблиц)
│       │   ├── {Timestamp}_AddConstraints.cs
│       │   ├── {Timestamp}_InitialInfrastructureFix.cs
│       │   └── ApplicationDbContextModelSnapshot.cs (снимок текущей модели)
│       │
│       └── 📁 bin/Debug/net8.0/
│
├── 📁 Presentation/                         (Presentation слой)
│   └── MatchService.WebHost/                (ASP.NET Core Web API)
│       ├── MatchService.WebHost.csproj
│       │
│       ├── Program.cs                       (Startup конфигурация)
│       │   ├── builder.Services.AddDbContext()
│       │   ├── builder.Services.AddScoped<IRepository<>>()
│       │   ├── builder.Services.AddControllers()
│       │   ├── app.MapControllers()
│       │   ├── Автоматическое применение миграций
│       │   └── app.Run()
│       │
│       ├── 📁 Controllers/
│       │   ├── MatchController.cs           (REST endpoints для Match)
│       │   │   ├── [HttpGet("{id}")] GetMatch()
│       │   │   ├── [HttpGet] GetAllMatches()
│       │   │   ├── [HttpPost("{id}/start")] StartMatch()
│       │   │   ├── [HttpPost("{id}/submit-result")] SubmitResult()
│       │   │   ├── [HttpPost("{id}/dispute")] DisputeMatch()
│       │   │   ├── [HttpPost("{id}/cancel")] CancelMatch()
│       │   │   └── Обработка DomainException
│       │   │
│       │   └── [другие контроллеры]
│       │
│       ├── 📁 Models/
│       │   ├── 📁 Requests/
│       │   │   ├── StartMatchRequest.cs
│       │   │   ├── SubmitResultRequest.cs
│       │   │   ├── DisputeMatchRequest.cs
│       │   │   └── CancelMatchRequest.cs
│       │   │
│       │   └── 📁 Responses/
│       │       ├── MatchDto.cs              (DTO для ответов)
│       │       ├── MatchDetailedDto.cs
│       │       └── ErrorResponse.cs
│       │
│       ├── appsettings.json                 (конфигурация при разработке)
│       │   ├── ConnectionStrings
│       │   │   └── DefaultConnection: "Host=localhost;Port=5432;Database=matchservice;..."
│       │   ├── Logging
│       │   └── AllowedHosts
│       │
│       ├── appsettings.Production.json      (конфигурация для production)
│       │   ├── ConnectionStrings (переменные окружения)
│       │   └── Logging (только errors)
│       │
│       └── 📁 bin/Debug/net8.0/
│
├── 📁 Contracts/                            (Контракты между микросервисами)
│   ├── Eventide.MatchService.Contracts.csproj
│   │
│   ├── 📁 Events/
│   │   ├── MatchCompletedEvent.cs           (Published Event для Message Queue)
│   │   │   ├── MatchId: Guid
│   │   │   ├── WinnerId: Guid
│   │   │   ├── Player1Score: int
│   │   │   ├── Player2Score: int
│   │   │   └── CompletedAt: DateTime
│   │   │
│   │   ├── MatchStartedEvent.cs
│   │   └── MatchCancelledEvent.cs
│   │
│   └── 📁 bin/Debug/net8.0/
│
└── 📁 .github/                              (GitHub Actions)
    └── workflows/
        ├── build.yml                        (CI: dotnet build & test)
        └── deploy.yml                       (CD: deploy to Azure)
```

---

## 📊 Таблица проектов и их зависимостей

| Проект | Тип | Тип вывода | Зависимости | Назначение |
|--------|-----|-----------|-------------|-----------|
| **MatchService.Domain** | Library | DLL | none | Доменные сущности, Value Objects, исключения |
| **MatchService.ValueObjects** | Library | DLL | Domain | Value Objects и Validators |
| **MatchService.Domain.Repositories.Abstractions** | Library | DLL | Domain, VO | Интерфейсы репозиториев |
| **MatchService.Infrastructure.EntityFramework** | Library | DLL | Domain, VO, Abstractions | EF конфигурации, Repository реализация |
| **MatchService.WebHost** | Web API | DLL | Domain, VO, Abstractions, Infrastructure | REST endpoints, Program.cs |
| **Eventide.MatchService.Contracts** | Library | DLL | none | Контракты для интеграции |
| **DomainApp** | Console | EXE | Domain, VO, Abstractions | Демонстрация работы доменной модели |

---

## 📝 Основные файлы и их назначение

### Domain Layer

#### MatchService.Domain/Base/Entity.cs
```
Базовый класс для всех Entity
- Generic: Entity<TId> где TId: struct
- Свойства: Id, CreatedAt, UpdatedAt
- Методы: Equals(), GetHashCode() на основе Id
- Инкапсулирует паттерн для каждой entity
```

#### MatchService.Domain/Entities/Match.cs
```
ГЛАВНАЯ СУЩНОСТЬ - Aggregate Root
- Методы: Start(), SubmitResult(), Dispute(), Cancel()
- Валидация инвариантов: победитель ∈ {Player1, Player2}
- State Machine: переходы Scheduled → InProgress → Completed
- Выбрасывает DomainExceptions при нарушениях
```

---

### Infrastructure Layer

#### MatchService.Infrastructure.EntityFramework/ApplicationDbContext.cs
```
DbContext для PostgreSQL
- DbSet<T> для User, Administrator, Match, Tournament, Bracket
- OnModelCreating() применяет конфигурации из сборки
- Supports: Async operations, Migrations, Relationships
```

#### MatchService.Infrastructure.EntityFramework/Configurations/MatchConfiguration.cs
```
Fluent API для Match
- Table name: "matches"
- Value Object conversions (MapName → string, PlayerScore → int)
- Enum conversion (MatchStatus → int)
- Foreign keys с OnDelete behavior
- UTC DateTime handling
```

#### MatchService.Infrastructure.EntityFramework/RepositoryEF/EFRepository.cs
```
Generic Repository implementation
- GetByIdAsync(), GetAllAsync()
- AddAsync() с SaveChanges()
- UpdateAsync(), DeleteAsync()
- ExistsAsync()
- Работает с любым Entity<TId>
```

---

### Presentation Layer

#### MatchService.WebHost/Program.cs
```
Startup конфигурация
1. builder.Services.AddDbContext<ApplicationDbContext>()
   - Npgsql provider
   - PostgreSQL connection string
2. builder.Services.AddScoped<IRepository<T, Guid>, EFRepository<T, Guid>>()
3. builder.Services.AddControllers()
4. Using scope: db.Database.MigrateAsync() — автоприменение миграций
5. app.MapControllers() + app.Run()
```

#### MatchService.WebHost/Controllers/MatchController.cs
```
REST API endpoints
- GET /api/v1/matches/{id} — получить матч
- GET /api/v1/matches — список матчей
- POST /api/v1/matches/{id}/start — запуск матча
- POST /api/v1/matches/{id}/submit-result — регистрация результата
- POST /api/v1/matches/{id}/dispute — оспаривание
- POST /api/v1/matches/{id}/cancel — отмена

Обработка:
- DomainException → 400 Bad Request
- NotFoundException → 404 Not Found
- Success → 200 Ok с DTO
```

---

## 📦 Database Schema (PostgreSQL)

```sql
-- Users (Игроки)
CREATE TABLE users (
    id UUID PRIMARY KEY,
    username VARCHAR(30) NOT NULL UNIQUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL
);
CREATE INDEX ix_users_username ON users(username);

-- Administrators
CREATE TABLE administrators (
    id UUID PRIMARY KEY,
    username VARCHAR(30) NOT NULL UNIQUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Tournaments
CREATE TABLE tournaments (
    id UUID PRIMARY KEY,
    tournament_name VARCHAR(100) NOT NULL,
    status INTEGER NOT NULL,
    start_date TIMESTAMP WITH TIME ZONE NOT NULL,
    end_date TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Brackets
CREATE TABLE brackets (
    id UUID PRIMARY KEY,
    tournament_id UUID REFERENCES tournaments(id) ON DELETE CASCADE,
    format INTEGER NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Matches (главная таблица)
CREATE TABLE matches (
    id UUID PRIMARY KEY,
    player1_id UUID NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    player2_id UUID NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    winner_id UUID REFERENCES users(id) ON DELETE SET NULL,
    administrator_id UUID NOT NULL REFERENCES administrators(id) ON DELETE RESTRICT,
    tournament_id UUID NOT NULL REFERENCES tournaments(id) ON DELETE RESTRICT,
    bracket_id UUID NOT NULL REFERENCES brackets(id) ON DELETE RESTRICT,
    
    status INTEGER NOT NULL,  -- 0: Scheduled, 1: InProgress, 2: Completed, 3: Disputed, 4: Cancelled
    
    player1_score INTEGER NOT NULL,
    player2_score INTEGER NOT NULL,
    map_name VARCHAR(50) NOT NULL,
    server_host VARCHAR(100) NOT NULL,
    server_port INTEGER NOT NULL,
    
    scheduled_time TIMESTAMP WITH TIME ZONE NOT NULL,
    started_at TIMESTAMP WITH TIME ZONE,
    completed_at TIMESTAMP WITH TIME ZONE,
    
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE INDEX ix_matches_status ON matches(status);
CREATE INDEX ix_matches_player1_id ON matches(player1_id);
CREATE INDEX ix_matches_player2_id ON matches(player2_id);
CREATE INDEX ix_matches_winner_id ON matches(winner_id);
CREATE INDEX ix_matches_tournament_id ON matches(tournament_id);
CREATE INDEX ix_matches_bracket_id ON matches(bracket_id);
CREATE INDEX ix_matches_scheduled_time ON matches(scheduled_time);
```

---

## 🔄 Data Flow Диаграмма

```
HTTP Request
    ↓
MatchController (Presentation)
    ↓
IRepository<Match> (Domain.Repositories.Abstractions)
    ↓
EFRepository<Match> (Infrastructure)
    ↓
ApplicationDbContext (Infrastructure)
    ↓
PostgreSQL Database
    ↓
Entity (Domain Model)
    ↓
Business Logic (Match.Start(), Match.SubmitResult(), etc.)
    ↓
Domain Exceptions (если нарушение инварианта)
    ↓
DTO (Response)
    ↓
HTTP Response (JSON)
```

---

## 🚀 Build & Run

### Восстановление зависимостей
```bash
dotnet restore
```

### Компиляция решения
```bash
dotnet build
```

### Миграция БД
```bash
dotnet ef database update \
  --project Infrastructure/MatchService.Infrastructure.EntityFramework/MatchService.Infrastructure.EntityFramework.csproj \
  --startup-project Presentation/MatchService.WebHost/MatchService.WebHost.csproj
```

### Запуск Web API
```bash
dotnet run --project Presentation/MatchService.WebHost/MatchService.WebHost.csproj
```
API: `http://localhost:5000/api/v1/matches`

### Запуск консольного приложения (демонстрация)
```bash
dotnet run --project Domain/DomainApp/DomainApp.csproj
```

---

## 📋 Файлы конфигурации

### appsettings.json (Development)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=matchservice;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
}
```

### appsettings.Production.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DB_CONNECTION_STRING}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Error"
    }
  }
}
```

---

## 📚 Организация Code по принципам

### Domain-Driven Design
- ✅ Bounded Context: MatchService
- ✅ Aggregate Root: Match
- ✅ Entities: User, Administrator, Tournament, Bracket
- ✅ Value Objects: Username, MapName, ServerInfo, PlayerScore
- ✅ Domain Exceptions: InvalidMatchStatusException, InvalidMatchWinnerException, etc.
- ✅ Ubiquitous Language: методы отражают бизнес-процессы (Start, SubmitResult, Dispute)

### Clean Architecture
- ✅ Domain Layer: чистая бизнес-логика, нет зависимостей от EF/ASP.NET
- ✅ Application Layer: использование Repository интерфейсов (не реализаций)
- ✅ Infrastructure Layer: EF конфигурации, Repository реализация
- ✅ Presentation Layer: Controllers, HTTP endpoints

### Design Patterns
- ✅ Repository Pattern: IRepository<TEntity, TId>
- ✅ State Machine: Match.TransitionStatus()
- ✅ Factory Pattern: Organizer.CreateTournament()
- ✅ Value Object Pattern: MapName, PlayerScore, etc.
- ✅ Aggregate Pattern: Match как корень агрегата

---

**Версия документа:** 1.0  
**Последнее обновление:** 14.05.2026  
**Статус:** ✅ Полная структура документирована
