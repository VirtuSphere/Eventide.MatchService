using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Validators;

namespace MatchService.ValueObjects;

public class Username(string name) : ValueObject<string>(new UsernameValidator(), name) { }
