using System.Threading;

namespace MudBlazorRokPaperScissors.Domain;

/// <summary>
/// Result of the game between 2 players, either Win, Lose or Draw
/// </summary>
public enum ResultType
{
    None,
    Win,
    Lose,
    Draw
}

/// <summary>
/// Type of game status, either NotStarted, DrawInProgress or DrawFinished
/// </summary>
public enum GameStatusType
{
    NotStarted,
    DrawInProgress,
    DrawFinished,
    //GameFinished
}

public class GameDrawResultEventArgs(GameDrawResult? gameDrawResult) : EventArgs
{
    public GameDrawResult? GameDrawResult { get; } = gameDrawResult;
    public DateTime TimeUtc { get; } = DateTime.UtcNow;
}

/// <summary>
/// Delegate for GameDrawResult event
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void GameDrawResultEventHandler(object sender, GameDrawResultEventArgs e);

public class GameDrawResult
{
    public GameDrawResult(List<DrawSelectedEventArgs> draws, DateTime timeUtc)
    {
        Draws = draws ?? throw new ArgumentNullException(nameof(draws));
        TimeUtc = timeUtc;
    }

    public string? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public string? LosserId { get; set; }
    public string? LosserName { get; set; }
    public ResultType Result { get; set; }
    public DateTime TimeUtc { get; }
    public List<DrawSelectedEventArgs> Draws { get; } = [];
}

/// <summary>
/// The Game class represents a game of Rock-Paper-Scissors between two players. 
/// Use the CreateGame method to create a new game with two players.
/// It includes properties, methods, and events to manage the game's state, players, and results.
/// </summary>
public class Game : IDisposable
{
    private Game() { }

    public static class Constants
    {
        /// <summary>
        /// Maximum number of players in a game. 2 players are required to start a game.
        /// </summary>
        public const int MaxPlayers = 2;        
    }

    #region Events and Handlers

    public GameDrawResultEventHandler? GameDrawResult;
    public void OnGameDrawResult() => GameDrawResult?.Invoke(this, new GameDrawResultEventArgs(LastGameDrawResult));

    #endregion

    #region Properties

    /// <summary>
    /// Dictionary of player Id and player object
    /// </summary>
    public Dictionary<string, Player> Players { get; private set; } = [];

    /// <summary>
    /// Dictionary of player Id and selected draw
    /// </summary>
    public Dictionary<string, DrawSelectedEventArgs> Draws { get; } = [];

    /// <summary>
    /// Dictionary of player Id and number of wins
    /// </summary>
    public Dictionary<string, int> Wins { get; } = [];

    /// <summary>
    /// Unique identifier for the game
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();
    public GameStatusType GameStatus { get; set; } = GameStatusType.NotStarted;
    public GameDrawResult? LastGameDrawResult { get; private set; }

    public IDateTimeService DateTimeService { get; init; }

    public Player Player1 { get; init; }
    public Player Player2 { get; init; }

    public string CurrentScore => $"{Player1.Name} ({Wins.GetValueOrDefault(Player1.Id, 0)}) : ({Wins.GetValueOrDefault(Player2.Id, 0)}) {Player2.Name}";

    /// <summary>
    /// List of game draw results. 
    /// History of all game results
    /// </summary>
    public List<GameDrawResult> DrawResults { get; } = [];

    #endregion

    #region Fields

    private GameService _gameService = new();

    #endregion

    #region public Methods
    public static ResultCom<Game> CreateGame(Player player1, Player player2, IDateTimeService dateTimeService, bool setUpGame = true)
    {
        if (player1 is null)
            return ResultCom<Game>.Failure("Players 1 must be provided");
        if (player2 is null)
            return ResultCom<Game>.Failure("Players 2 must be provided");
        if (player1 == player2)
            return ResultCom<Game>.Failure("Players must be different");
        if (dateTimeService is null)
            return ResultCom<Game>.Failure("DateTimeService must be provided");

        Game game = new() 
        {
            GameStatus = GameStatusType.NotStarted,
            Player1 = player1,
            Player2 = player2,
            DateTimeService = dateTimeService
        };
        game.Players.TryAdd(player1.Id, player1);
        game.Players.TryAdd(player2.Id, player2);
        if (setUpGame)
        {
            var st = game.SetUpGame();
            if (st.IsFailure)
                return ResultCom<Game>.Failure(st.ErrorMessage);
        }
        return ResultCom<Game>.Success(game);
    }

    public Result SetUpGame()
    {
        if (Players.Count != Constants.MaxPlayers)        
            return Result.Failure("Game must have 2 players");

        LastGameDrawResult = null;
        DrawResults.Clear();
        AddHandlers();
        GameStatus = GameStatusType.DrawInProgress;
        return Result.Success();
    }

    public Result EndDraw()
    {
        //if (GameStatus == GameStatusType.DrawFinished)
        //    return ResultCom.Failure("Game has already finished");

        //RemoveHandlers();        
        GameStatus = GameStatusType.DrawFinished;
        Draws.Clear();
        return Result.Success();
    }

    #endregion

    #region private Methods
    private void UpdateScore(string winnerId)
    {
        if (winnerId is null)
            throw new ArgumentNullException(nameof(winnerId));
        if (Wins.ContainsKey(winnerId))
            Wins[winnerId]++;
        else
            Wins.TryAdd(winnerId, 1);
    }

    private void Player_DrawSelected(object sender, DrawSelectedEventArgs e)
    {
        if (e.Draw == DrawType.None)
            throw new ApplicationException($"{nameof(e.Draw)} {e.Draw} is not valid type");

        //prevent player from changing draw
        Draws.TryAdd(e.PlayerId, e);
        if (Draws.Count == Constants.MaxPlayers)
        {
            LastGameDrawResult = _gameService.GetGameResult(this);
            if (LastGameDrawResult.Result != ResultType.Draw && LastGameDrawResult.WinnerId is not null)
                UpdateScore(LastGameDrawResult.WinnerId); //increment winner score

            DrawResults.Insert(0, LastGameDrawResult);
            EndDraw();
            OnGameDrawResult();
        }
    }

    private void RemoveHandlers()
    {
        foreach (var player in Players.Values)        
            player.DrawSelected -= Player_DrawSelected;
        Draws.Clear();
    }

    private void AddHandlers()
    {        
        foreach (var player in Players.Values)        
            player.DrawSelected += Player_DrawSelected;
        Draws.Clear();
    }


    /// <summary>
    /// remove all event handlers
    /// </summary>
    public void Dispose() => RemoveHandlers();

    #endregion
}
