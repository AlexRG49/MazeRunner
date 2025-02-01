public class Cell
{
    public int X { get; }
    public int Y { get; }
    public bool Visited { get; set; }
    public bool[] Walls { get; } // [Arriba, Derecha, Abajo, Izquierda]
    public bool IsEntrance { get; set; }
    public bool IsExit { get; set; }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        Visited = false;
        Walls = new bool[] { true, true, true, true }; // Todas las paredes están presentes inicialmente
        IsEntrance = false;
        IsExit = false;
    }
}