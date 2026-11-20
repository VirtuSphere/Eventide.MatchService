using MatchService.Application.Models.Administrator;
using MatchService.Application.Services.Abstractions.Base;

namespace MatchService.Application.Services.Abstractions;

public interface IAdministratorApplicationService : IApplicationService<AdministratorModel, CreateAdministratorModel, Guid>
{
    Task<AdministratorModel?> GetAdministratorByUsernameAsync(string userName, CancellationToken cancellationToken = default);
}