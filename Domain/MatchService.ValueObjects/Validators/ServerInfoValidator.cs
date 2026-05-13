using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Exceptions;

namespace MatchService.ValueObjects.Validators;

public class ServerInfoValidator : IValidator<string>
{
    public static int MAX_LENGTH => 120;

    public void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ServerInfoNullOrWhiteSpaceException(nameof(value));
        }

        if (value.Length > MAX_LENGTH)
        {
            throw new ServerInfoTooLongException(nameof(value), value, MAX_LENGTH);
        }
    }
}
