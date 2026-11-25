using MatchService.Application.Models.Administrator;
using MatchService.Application.Models.Bracket;
using MatchService.Application.Models.Match;
using MatchService.Application.Models.User;
using MatchService.Domain.Enums;
using Xunit;

namespace MatchService.Tests;

public sealed class ApiModelCoverageTests
{
    [Fact]
    public void ModelRecordsExposeTheirValues()
    {
        var id = Guid.NewGuid();
        var user = new UserModel(id, "Player");
        var createUser = new CreateUserModel("NewPlayer");
        var administrator = new AdministratorModel(id, "Admin");
        var createAdministrator = new CreateAdministratorModel("NewAdmin");
        var bracket = new BracketModel(id);
        var createBracket = new CreateBracketModel(id);
        var createMatch = new CreateMatchModel(id, id, id, id, id);
        var match = new MatchModel(id, id, id, id, id, id, id, MatchStatus.Completed,
            DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, "Ancient", "server", 1, 0, DateTime.UtcNow);

        Assert.Equal("Player", user.Username);
        Assert.Equal("NewPlayer", createUser.Username);
        Assert.Equal("Admin", administrator.Username);
        Assert.Equal("NewAdmin", createAdministrator.Username);
        Assert.Equal(id, bracket.Id);
        Assert.Equal(id, createBracket.Id);
        Assert.Equal(id, createMatch.TournamentId);
        Assert.Equal(id, match.WinnerId);
        Assert.Equal("Ancient", match.MapName);
        Assert.Equal("server", match.ServerInfo);
        Assert.Equal(1, match.Player1Score);
        Assert.Equal(0, match.Player2Score);
        Assert.Equal(MatchStatus.Completed, match.Status);
        Assert.Equal(id, match.Id);
        Assert.Equal(id, match.BracketId);
        Assert.Equal(id, match.AdministratorId);
        Assert.Equal(id, match.Player1Id);
        Assert.Equal(id, match.Player2Id);
        Assert.NotEqual(default, match.CreatedAt);
    }
}
