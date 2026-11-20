using MatchService.Domain;
using MatchService.ValueObjects;

namespace MatchService.Tests;

internal static class TestData
{
    public static Administrator Administrator(string name = "AdminUser")
        => new(Guid.NewGuid(), new Username(name));

    public static User User(string name = "PlayerOne")
        => new(Guid.NewGuid(), new Username(name));

    public static Match Match(Administrator? administrator = null)
    {
        administrator ??= Administrator();
        return administrator.CreateMatch(
            new Tournament(Guid.NewGuid()),
            new Bracket(Guid.NewGuid()),
            User(),
            User("PlayerTwo"));
    }
}
