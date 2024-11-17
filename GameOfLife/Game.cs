using System.Drawing;
using System.Xml.Serialization;

namespace GameOfLife;

public class Game(Cell[,] initialState, uint x, uint y)
{
    public uint X { get; private set; } = x;
    public uint Y { get; private set; } = y;

    private Statistics _statistics;

    public Statistics Statistics => _statistics;

    public Cell[,] Grid { get; private set; } = initialState;

    public static Game Empty(uint x, uint y)
    {
        var grid = new Cell[x, y];

        for (uint i = 0; i < x; i++)
        for (uint j = 0; j < y; j++)
            grid[i, j] = Cell.Dead();

        return new Game(grid, x, y);
    }

    public static Game Rand(uint x, uint y)
    {
        var rand = new Random();
        var grid = new Cell[x, y];

        for (uint i = 0; i < x; i++)
        for (uint j = 0; j < y; j++)
            grid[i, j] = rand.Next(10) < 7 ? Cell.Dead() : Cell.Alive();

        return new Game(grid, x, y);
    }

    public void Export(string path)
    {
        using var fs = new FileStream(path, FileMode.Create);

        var saveState = new SaveState
        {
            stats = _statistics,
            x = X,
            y = Y,
            aliveCells = [],
        };
        var ser = new XmlSerializer(typeof(SaveState));

        for (var i = 0; i < x; i++)
        for (var j = 0; j < y; j++)
            if (Grid[i, j].State)
                saveState.aliveCells.Add(new Point(i, j));

        ser.Serialize(fs, saveState);
    }

    public static Game Import(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Open);
        var ser = new XmlSerializer(typeof(SaveState));

        if (ser.Deserialize(fs) is not SaveState saveState)
            throw new Exception("Cannot Deserialize");

        var game = Game.Empty(saveState.x, saveState.y);
        foreach (var cell in saveState.aliveCells)
            game.Grid[cell.X, cell.Y].State = true;
        game._statistics = saveState.stats;

        return game as Game;
    }

    public void Clean()
    {
        _statistics = new Statistics();
        for (uint i = 0; i < X; i++)
        for (uint j = 0; j < Y; j++)
            Grid[i, j].State = false;
    }

    public void Resize(uint x, uint y)
    {
        var grid = new Cell[x, y];
        for (uint i = 0; i < x; i++)
        for (uint j = 0; j < y; j++)
            grid[i, j] = Cell.Dead();

        for (uint i = 0; i < X; i++)
        for (uint j = 0; j < Y; j++)
            grid[i, j] = Grid[i, j];
        X = x;
        Y = y;
    }

    public void NextGeneration()
    {
        var newGrid = new Cell[X, Y];

        for (uint i = 0; i < X; i++)
        for (uint j = 0; j < Y; j++)
            newGrid[i, j] = CalcSurvival(i, j);

        for (uint i = 0; i < X; i++)
        for (uint j = 0; j < Y; j++)
        {
            if (Grid[i, j].State == newGrid[i, j].State)
                continue;

            Grid[i, j].State = newGrid[i, j].State;
            if (newGrid[i, j].State)
                _statistics.born++;
            else
                _statistics.died++;
        }

        _statistics.generation++;
    }

    private Cell CalcSurvival(uint i, uint j)
    {
        var aliveNeighbours = CountAliveNeighbours(i, j);

        if (Grid[i, j].State == false)
            return aliveNeighbours == 3 ? Cell.Alive() : Cell.Dead();

        return aliveNeighbours switch
        {
            < 2 => Cell.Dead(),
            2 or 3 => Cell.Alive(),
            _ => Cell.Dead(),
        };
    }

    private uint CountAliveNeighbours(uint i, uint j)
    {
        uint count = 0;

        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                var neighbourX = i + x;
                var neighbourY = j + y;

                if (
                    neighbourX < 0
                    || neighbourX >= this.X
                    || neighbourY < 0
                    || neighbourY >= this.Y
                )
                    continue;
                if (Grid[neighbourX, neighbourY].State == true)
                    count++;
            }
        }

        return count;
    }
}
