using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Validators;

namespace MatchService.ValueObjects;

/// <summary>
/// Represents type of the entity's MapName.
/// </summary>
/// <param name="name">The MapName of the entity.</param>
public class MapName(string name) : ValueObject<string>(new MapNameValidator(), name);