using Eventide.MatchService.Application.Common;
using Eventide.MatchService.Application.DTOs;
using MediatR;

namespace Eventide.MatchService.Application.Queries.GetMatch;

public class GetMatchQuery : IRequest<Result<MatchDto>>
{
    public Guid MatchId { get; init; }
}