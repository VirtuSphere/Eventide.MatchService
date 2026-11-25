using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.User;

/// <summary>
/// Represents a user returned by the API.
/// </summary>
public sealed record UserModel(Guid Id, string Username) : IModel<Guid>;