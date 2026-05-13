namespace MatchService.ValueObjects.Exceptions;

public class ServerInfoNullOrWhiteSpaceException(string paramName)
    : ArgumentNullException(paramName, $"The \"{paramName}\" value must not be null, empty, or whitespace.");
