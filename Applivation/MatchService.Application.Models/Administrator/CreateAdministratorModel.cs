using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.Administrator;

/// <summary>
/// Payload for creating or updating an administrator.
/// </summary>
public sealed record CreateAdministratorModel(string Username) : ICreateModel;
