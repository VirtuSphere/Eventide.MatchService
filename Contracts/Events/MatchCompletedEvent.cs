namespace Eventide.MatchService.Contracts.Events;

public class MatchCompletedEvent
{
    public Guid MatchId { get; init; }
    public Guid WinnerId { get; init; }
    public Guid LoserId { get; init; }
    public int WinnerScore { get; init; }
    public int LoserScore { get; init; }
}