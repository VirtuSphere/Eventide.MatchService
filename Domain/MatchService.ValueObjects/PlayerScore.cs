using MatchService.ValueObjects.Base;
using MatchService.ValueObjects.Validators;

namespace MatchService.ValueObjects;

public class PlayerScore(int value) : ValueObject<int>(new PlayerScoreValidator(), value);
