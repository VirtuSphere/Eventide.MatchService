# MatchService — Справочник по структуре проекта

**Дата создания:** 14 мая 2026  
**Версия:** .NET 8 / C# 12  
**Архитектура:** DDD + Clean Architecture

---

## 🎯 Быстрая навигация

### Где найти...

| Что нужно | Где искать | Файл |
|----------|-----------|------|
| **Бизнес-логика матча** | Domain | `Domain/MatchService.Domain/Entities/Match.cs` |
| **Value Objects** | Domain | `Domain/MatchService.ValueObjects/*.cs` |
| **Исключения** | Domain | `Domain/MatchService.Domain/Exceptions/*.cs` |
| **Repository интерфейсы** | Domain | `Domain/MatchService.Domain.Repositories.Abstractions/Base/IRepository<>.cs` |
| **EF конфигурации** | Infrastructure | `Infrastructure/.../Configurations/*.cs` |
| **DbContext** | Infrastructure | `Infrastructure/.../ApplicationDbContext.cs` |
| **REST endpoints** | Presentation | `Presentation/MatchService.WebHost/Controllers/MatchController.cs` |
| **Startup конфиг** | Presentation | `Presentation/MatchService.WebHost/Program.cs` |
| **Миграции** | Infrastructure | `Infrastructure/.../Migrations/*.cs` |
| **Демонстрация** | Testing | `Domain/DomainApp/Program.cs` |

---

## 📦 Проекты и их роли

### 1️⃣ Domain Layer

**MatchService.Domain**
- ✅ Entities: Match, User, Administrator, Tournament, Bracket
- ✅ Enums: MatchStatus
- ✅ Exceptions: все доменные исключения
- ✅ Base: Entity<TId>, DomainException
- ❌ НЕ содержит: EF Core, БД, Controllers

**MatchService.ValueObjects**
- ✅ Value Objects: Username, MapName, ServerInfo, PlayerScore
- ✅ Validators: для каждого VO
- ✅ Base: ValueObject<T>, IValidator<T>
- ❌ НЕ содержит: Entity логику, Repositories

**MatchService.Domain.Repositories.Abstractions**
- ✅ IRepository<TEntity, TId> — generic интерфейс
- ✅ IUserRepository, IAdministratorRepository — специализированные
- ✅ НЕ зависит от EF Core
- ❌ Реализация находится в Infrastructure

---

### 2️⃣ Infrastructure Layer

**MatchService.Infrastructure.EntityFramework**
- ✅ ApplicationDbContext — DbContext для PostgreSQL
- ✅ Configurations — Fluent API для каждой Entity
- ✅ EFRepository — generic Repository реализация
- ✅ Repositories — специализированные Repository классы
- ✅ Migrations — EF-генерированные миграции БД
- ✅ Зависит от Domain, ValueObjects, Abstractions

---

### 3️⃣ Presentation Layer

**MatchService.WebHost**
- ✅ Program.cs — Startup, DI регистрация, миграции
- ✅ Controllers — REST endpoints
- ✅ Models — DTOs для запросов и ответов
- ✅ appsettings.json — конфигурация БД
- ✅ Зависит от всех слоёв (как точка входа)

---

### 4️⃣ Testing & Contracts

**DomainApp**
- ✅ Console приложение
- ✅ Демонстрация жизненного цикла Match
- ✅ Примеры использования доменной модели

**Eventide.MatchService.Contracts**
- ✅ Events для интеграции с другими микросервисами
- ✅ MatchCompletedEvent, MatchStartedEvent, MatchCancelledEvent
- ✅ Используется NotificationService и другими сервисами

---

## 🔑 Ключевые концепции

### Match Lifecycle (State Machine)

```
Scheduled ──┐
            │
            ▼
      InProgress ──┐
            ▲      │
            │      │
         Dispute  │
            ▲      │
            │      ▼
            └─ Completed
            │
            └────→ Cancelled (в любой момент)
```

| Переход | Метод | Условие | Исключение |
|---------|-------|---------|-----------|
| Scheduled → InProgress | Start() | Оба игрока есть | MatchNotScheduledException |
| InProgress → Completed | SubmitResult() | Результат указан | MatchNotInProgressException |
| Completed → Disputed | Dispute() | - | - |
| Disputed → Completed | (админ) | - | - |
| * → Cancelled | Cancel() | - | - |

---

### Clean Architecture Layers

```
Layer 4: Presentation (Controllers, REST)
         ↑ зависит от
Layer 3: Infrastructure (EF, Repositories)
         ↑ зависит от
Layer 2: Application (Services, DTOs) — ПОКА НЕ РЕАЛИЗОВАНО
         ↑ зависит от
Layer 1: Domain (Entities, Value Objects, Exceptions)
```

**Правило зависимостей:** Слои зависят ТОЛЬКО от более внутренних слоёв.

---

### DDD Concepts

| Концепция | В MatchService | Пример |
|-----------|----------------|--------|
| **Bounded Context** | MatchService | Управляет ТОЛЬКО матчами, не турнирами |
| **Aggregate Root** | Match | Корень, через который проходят все операции |
| **Entity** | User, Administrator | Имеют идентичность (ID) |
| **Value Object** | MapName, PlayerScore | Определены значением, не ID |
| **Domain Event** | MatchCompletedEvent | События для асинхронной коммуникации |
| **Repository** | IRepository<> | Абстракция доступа к данным |
| **Ubiquitous Language** | Start(), SubmitResult() | Методы отражают бизнес-процессы |

---

## 📝 Типичные задачи

### Добавить новый REST endpoint

**Файл:** `Presentation/MatchService.WebHost/Controllers/MatchController.cs`

```csharp
[HttpPost("{id}/new-action")]
public async Task<IActionResult> NewAction(Guid id, CancellationToken ct)
{
    var match = await _matchRepository.GetByIdAsync(id, ct);
    if (match == null) return NotFound();
    
    try
    {
        match.NewMethod();  // бизнес-логика в домене
        await _matchRepository.UpdateAsync(match, ct);
        return Ok();
    }
    catch (DomainException ex)
    {
        return BadRequest(ex.Message);
    }
}
```

---

### Добавить новый State для Match

1. **Domain/MatchService.Domain/Enums/MatchStatus.cs**
   ```csharp
   public enum MatchStatus
   {
       Scheduled,
       InProgress,
       Completed,
       Disputed,
       Cancelled,
       NewStatus  // ← добавить
   }
   ```

2. **Domain/MatchService.Domain/Entities/Match.cs**
   ```csharp
   private void ValidateStatusTransition(MatchStatus next)
   {
       bool isValid = Status switch
       {
           MatchStatus.Scheduled => next is MatchStatus.InProgress or MatchStatus.Cancelled,
           MatchStatus.InProgress => next is MatchStatus.Completed or MatchStatus.Cancelled,
           MatchStatus.NewStatus => next is MatchStatus.Something,  // ← добавить
           // ... остальные
       };
   }
   ```

3. **Infrastructure/.../Configurations/MatchConfiguration.cs**
   - Не нужно менять (enum хранится как int)

---

### Добавить новое Value Object

1. **Domain/MatchService.ValueObjects/MyValueObject.cs**
   ```csharp
   public class MyValueObject : ValueObject<string>
   {
       public MyValueObject(string value) 
           : base(new MyValueObjectValidator(), value ?? ...)
       {
       }
   }
   ```

2. **Domain/MatchService.ValueObjects/Validators/MyValueObjectValidator.cs**
   ```csharp
   public class MyValueObjectValidator : IValidator<string>
   {
       public void Validate(string value)
       {
           if (string.IsNullOrEmpty(value))
               throw new ArgumentException("...");
       }
   }
   ```

3. **Infrastructure/.../Configurations/MatchConfiguration.cs**
   ```csharp
   builder.Property(m => m.MyVO)
       .HasConversion(
           vo => vo.Value,
           value => new MyValueObject(value))
       .HasColumnName("my_vo")
       .HasColumnType("character varying(50)")
       .IsRequired();
   ```

---

### Создать новую миграцию

```bash
dotnet ef migrations add {MigrationName} \
  --project Infrastructure/MatchService.Infrastructure.EntityFramework/MatchService.Infrastructure.EntityFramework.csproj \
  --startup-project Presentation/MatchService.WebHost/MatchService.WebHost.csproj
```

Миграция автоматически создастся в `Infrastructure/.../Migrations/`

---

## 📊 Database Tables

| Таблица | PK | FK | Индексы |
|--------|----|----|---------|
| **users** | id (UUID) | - | username |
| **administrators** | id (UUID) | - | username |
| **tournaments** | id (UUID) | - | - |
| **brackets** | id (UUID) | tournament_id | - |
| **matches** | id (UUID) | player1_id, player2_id, winner_id, admin_id, tournament_id, bracket_id | status, player1_id, player2_id, tournament_id, scheduled_time |

---

## 🔐 Security Checklist

- ✅ `EnableSensitiveDataLogging()` отключён в Production
- ✅ SQL Injection защита через parameterized queries (EF Core)
- ✅ Валидация всех Input через Value Objects
- ✅ Business rules enforced в Domain слое
- ✅ Foreign Keys установлены с OnDelete
- ❓ Нужна: JWT Authentication в Controllers
- ❓ Нужна: Authorization (role-based access)
- ❓ Нужна: Input sanitization в HTTP

---

## 📈 Масштабируемость

### Горизонтальное масштабирование
- ✅ Stateless Controllers
- ✅ Async/Await для non-blocking IO
- ✅ PostgreSQL поддерживает множество подключений
- ✅ Generic Repository для любых типов

### Вертикальное масштабирование
- ✅ Индексы на часто запрашиваемые колонки
- ✅ Connection pooling через Npgsql
- ✅ No N+1 queries (используется Include())

### Производительность
- 🔄 TODO: Добавить кэширование (Redis)
- 🔄 TODO: Добавить пагинацию в GetAllAsync()
- 🔄 TODO: Query optimization (ассиметричные индексы)

---

## 🧪 Тестирование

### Unit Tests (Domain слой)
```csharp
[Test]
public void Match_CannotStartIfNotScheduled()
{
    var match = new Match(..., status: InProgress, ...);
    
    Assert.Throws<InvalidMatchStatusException>(() => match.Start());
}
```

### Integration Tests (Infrastructure слой)
```csharp
[Test]
public async Task Repository_CanSaveAndLoad()
{
    var match = new Match(...);
    await repository.AddAsync(match);
    
    var loaded = await repository.GetByIdAsync(match.Id);
    Assert.AreEqual(match.Id, loaded.Id);
}
```

---

## 🚀 Deployment

### Local Development
```bash
dotnet run --project Presentation/MatchService.WebHost/
```
API: http://localhost:5000/api/v1/matches

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /app/bin/Release/net8.0/ .
ENTRYPOINT ["dotnet", "MatchService.WebHost.dll"]
```

### PostgreSQL
```bash
docker run -e POSTGRES_PASSWORD=postgres \
           -e POSTGRES_DB=matchservice \
           -p 5432:5432 \
           postgres:14
```

---

## 📚 Документация

| Документ | Расположение | Назначение |
|----------|-------------|-----------|
| **coursework.md** | `/` | Полная курсовая работа (50+ страниц) |
| **coursework.docx** | `/` | Word версия курсовой |
| **PROJECT_STRUCTURE.md** | `/` | Детальная структура проекта |
| **QUICK_REFERENCE.md** | `/` | Этот файл — быстрая справка |
| **README.md** | `/` | Overview проекта |

---

## 🔗 Интеграция с другими сервисами

### REST API Calls
```csharp
// MatchService → UserService
var user = await httpClient.GetAsync($"http://userservice/api/users/{userId}");

// MatchService → TournamentService
var tournament = await httpClient.GetAsync($"http://tournament/api/tournaments/{id}");
```

### Message Queue (RabbitMQ)
```csharp
// MatchService публикует событие
await eventBus.PublishAsync(new MatchCompletedEvent
{
    MatchId = match.Id,
    WinnerId = match.Winner.Id
});

// NotificationService подписан на событие
eventBus.Subscribe<MatchCompletedEvent>(async @event =>
{
    await notificationService.SendAsync($"Match {event.MatchId} completed!");
});
```

---

## 📞 Контакты и поддержка

**Репозиторий:** https://github.com/hyesos/VirtuSphere/MatchService  
**Автор:** [Your Name]  
**Дата последнего обновления:** 14.05.2026  
**Версия:** 1.0
