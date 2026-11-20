using MatchService.Application.Models.Base;
using MatchService.Domain.Enums;

namespace MatchService.Application.Models.Match;

public sealed record MatchModel(
    Guid Id,
    Guid TournamentId,
    Guid BracketId,
    Guid AdministratorId,
    Guid Player1Id,
    Guid Player2Id,
    Guid? WinnerId,
    MatchStatus Status,
    DateTime? ScheduledTime,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string? MapName,
    string? ServerInfo,
    int? Player1Score,
    int? Player2Score,
    DateTime CreatedAt) : IModel<Guid>;
