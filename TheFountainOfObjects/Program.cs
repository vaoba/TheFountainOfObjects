namespace TheFountainOfObjects;

class Program
{
    static void Main()
    {
        Tile[,] Tiles = new Tile[3, 3];
        Console.WriteLine(Tiles[1, 1].Type);
        
        Console.WriteLine("Hello, World!");
    }
}

public class Game
{
    public bool Fountain { get; set; } = false;

    public void Update()
    {
        Level level = new(3);
        Player player = new Player(0, 0);
        
        while (true)
        {
            if (CheckWin() || CheckLose()) break;
        }
    }

    private bool CheckWin()
    {
        return false;
    }

    private bool CheckLose()
    {
        return false;
    }
}

public class Player(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}

public class Level
{
    public Tile[,] Tiles { get; init; }

    public Level(int size)
    {
        Tiles = new Tile[size, size];
        FillLevel();
    }

    public void FillLevel()
    {
        for (int y = 0; y < Tiles.GetLength(1); y++)
        {
            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                if (x == 0 && y == 0) Tiles[x, y] = new(x, y, TileType.Entrance);
                else if (x == 0 && y == 2) Tiles[x, y] = new Tile(x, y, TileType.Fountain);
                else Tiles[x, y] = new Tile(x, y, TileType.Empty);
            }
        }
    }
}

public record Tile(int X, int Y, TileType Type);

public enum TileType
{
    Empty,
    Entrance,
    Fountain
}