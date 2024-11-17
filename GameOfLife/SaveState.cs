using System.Drawing;

namespace GameOfLife;

public struct SaveState()
{
    public Statistics stats;
    public uint x;
    public uint y;
    public List<Point> aliveCells;
}
