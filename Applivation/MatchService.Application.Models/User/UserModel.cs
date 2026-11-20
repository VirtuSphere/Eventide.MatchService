using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.User;

public sealed record UserModel(Guid Id, string Username) : IModel<Guid>;