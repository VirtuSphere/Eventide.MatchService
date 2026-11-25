# Eventide.MatchService

Микросервис управления матчами платформы VirtuSphere.

## Назначение

MatchService хранит пользователей, администраторов, матчи, турниры и турнирные сетки и предоставляет REST API для CRUD-сценариев и создания матча. Доменная логика построена на сущностях и объектах-значениях; данные сохраняются через Entity Framework Core в PostgreSQL.

Подробная техническая документация: [coursework.md](coursework.md).

## Быстрый старт

Требуется .NET 8 SDK и PostgreSQL.

```powershell
dotnet restore Eventide.MatchService.sln
dotnet build Eventide.MatchService.sln --configuration Release
dotnet run --project Presentation\MatchService.WebHost
```

В среде Development Swagger доступен по адресу `http://localhost:5117/swagger`.

Строка подключения задаётся в `Presentation/MatchService.WebHost/appsettings.json`. Для секретов и production-среды используйте переменные среды или Secret Manager.

## Основные API

| Маршрут | Назначение |
|---|---|
| `/api/users` | Пользователи |
| `/api/administrators` | Администраторы |
| `/api/matches` | Матчи |

## Тестирование

```powershell
dotnet test Tests\MatchService.Tests\MatchService.Tests.csproj --configuration Release
```

GitHub Actions собирает решение и проверяет порог line coverage не ниже 95%.