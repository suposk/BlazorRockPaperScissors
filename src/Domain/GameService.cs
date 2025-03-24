namespace MudBlazorRokPaperScissors.Domain;

public class GameService
{
    public GameDrawResult GetGameResult(Game game)
    {
        if (game.Players.Count != Game.Constants.MaxPlayers)
            Result.Failure("Game must have 2 players");

        Player player1 = game.Player1;
        Player player2 = game.Player2;

        if (player1.Draw == DrawType.None || player2.Draw == DrawType.None)
            Result.Failure("Players must select draw");

        if (player1.Draw == player2.Draw)
        {            
            var res = new GameDrawResult(game.Draws.Values.ToList(), game.DateTimeService.UtcNow);
            res.Result = ResultType.Draw;
            return res;
        }

        else if (player1.Draw == DrawType.Rock && player2.Draw == DrawType.Scissors ||
                 player1.Draw == DrawType.Paper && player2.Draw == DrawType.Rock ||
                 player1.Draw == DrawType.Scissors && player2.Draw == DrawType.Paper)
        {
            //player1 wins
            var res = new GameDrawResult(game.Draws.Values.ToList(), game.DateTimeService.UtcNow);
            res.Result = ResultType.Win;
            res.WinnerId = player1.Id;
            res.WinnerName = player1.Name;
            res.LosserId = player2.Id;
            res.LosserName = player2.Name;
            return res;
        }
        else
        {
            //player2 wins
            var res = new GameDrawResult(game.Draws.Values.ToList(), game.DateTimeService.UtcNow);
            res.Result = ResultType.Lose;
            res.WinnerId = player2.Id;
            res.WinnerName = player2.Name;
            res.LosserId = player1.Id;
            res.LosserName = player1.Name;
            return res;
        }
    }
}
