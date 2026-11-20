using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.Bracket;

public sealed record BracketModel(Guid Id) : IModel<Guid>;