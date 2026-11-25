using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.Match;

/// <summary>
/// Payload for creating a match.
/// </summary>
public sealed record CreateMatchModel(
    Guid TournamentId,
    Guid BracketId,
    Guid AdministratorId,
    Guid Player1Id,
    Guid Player2Id) : ICreateModel;
