using MatchService.Domain.Exceptions;
using MatchService.ValueObjects;
using Xunit;

namespace MatchService.Tests;

public sealed class AdministratorTests
{
    [Fact]
    public void Administrator_ManagesOwnedMatches()
    {
        var administrator = TestData.Administrator();
        var match = administrator.CreateMatch(
            new MatchService.Domain.Tournament(Guid.NewGuid()),
            new MatchService.Domain.Bracket(Guid.NewGuid()),
            TestData.User(),
            TestData.User("PlayerTwo"));

        Assert.True(administrator.RescheduleMatch(match, DateTime.UtcNow.AddHours(1)));
        Assert.True(administrator.StartMatch(match, new ServerInfo("server"), new MapName("Ancient")));
        Assert.True(administrator.SubmitMatchResult(match, match.Player1, new PlayerScore(1), new PlayerScore(0)));
        Assert.True(administrator.DisputeMatch(match));
        Assert.True(administrator.DeleteMatch(match));
        Assert.Empty(administrator.Matches);
    }

    [Fact]
    public void Administrator_RejectsUnknownMatch()
    {
        var administrator = TestData.Administrator();

        Assert.Throws<MatchNotFoundException>(() => administrator.DeleteMatch(TestData.Match()));
    }

    [Fact]
    public void ChangeUsername_ReturnsWhetherValueChanged()
    {
        var administrator = TestData.Administrator();

        Assert.False(administrator.ChangeUsername(new Username("AdminUser")));
        Assert.True(administrator.ChangeUsername(new Username("NewAdmin")));
        Assert.Equal("NewAdmin", administrator.Username.Value);
    }
}
