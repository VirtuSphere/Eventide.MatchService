namespace MatchService.ValueObjects.Exceptions;

public class ServerInfoTooLongException(string paramName, string value, int maxLength)
    : ArgumentException($"The \"{paramName}\" value must not exceed {maxLength} characters. Actual length: {value.Length}.", paramName);
