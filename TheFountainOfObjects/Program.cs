namespace TheFountainOfObjects;

static class Program
{
    static void Main()
    {
        Game game = GetGame();
        
        Console.Clear();
        
        TextHelper.Write(@"You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search of the Fountain of Objects.

Light is visible only in the entrance, and no other light is seen anywhere in the caverns.

You must navigate the Caverns with your senses.

Find the Fountain of Objects, activate it, and return to the entrance.

Look out for pits. You will feel a breeze if a pit is in an adjacent room. If you enter a room with a pit, you will die.

Maelstroms are violent forces of sentient wind. Entering a room with one could transport you to any other location in the caverns. You will be able to hear their growling and groaning in nearby rooms

You carry with you a bow and a quiver of arrows. You can use them to shoot monsters in the caverns but be warned: you have limited supply.

Press enter to continue..", ConsoleColor.Magenta);

        Console.ReadLine();
        Console.Clear();
        
        game.Update();
    }

    static Game GetGame()
    {
        TextHelper.WriteLine("Welcome, choose a world size (small, medium, large) to begin the game: ", ConsoleColor.Magenta);
        Console.ForegroundColor = ConsoleColor.Cyan;
        while (true)
        {
            string? input = Console.ReadLine();

            Game? game = input switch
            {
                "small" => CreateGame4X4(),
                "medium" => CreateGame6X6(),
                "large" => CreateGame8X8(),
                _ => null
            };

            if (game != null) return game;
        }
    }

    static Game CreateGame4X4()
    {
        Level level = new(4);
        // ENTRANCE, FOUNTAIN
        Position start = new Position(0, 0);
        Position fountain = new Position(2, 0);
        level.SetTileAtPosition(start, TileType.Entrance);
        level.SetTileAtPosition(fountain, TileType.Fountain);
        // PITS
        level.SetTileAtPosition(new(3, 3), TileType.Pit);
        // MAELSTROM
        level.SetTileAtPosition(new(2, 2), TileType.Maelstrom);
        // AMAROK
        level.SetTileAtPosition(new(0, 2), TileType.Amarok);
        
        Player player = new(start);
        return new Game(level, player);
    }

    static Game CreateGame6X6()
    {
        Level level = new(6);
        // ENTRANCE, FOUNTAIN
        Position start = new Position(0, 0);
        Position fountain = new Position(4, 4);
        level.SetTileAtPosition(start, TileType.Entrance);
        level.SetTileAtPosition(fountain, TileType.Fountain);
        // PITS
        level.SetTileAtPosition(new(5,5), TileType.Pit);
        level.SetTileAtPosition(new(0,5), TileType.Pit);
        // MAELSTROM
        level.SetTileAtPosition(new(2, 3), TileType.Maelstrom);
        // AMAROK
        level.SetTileAtPosition(new(1, 2), TileType.Amarok);
        level.SetTileAtPosition(new(4, 2), TileType.Amarok);
        
        Player player = new(start);
        return new Game(level, player);
    }
    
    static Game CreateGame8X8()
    {
        Level level = new(8);
        // ENTRANCE, FOUNTAIN
        Position start = new Position(0, 0);
        Position fountain = new Position(5, 5);
        level.SetTileAtPosition(start, TileType.Entrance);
        level.SetTileAtPosition(fountain, TileType.Fountain);
        // PITS
        level.SetTileAtPosition(new(7,7), TileType.Pit);
        level.SetTileAtPosition(new(0,7), TileType.Pit);
        level.SetTileAtPosition(new(7,0), TileType.Pit);
        // MAELSTROM
        level.SetTileAtPosition(new(2, 4), TileType.Maelstrom);
        level.SetTileAtPosition(new(6, 6), TileType.Maelstrom);
        // AMAROK
        level.SetTileAtPosition(new(1, 2), TileType.Amarok);
        level.SetTileAtPosition(new(4, 2), TileType.Amarok);
        level.SetTileAtPosition(new(3, 5), TileType.Amarok);
        
        Player player = new(start);
        return new Game(level, player);
    }
}

public class Game(Level level, Player player)
{
    public bool Fountain { get; set; }
    public Level Level { get; } = level;
    public Player Player { get; } = player;
    public TileType CurrentTile { get; private set; } = TileType.Entrance;

    public void Update()
    {
        while (true)
        {
            // Level.PrintLevel(this);
            
            CurrentTile = Level.GetTileAt(Player.Position);
            ShowStatus();

            if (CurrentTile == TileType.Maelstrom)
            {
                MaelstromInteraction();
                continue;
            }

            Player.Sense(this);
            
            if (CheckWin() || CheckLose()) break;
            
            IAction action = GetAction();
            action.Execute(this);
        }
    }

    private void ShowStatus()
    {
        TextHelper.WriteLine("----------------------------------------------------------------------------------------------", ConsoleColor.White);
        TextHelper.WriteLine($"You are in the room [{Player.Position.X}, {Player.Position.Y}]. Arrows {Player.Arrows} / 5.", ConsoleColor.White);
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
        if (CurrentTile == TileType.Pit)
        {
            TextHelper.WriteLine("You have fallen into a pit, the game is over!", ConsoleColor.Red);
            TextHelper.WriteLine("Press enter to end the game..", ConsoleColor.Gray);
            Console.ReadLine();
            return true;
        }

        if (CurrentTile == TileType.Amarok)
        {
            TextHelper.WriteLine("You have been slain by an Amarok, the game is over!", ConsoleColor.Red);
            TextHelper.WriteLine("Press enter to end the game..", ConsoleColor.Gray);
            Console.ReadLine();
            return true;
        }
        
        return false;
    }

    private void MaelstromInteraction()
    {
        Level.SetTileAtPosition(Player.Position, TileType.Empty);
        Level.SetTileAtPosition(new(Math.Clamp(Player.Position.X - 2, 0, Level.Size - 1), Math.Clamp(Player.Position.Y - 1, 0, Level.Size - 1)), TileType.Maelstrom);
        Player.Position = new(Math.Clamp(Player.Position.X + 2, 0, Level.Size - 1), Math.Clamp(Player.Position.Y + 1, 0, Level.Size - 1));
        
        TextHelper.WriteLine("You have been swept away by the powerful wind of a Maelstrom!", ConsoleColor.Red);
    }

    private IAction GetAction()
    {
        TextHelper.WriteLine("Choose an action: ", ConsoleColor.White);
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            string? input = Console.ReadLine();

            IAction? action = input switch
            {
                "move north" => new MoveAction(new(0, 1)),
                "move east" => new MoveAction(new(1, 0)),
                "move south" => new MoveAction(new(0, -1)),
                "move west" => new MoveAction(new(-1, 0)),
                "enable fountain" => new FountainAction(),
                "shoot north" => new ShootAction(new(0, 1)),
                "shoot east" => new ShootAction(new(1, 0)),
                "shoot south" => new ShootAction(new(0, -1)),
                "shoot west" => new ShootAction(new(-1, 0)),
                "help" => new HelpAction(),
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
    public int Arrows { get; set; } = 5;

    public void Sense(Game game)
    {
        EntranceSense(game);
        FountainSense(game);
        AdjacentSense(game);
    }

    private void EntranceSense(Game game)
    {
        if (game.CurrentTile == TileType.Entrance) TextHelper.WriteLine(game.Fountain ? "The Fountain of Objects has been reactivated, and you have escaped with your life!" : "You see light coming from the cavern entrance.", ConsoleColor.Yellow);
    }

    private void FountainSense(Game game)
    {
        if (game.CurrentTile == TileType.Fountain) TextHelper.WriteLine(game.Fountain ? "You hear the rushing waters from the Fountain of Objects. It has been reactivated!" : "You hear water dripping in this room. The Fountain of Objects is here!", ConsoleColor.Blue);
    }

    private void AdjacentSense(Game game)
    {
        if (game.Level.IsTypeAdjacent(Position, TileType.Pit, false)) TextHelper.WriteLine("You feel a draft. There is a pit in a nearby room.", ConsoleColor.Magenta);
        if (game.Level.IsTypeAdjacent(Position, TileType.Maelstrom, true)) TextHelper.WriteLine("You hear the growling and groaning of a maelstrom nearby.", ConsoleColor.Magenta);
        if (game.Level.IsTypeAdjacent(Position, TileType.Amarok, true)) TextHelper.WriteLine("You can smell the rotten stench of an amarok in a nearby room.", ConsoleColor.Magenta);
    }
}

public class Level(int size)
{
    public int Size { get; } = size;
    private readonly TileType[,] _tiles = new TileType[size, size];
    private readonly Position[] _directions = new Position[] { new(1, 0), new(0, 1), new(-1, 0), new(0, -1), new(1, 1), new(-1, 1), new(-1, -1), new(1, -1) };
    
    public TileType GetTileAt(Position position) => _tiles[position.X, position.Y];

    public bool IsInBounds(Position position)
    {
        if (position.X < 0 || position.X >= _tiles.GetLength(0)) return false;
        if (position.Y < 0 || position.Y >= _tiles.GetLength(1)) return false;
        return true;
    }

    public void SetTileAtPosition(Position position, TileType type) => _tiles[position.X, position.Y] = type;

    public bool IsTypeAdjacent(Position position, TileType type, bool diagonal)
    {
        int length = diagonal ? 8 : 4;
        
        for (int i = 0; i < length; i++)
        {
            if (IsInBounds(position + _directions[i]))
            {
                if (_tiles[position.X + _directions[i].X, position.Y + _directions[i].Y] == type) return true;
            }
        }
        return false;
    }

    public void PrintLevel(Game game)
    {
        Console.Clear();
        for (int y = _tiles.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                if (x == game.Player.Position.X && y == game.Player.Position.Y) TextHelper.Write($"[{_tiles[x,y].ToString()}] ", ConsoleColor.Green);
                else if (_tiles[x,y] != TileType.Empty) TextHelper.Write($"[{_tiles[x,y].ToString()}] ", ConsoleColor.Red);
                else TextHelper.Write($"[{_tiles[x,y].ToString()}] ", ConsoleColor.Yellow);
            }
            Console.WriteLine();
        }
    }
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
    public void Execute(Game game)
    {
        if (game.Level.IsInBounds(game.Player.Position + destination)) game.Player.Position += destination;
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

public class ShootAction(Position destination) : IAction
{
    public void Execute(Game game)
    {
        if (game.Player.Arrows > 0)
        {
            game.Player.Arrows -= 1;
            if (game.Level.GetTileAt(game.Player.Position + destination) == TileType.Amarok)
            {
                game.Level.SetTileAtPosition(game.Player.Position + destination, TileType.Empty);
                TextHelper.WriteLine("You have killed an Amarok!", ConsoleColor.Green);
            }
            else if (game.Level.GetTileAt(game.Player.Position + destination) == TileType.Maelstrom)
            {
                game.Level.SetTileAtPosition(game.Player.Position + destination, TileType.Empty);
                TextHelper.WriteLine("You have destroyed a Maelstrom!", ConsoleColor.Green);
            }
            else TextHelper.WriteLine("The room you have shot at is completely empty.", ConsoleColor.Red);
        }
        else TextHelper.WriteLine("You do not have any more arrows.", ConsoleColor.Red);
    }
}

public class HelpAction : IAction
{
    public void Execute(Game game)
    {
        TextHelper.WriteLine(@"The move keyword lets you travel through the caverns, write any of the following to move:
[move north], [move east], [move south], [move west]

The shoot keyword lets you attack whatever resides in an adjacent room, in your chosen direction, write any of the following to shoot:
[shoot north], [shoot east], [shoot south], [shoot west]

Write [enable fountain] to activate the fountain while standing in the fountain room, the rush back to the entrance.", ConsoleColor.White);
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
    Fountain,
    Pit,
    Maelstrom,
    Amarok
}