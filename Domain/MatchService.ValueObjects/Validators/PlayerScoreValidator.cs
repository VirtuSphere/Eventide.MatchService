using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Exceptions;

namespace MatchService.ValueObjects.Validators;

public class PlayerScoreValidator : IValidator<int>
{
    public static int MIN_VALUE => 0;
    public static int MAX_VALUE => 999;

    public void Validate(int value)
    {
        if (value < MIN_VALUE || value > MAX_VALUE)
        {
            throw new PlayerScoreOutOfRangeException(nameof(value), value, MIN_VALUE, MAX_VALUE);
        }
    }
}
