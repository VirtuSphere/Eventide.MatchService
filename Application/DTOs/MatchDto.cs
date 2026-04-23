namespace Eventide.MatchService.Application.DTOs;

public class MatchDto
{
    public Guid Id { get; init; }
    public Guid TournamentId { get; init; }
    public Guid Player1Id { get; init; }
    public Guid Player2Id { get; init; }
    public Guid? WinnerId { get; init; }
    public string Status { get; init; } = string.Empty;
    public int Player1Score { get; init; }
    public int Player2Score { get; init; }
    public DateTime? ScheduledTime { get; init; }
    public string? MapName { get; init; }
}