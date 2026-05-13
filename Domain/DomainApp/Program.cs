using MatchService.Domain;
using MatchService.ValueObjects;

namespace DomainApp;

public class Program
{
	public static void Main(string[] args)
	{
		var administrator = new Administrator(Guid.NewGuid(), new Username("AdminUser"));
		var playerOne = new User(Guid.NewGuid(), new Username("Player1"));
		var playerTwo = new User(Guid.NewGuid(), new Username("Player2"));
		var match = administrator.CreateMatch(Guid.NewGuid(), Guid.NewGuid(), playerOne, playerTwo);

		PrintSection("1. Матч создан", match);

		var scheduledTime = DateTime.UtcNow.AddMinutes(15);
		match.Schedule(scheduledTime);
		PrintSection($"2. Матч запланирован на {scheduledTime:u}", match);

		var serverInfo = new ServerInfo("EU-West-1 / Frankfurt / Match Server #12");
		var mapName = new MapName("Ancient Ruins");
		administrator.StartMatch(match, serverInfo, mapName);
		PrintSection("3. Матч запущен", match);

		administrator.SubmitMatchResult(match, playerOne, new PlayerScore(3), new PlayerScore(1));
		PrintSection("4. Результат матча зафиксирован", match);

		playerOne.DisputeMatch(match);
		PrintSection("5. Матч оспорен игроком", match);
	}

	private static void PrintSection(string title, Match match)
	{
		Console.WriteLine();
		Console.WriteLine(new string('=', 72));
		Console.WriteLine(title);
		Console.WriteLine(new string('-', 72));
		Console.WriteLine(match);
	}
}
