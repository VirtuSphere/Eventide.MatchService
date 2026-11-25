using AutoMapper;
using MatchService.Application.Models.Match;
using MatchService.Application.Services;
using MatchService.Application.Services.Mapping;
using MatchService.Domain;
using MatchService.Domain.Repositories.Abstractions.Repositories;
using Moq;
using Xunit;
using MatchEntity = MatchService.Domain.Match;

namespace MatchService.Tests;

public sealed class MatchApplicationServiceTests
{
    private readonly Mock<IMatchRepository> matches = new();
    private readonly Mock<ITournamentRepository> tournaments = new();
    private readonly Mock<IBracketRepository> brackets = new();
    private readonly Mock<IAdministratorRepository> administrators = new();
    private readonly Mock<IUserRepository> users = new();
    private readonly IMapper mapper = new MapperConfiguration(
        configuration => configuration.AddProfile<ApplicationProfile>()).CreateMapper();

    [Fact]
    public async Task GetModelsAndGetByIdMapMatchesAndHandleMissing()
    {
        var match = TestData.Match();
        matches.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new[] { match });
        matches.Setup(x => x.GetByIdAsync(match.Id, It.IsAny<CancellationToken>())).ReturnsAsync(match);
        matches.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != match.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchEntity?)null);
        var service = CreateService();

        Assert.Equal(match.Id, Assert.Single(await service.GetModelsAsync()).Id);
        Assert.Equal(match.Id, (await service.GetModelByIdAsync(match.Id))!.Id);
        Assert.Null(await service.GetModelByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateReturnsNullWhenAnyRequiredEntityIsMissing()
    {
        tournaments.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tournament?)null);
        brackets.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Bracket?)null);
        administrators.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Administrator?)null);
        users.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await CreateService().CreateModelAsync(CreateRequest());

        Assert.Null(result);
        matches.Verify(x => x.AddAsync(It.IsAny<MatchEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateResolvesDependenciesAndPersistsMatch()
    {
        var request = CreateRequest();
        var tournament = new Tournament(request.TournamentId);
        var bracket = new Bracket(request.BracketId);
        var administrator = TestData.Administrator();
        var player1 = TestData.User();
        var player2 = TestData.User("PlayerTwo");
        tournaments.Setup(x => x.GetByIdAsync(request.TournamentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        brackets.Setup(x => x.GetByIdAsync(request.BracketId, It.IsAny<CancellationToken>())).ReturnsAsync(bracket);
        administrators.Setup(x => x.GetByIdAsync(request.AdministratorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(administrator);
        users.Setup(x => x.GetByIdAsync(request.Player1Id, It.IsAny<CancellationToken>())).ReturnsAsync(player1);
        users.Setup(x => x.GetByIdAsync(request.Player2Id, It.IsAny<CancellationToken>())).ReturnsAsync(player2);
        var service = CreateService();

        var result = await service.CreateModelAsync(request);

        Assert.NotNull(result);
        Assert.Equal(tournament.Id, result.TournamentId);
        Assert.Equal(player1.Id, result.Player1Id);
        matches.Verify(x => x.AddAsync(It.IsAny<MatchEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAndDeleteReportWhetherMatchExists()
    {
        var match = TestData.Match();
        matches.Setup(x => x.GetByIdAsync(match.Id, It.IsAny<CancellationToken>())).ReturnsAsync(match);
        matches.Setup(x => x.GetByIdAsync(It.Is<Guid>(id => id != match.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MatchEntity?)null);
        var service = CreateService();

        Assert.False(await service.UpdateModelAsync(CreateModel(Guid.NewGuid())));
        Assert.True(await service.UpdateModelAsync(CreateModel(match.Id)));
        Assert.False(await service.DeleteModelAsync(Guid.NewGuid()));
        Assert.True(await service.DeleteModelAsync(match.Id));
        matches.Verify(x => x.DeleteAsync(match, It.IsAny<CancellationToken>()), Times.Once);
    }

    private MatchApplicationService CreateService()
        => new(matches.Object, tournaments.Object, brackets.Object, administrators.Object, users.Object, mapper);

    private static CreateMatchModel CreateRequest()
        => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private static MatchModel CreateModel(Guid id)
        => new(id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, MatchService.Domain.Enums.MatchStatus.Scheduled, null, null, null, null, null, null, null,
            DateTime.UtcNow);
}
