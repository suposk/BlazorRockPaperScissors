namespace MudBlazorRokPaperScissors.UnitTest.Domain;

public class GameTest
{
    [Fact]
    public void Create_Game_Ok()
    {
        var game = Game.CreateGame(BaseTest.PlayerHuman, BaseTest.PlayerComputer, BaseTest.dateTimeService, setUpGame: true).Value!;
        //act
        //request.Create(UtcNow);
        //assert
        //Assert.NotNull(game);
        game.Should().NotBeNull();
        game.Player1.Should().NotBeNull();
        game.Player2.Should().NotBeNull();
        game.GameStatus.Should().Be(GameStatusType.DrawInProgress);
    }

    [Fact]
    public void Start_Game_Ok()
    {
        var game = Game.CreateGame(BaseTest.PlayerHuman, BaseTest.PlayerComputer, BaseTest.dateTimeService, setUpGame: false).Value!;
        game.SetUpGame();
        game.GameStatus.Should().Be(GameStatusType.DrawInProgress);
    }

    [Fact]
    public void Start_Game_Fail()
    {
        var game = Game.CreateGame(BaseTest.PlayerHuman, BaseTest.PlayerComputer, BaseTest.dateTimeService, setUpGame: false).Value!;
        //game.SetUpGame();
        game.GameStatus.Should().Be(GameStatusType.NotStarted);
    }

    [Fact]
    public void End_Game_Ok()
    {
        var game = BaseTest.GameHumanComputer;
        game.Player1.SelectDraw(DrawType.Rock);
        game.Player2.SelectDraw(DrawType.Scissors);
        game.GameStatus.Should().Be(GameStatusType.DrawFinished);
    }

    [Fact]
    public void Game_Should_Be_Draw()
    {
        var game = BaseTest.GameHumanComputer;
        game.Player1.SelectDraw(DrawType.Rock);
        game.Player2.SelectDraw(DrawType.Rock);
        game.LastGameDrawResult?.Result.Should().Be(ResultType.Draw);
        game.GameStatus.Should().Be(GameStatusType.DrawFinished);
    }

    [Fact]
    public void Game_Player1_Should_Win_1()
    {
        var game = BaseTest.GameHumanComputer;
        game.Player1.SelectDraw(DrawType.Rock);
        game.Player2.SelectDraw(DrawType.Scissors);
        game.LastGameDrawResult?.Result.Should().Be(ResultType.Win);
        game.GameStatus.Should().Be(GameStatusType.DrawFinished);
    }

    [Fact]
    public void Game_Player1_Should_Win_2()
    {
        var game = BaseTest.GameHumanComputer;
        game.Player1.SelectDraw(DrawType.Paper);
        game.Player2.SelectDraw(DrawType.Rock);
        game.LastGameDrawResult?.Result.Should().Be(ResultType.Win);
        game.GameStatus.Should().Be(GameStatusType.DrawFinished);
    }

    public void Game_Player1_Should_Win_3()
    {
        var game = BaseTest.GameHumanComputer;
        game.Player1.SelectDraw(DrawType.Scissors);
        game.Player2.SelectDraw(DrawType.Paper);
        game.LastGameDrawResult?.Result.Should().Be(ResultType.Win);
        game.GameStatus.Should().Be(GameStatusType.DrawFinished);
    }

    public void Game_Player1_Should_Lose()
    {
        var game = BaseTest.GameHumanComputer;
        game.Player1.SelectDraw(DrawType.Rock);
        game.Player2.SelectDraw(DrawType.Paper);
        game.LastGameDrawResult?.Result.Should().Be(ResultType.Lose);
        game.GameStatus.Should().Be(GameStatusType.DrawFinished);
    }
}
