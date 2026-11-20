using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.Bracket;

public sealed record CreateBracketModel(Guid Id) : ICreateModel;