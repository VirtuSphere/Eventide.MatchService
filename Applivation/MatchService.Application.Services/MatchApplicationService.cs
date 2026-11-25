using AutoMapper;
using MatchService.Application.Models.Match;
using MatchService.Application.Services.Abstractions;
using MatchService.Domain.Repositories.Abstractions.Repositories;

namespace MatchService.Application.Services;

public sealed class MatchApplicationService(
    IMatchRepository matchRepository,
    ITournamentRepository tournamentRepository,
    IBracketRepository bracketRepository,
    IAdministratorRepository administratorRepository,
    IUserRepository userRepository,
    IMapper mapper) : IMatchApplicationService
{
    public async Task<IEnumerable<MatchModel>> GetModelsAsync(CancellationToken cancellationToken = default)
        => (await matchRepository.GetAllAsync(cancellationToken)).Select(mapper.Map<MatchModel>);

    public async Task<MatchModel?> GetModelByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetByIdAsync(id, cancellationToken);
        return match is null ? null : mapper.Map<MatchModel>(match);
    }

    public async Task<MatchModel?> CreateModelAsync(CreateMatchModel matchInformation, CancellationToken cancellationToken = default)
    {
        var tournament = await tournamentRepository.GetByIdAsync(matchInformation.TournamentId, cancellationToken);
        var bracket = await bracketRepository.GetByIdAsync(matchInformation.BracketId, cancellationToken);
        var administrator = await administratorRepository.GetByIdAsync(matchInformation.AdministratorId, cancellationToken);
        var player1 = await userRepository.GetByIdAsync(matchInformation.Player1Id, cancellationToken);
        var player2 = await userRepository.GetByIdAsync(matchInformation.Player2Id, cancellationToken);

        if (tournament is null || bracket is null || administrator is null || player1 is null || player2 is null)
            return null;

        var match = administrator.CreateMatch(tournament, bracket, player1, player2);
        await matchRepository.AddAsync(match, cancellationToken);
        return mapper.Map<MatchModel>(match);
    }

    public async Task<bool> UpdateModelAsync(MatchModel matchInformation, CancellationToken cancellationToken = default)
        => await matchRepository.GetByIdAsync(matchInformation.Id, cancellationToken) is not null;

    public async Task<bool> DeleteModelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var match = await matchRepository.GetByIdAsync(id, cancellationToken);
        if (match is null)
            return false;

        await matchRepository.DeleteAsync(match, cancellationToken);
        return true;
    }
}
