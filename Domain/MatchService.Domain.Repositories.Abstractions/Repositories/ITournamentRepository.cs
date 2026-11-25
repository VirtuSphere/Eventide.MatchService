using MatchService.Domain.Repositories.Abstractions.Base;

namespace MatchService.Domain.Repositories.Abstractions.Repositories;

public interface ITournamentRepository : IRepository<Tournament, Guid>
{
}
