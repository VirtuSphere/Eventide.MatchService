using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.User;

public sealed record CreateUserModel(string Username) : ICreateModel;