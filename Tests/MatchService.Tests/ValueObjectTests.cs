using MatchService.ValueObjects;
using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Exceptions;
using Xunit;

namespace MatchService.Tests;

public sealed class ValueObjectTests
{
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ab")]
    public void Username_RejectsInvalidValues(string value)
        => Assert.ThrowsAny<ArgumentException>(() => new Username(value));

    [Fact]
    public void Username_AcceptsBoundaryValues()
    {
        var username = new Username(new string('a', 3));
        var maximum = new Username(new string('a', 30));

        Assert.Equal("aaa", username.Value);
        Assert.Equal(30, maximum.Value.Length);
    }

    [Fact]
    public void Username_RejectsValuesLongerThanMaximum()
        => Assert.Throws<ArgumentLongValueException>(() => new Username(new string('a', 31)));

    [Fact]
    public void MapName_RejectsInvalidValues()
    {
        Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => new MapName(" "));
        Assert.Throws<ArgumentShortValueException>(() => new MapName("ab"));
        Assert.Throws<ArgumentLongValueException>(() => new MapName(new string('a', 31)));
    }

    [Fact]
    public void ServerInfo_RejectsBlankAndTooLongValues()
    {
        Assert.Throws<ServerInfoNullOrWhiteSpaceException>(() => new ServerInfo(" "));
        Assert.Throws<ServerInfoTooLongException>(() => new ServerInfo(new string('a', 121)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000)]
    public void PlayerScore_RejectsValuesOutsideRange(int value)
        => Assert.Throws<PlayerScoreOutOfRangeException>(() => new PlayerScore(value));

    [Fact]
    public void ValueObjects_WithSameValue_AreEqual()
    {
        Assert.Equal(new Username("Player"), new Username("Player"));
        Assert.NotEqual(new Username("Player"), new Username("Other"));
        Assert.Equal("Player", new Username("Player").ToString());
    }

    [Fact]
    public void ValueObject_RejectsNullValidator()
    {
        Assert.Throws<ValidatorNullException>(() =>
            new TestValueObject(null!, "value"));
    }

    [Fact]
    public void ValueObject_EqualityAndOperatorsCoverAllBranches()
    {
        var first = new TestValueObject(new TestValidator(), "value");
        var sameReference = first;
        var equal = new TestValueObject(new TestValidator(), "value");
        var different = new TestValueObject(new TestValidator(), "other");

        Assert.True(first.Equals((object?)equal));
        Assert.False(first.Equals((object?)null));
        Assert.False(first.Equals((object?)"value"));
        Assert.True(first.Equals(sameReference));
        Assert.False(first.Equals(different));
        Assert.Equal(first.GetHashCode(), equal.GetHashCode());
        Assert.Equal("value", first.ToString());
        Assert.True(first == equal);
        Assert.True(first != different);
        Assert.True(TestValueObject.Empty == null);
    }

    private sealed class TestValueObject(IValidator<string> validator, string value)
        : ValueObject<string>(validator, value)
    {
        public static TestValueObject? Empty => null;
    }

    private sealed class TestValidator : IValidator<string>
    {
        public void Validate(string value)
        {
        }
    }
}
