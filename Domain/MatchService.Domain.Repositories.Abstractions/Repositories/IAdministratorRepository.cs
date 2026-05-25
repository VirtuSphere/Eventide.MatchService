using MatchService.Domain.Repositories.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.Domain.Repositories.Abstractions.Repositories;

public interface IAdministratorRepository : IRepository<Administrator, Guid>
{
    // Так как имя пользователя уникальное
    Task<Administrator?> GetAdministratorByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<Administrator?> GetAdministratorByMatchIdAsync(Guid matchId, CancellationToken cancellationToken);
}

