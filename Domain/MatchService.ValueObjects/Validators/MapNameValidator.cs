using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Exceptions;

namespace MatchService.ValueObjects.Validators;

public class MapNameValidator : IValidator<string>
{
    /// <summary>
    /// The MapName's max length
    /// </summary>
    public static int MAX_LENGTH => 30;

    /// <summary>
    /// The MapName's min length
    /// </summary>
    public static int MIN_LENGTH => 3;

    /// <summary>
    /// Verifies the string to make sure it is not null, empty or doesn't consists only white-space characters.
    /// </summary>
    /// <param name="value">A string containing data.</param>
    /// <exception cref="ArgumentNullOrWhiteSpaceException"></exception>
    /// <exception cref="ArgumentLongValueException"></exception>
    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullOrWhiteSpaceException(nameof(value));

        if (value.Length > MAX_LENGTH)
            throw new ArgumentLongValueException(nameof(value), value, MAX_LENGTH);

        if (value.Length < MIN_LENGTH)
            throw new ArgumentShortValueException(nameof(value), value, MIN_LENGTH);
    }
}