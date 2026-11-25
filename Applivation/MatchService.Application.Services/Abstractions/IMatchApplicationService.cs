using MatchService.Application.Models.Match;
using MatchService.Application.Services.Abstractions.Base;

namespace MatchService.Application.Services.Abstractions;

public interface IMatchApplicationService : IApplicationService<MatchModel, CreateMatchModel, Guid>
{
}
