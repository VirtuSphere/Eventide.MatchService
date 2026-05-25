using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Validators;

namespace MatchService.ValueObjects;

public class ServerInfo(string value) : ValueObject<string>(new ServerInfoValidator(), value);
