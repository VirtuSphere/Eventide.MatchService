using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.Match;

public record class CreateMatchModel(
    Guid TournamentId,
    Guid BracketId,
    Guid AdministratorId,
    Guid Player1Id,
    Guid Player2Id) : ICreateModel;
