using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.User;

/// <summary>
/// Payload for creating a user.
/// </summary>
public sealed record CreateUserModel(string Username) : ICreateModel;