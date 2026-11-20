using MatchService.Application.Models.Base;

namespace MatchService.Application.Models.Administrator;

/// <summary>
/// Administrator returned by the MatchService API.
/// </summary>
public sealed record AdministratorModel(Guid Id, string Username) : IModel<Guid>;
