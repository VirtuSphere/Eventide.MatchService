namespace MatchService.ValueObjects.Exceptions;

public class PlayerScoreOutOfRangeException(string paramName, int value, int minValue, int maxValue)
    : ArgumentOutOfRangeException(paramName, value, $"The \"{paramName}\" value must be in range [{minValue}, {maxValue}].");
