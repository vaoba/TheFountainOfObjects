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
            
            if (CheckWin() || CheckLose()) break;
            
            IAction action = GetAction();
            action.Execute(this);
        }
    }

    private void ShowStatus()
    {
        Console.WriteLine("----------------------------------------------------------------------------------------------");
        Console.WriteLine($"You are in the room [{Player.Position.X}, {Player.Position.Y}]");
        Console.WriteLine(CurrentTile);
        //TODO : SENSING
    }
    
    private bool CheckWin()
    {
        if (Fountain && CurrentTile == TileType.Entrance) return true;
        return false;
    }

    private bool CheckLose()
    {
        if (!Player.IsAlive) return true;
        return false;
    }

    private IAction GetAction()
    {
        Console.WriteLine("Choose an action: ");
        while (true)
        {
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
}

public class Level(int size)
{
    private TileType[,] _tiles = new TileType[size, size];

    public TileType GetTileAt(Position position)
    {
        return _tiles[position.X, position.Y];
    }

    public bool IsInBounds(Position position)
    {
        if (position.X < 0 || position.X > _tiles.GetLength(0)) return false;
        if (position.Y < 0 || position.Y > _tiles.GetLength(1)) return false;
        return true;
    }

    public void SetTileAtPosition(Position position, TileType type) => _tiles[position.X, position.Y] = type;
}

public struct Position(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public static Position operator +(Position a, Position b)
    {
        return new Position(a.X + b.X, a.Y + b.Y);
    }

    public static Position operator -(Position a, Position b)
    {
        return new Position(a.X - b.X, a.Y - b.Y);
    }

    public static bool operator ==(Position a, Position b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Position a, Position b)
    {
        return !a.Equals(b);
    }
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
        if (game.Level.IsInBounds(game.Player.Position + _destination))
        {
            game.Player.Position += _destination;
        }
    }
}

public class FountainAction : IAction
{
    public void Execute(Game game)
    {
        if (game.CurrentTile == TileType.Fountain)
        {
            game.Fountain = true;
        }
    }
}

public enum TileType
{
    Empty,
    Entrance,
    Fountain
}