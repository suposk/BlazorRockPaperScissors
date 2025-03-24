namespace MudBlazorRokPaperScissors.Domain;

/// <summary>
/// Type of draw, either Rock, Paper or Scissors
/// </summary>
public enum DrawType
{
    None,
    Rock,
    Paper,
    Scissors
}

/// <summary>
/// Type of player, either Human or Computer
/// </summary>
public enum PlayerType
{
    Human,
    Computer
}


public class DrawSelectedEventArgs(DrawType draw , string playerId, string playerName) : EventArgs
{
    public DrawType Draw { get; } = draw;
    public DateTime TimeUtc { get; } = DateTime.UtcNow;
    public string PlayerId { get; } = playerId;
    public string PlayerName { get; } = playerName;
    public override string ToString() => $"{PlayerName} selected {Draw}";
}

/// <summary>
/// Delegate for DrawSelected event
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
public delegate void DrawSelectedEventHandler(object sender, DrawSelectedEventArgs e);

/// <summary>
/// The Player class represents a player in a Rock-Paper-Scissors game. 
/// It includes properties and methods to manage the player's state and actions.
/// </summary>
public class Player
{
    public string Id { get; }
    public string Name { get; }
    public PlayerType PlayerType { get; }
    public DrawType Draw { get; private set; }    

    public Player(PlayerType playerType, string name, string? id)
    {
        if (string.IsNullOrWhiteSpace(name))        
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));        

        Name = name;
        PlayerType = playerType;
        Id = id ?? Guid.NewGuid().ToString();
    }

    public Result SelectDraw(DrawType draw)
    {
        if (draw == DrawType.None)
            return Result.Failure($"Draw {Draw} is not valid option");

        Draw = draw;
        OnDrawSelected();        
        return Result.Success();
    }

    public void ResetDraw() => Draw = DrawType.None;

    #region Events

    public event DrawSelectedEventHandler? DrawSelected;
    public void OnDrawSelected() => DrawSelected?.Invoke(this, new DrawSelectedEventArgs(Draw, Id, Name));
    
    #endregion

    #region GetHashCode and Equals

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (ReferenceEquals(obj, this))
            return true;
        if (obj.GetType() != GetType())
            return false;

        Player? rhs = obj as Player;
        //return Id == rhs.Id;
        return Id == rhs?.Id && Name == rhs?.Name;
    }

    public static bool operator ==(Player lhs, Player rhs) { return Equals(lhs, rhs); }
    public static bool operator !=(Player lhs, Player rhs) { return !Equals(lhs, rhs); }

    #endregion
}
