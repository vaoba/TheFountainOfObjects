namespace TheFountainOfObjects;

class Program
{
    static void Main()
    {
        Game game = CreateGame4X4();
        game.Update();
    }

    static Game CreateGame4X4()
    {
        Level level = new(4);
        Position start = new Position(0, 0);
        Position fountain = new Position(2, 0);
        level.SetTileAtPosition(start, TileType.Entrance);
        level.SetTileAtPosition(fountain, TileType.Fountain);
        Player player = new(start);
        return new Game(level, player);
    }
}

public class Game(Level level, Player player)
{
    public bool Fountain { get; set; } = false;
    public Level Level { get; } = level;
    public Player Player { get; } = player;
    public TileType CurrentTile { get; set; } = TileType.Entrance;

    public void Update()
    {
        while (true)
        {
            CurrentTile = level.GetTileAt(Player.Position);
            
            ShowStatus();
            player.Sense(this);

            if (CheckWin() || CheckLose()) break;
            
            IAction action = GetAction();
            action.Execute(this);
        }
    }

    private void ShowStatus()
    {
        TextHelper.WriteLine("----------------------------------------------------------------------------------------------", ConsoleColor.White);
        TextHelper.WriteLine($"You are in the room [{Player.Position.X}, {Player.Position.Y}]", ConsoleColor.Magenta);
        // TextHelper.WriteLine(CurrentTile.ToString(), ConsoleColor.White);
    }
    
    private bool CheckWin()
    {
        if (Fountain && CurrentTile == TileType.Entrance)
        {
            TextHelper.WriteLine("You win!", ConsoleColor.Green);
            Console.ReadLine();
            return true;
        }
        return false;
    }

    private bool CheckLose()
    {
        if (!Player.IsAlive)
        {
            TextHelper.WriteLine("You lose!", ConsoleColor.Red);
            Console.ReadLine();
            return true;
        }
        return false;
    }

    private IAction GetAction()
    {
        TextHelper.WriteLine("Choose an action: ", ConsoleColor.Magenta);
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string? input = Console.ReadLine();

            IAction? action = input switch
            {
                "move north" => new MoveAction(new Position(0, 1)),
                "move east" => new MoveAction(new Position(1, 0)),
                "move south" => new MoveAction(new Position(0, -1)),
                "move west" => new MoveAction(new Position(-1, 0)),
                "enable fountain" => new FountainAction(),
                _ => null
            };

            if (action != null) return action;
        }
    }
}

public class Player(Position position)
{
    public Position Position { get; set; } = position;
    public bool IsAlive { get; set; } = true;

    public void Sense(Game game)
    {
        EntranceSense(game);
        FountainSense(game);
    }

    private void EntranceSense(Game game)
    {
        if (game.CurrentTile == TileType.Entrance)
        {
            if (game.Fountain) TextHelper.WriteLine("The Fountain of Objects has been reactivated, and you have escaped with your life!", ConsoleColor.Yellow);
            else TextHelper.WriteLine("You see light coming from the cavern entrance.", ConsoleColor.Yellow);
        }
    }

    private void FountainSense(Game game)
    {
        if (game.CurrentTile == TileType.Fountain)
        {
            if (game.Fountain) TextHelper.WriteLine("You hear the rushing waters from the Fountain of Objects. It has been reactivated!", ConsoleColor.Blue);
            else TextHelper.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!", ConsoleColor.Blue);
        }
    }
}

public class Level(int size)
{
    private TileType[,] _tiles = new TileType[size, size];

    public TileType GetTileAt(Position position) => _tiles[position.X, position.Y];

    public bool IsInBounds(Position position)
    {
        if (position.X < 0 || position.X >= _tiles.GetLength(0)) return false;
        if (position.Y < 0 || position.Y >= _tiles.GetLength(1)) return false;
        return true;
    }

    public void SetTileAtPosition(Position position, TileType type) => _tiles[position.X, position.Y] = type;
}

public struct Position(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) => new Position(a.X - b.X, a.Y - b.Y);
    public static bool operator ==(Position a, Position b) => a.Equals(b);
    public static bool operator !=(Position a, Position b) => !a.Equals(b);
}

public interface IAction
{ 
    void Execute(Game game){}
}

public class MoveAction(Position destination) : IAction
{
    private readonly Position _destination = destination;
    public void Execute(Game game)
    {
        if (game.Level.IsInBounds(game.Player.Position + _destination)) game.Player.Position += _destination;
        else TextHelper.WriteLine("You can't go that way.", ConsoleColor.Magenta);
    }
}

public class FountainAction : IAction
{
    public void Execute(Game game)
    {
        if (game.CurrentTile == TileType.Fountain) game.Fountain = true;
    }
}

public static class TextHelper
{
    public static void WriteLine(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
    }
    
    public static void Write(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
    }
}

public enum TileType
{
    Empty,
    Entrance,
    Fountain
}