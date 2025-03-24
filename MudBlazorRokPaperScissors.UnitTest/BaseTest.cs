namespace MudBlazorRokPaperScissors.UnitTest;

public abstract class BaseTest
{
    public static readonly IDateTimeService dateTimeService = new DateTimeService();

    public static readonly Player PlayerHuman = new(PlayerType.Human, "Human", "Human-1");
    public static readonly Player PlayerComputer = new(PlayerType.Computer, "Computer", "Computer-2");

    public static readonly Game GameHumanComputer = Game.CreateGame(PlayerHuman, PlayerComputer, dateTimeService, true).Value!;

    //public static readonly DateTime UtcNow = new DateTime(2021, 1, 1, 1, 1, 1);
}
