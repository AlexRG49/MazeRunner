using System;
using System.Collections.Generic;

public class MazeGenerator
{
    private Maze maze;
    private Random random;

    public MazeGenerator(Maze maze)
    {
        this.maze = maze;
        random = new Random();
    }

    public void Generate()
    {
        // Inicializar todas las celdas como no visitadas
        for (int y = 0; y < maze.Height; y++)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                maze.Grid[y, x].Visited = false;
            }
        }

        // Empezar desde una celda aleatoria
        int startX = random.Next(maze.Width);
        int startY = random.Next(maze.Height);
        Cell startCell = maze.Grid[startY, startX];

        // Generar el laberinto usando Recursive Backtracking
        RecursiveBacktracking(startCell);

        // Crear entradas y condición de victoria
        CreateEntrancesAndVictoryCondition();

        // Verificar si el laberinto es completamente conectado
        if (!maze.IsFullyConnected())
        {
            throw new Exception("El laberinto no es completamente conectado.");
        }
    }

    private void RecursiveBacktracking(Cell current)
    {
        current.Visited = true;

        // Obtener vecinos no visitados en orden aleatorio
        var neighbors = GetUnvisitedNeighbors(current);
        Shuffle(neighbors);

        foreach (var neighbor in neighbors)
        {
            if (!neighbor.Visited)
            {
                // Remover la pared entre la celda actual y el vecino
                RemoveWalls(current, neighbor);

                // Llamar recursivamente para el vecino
                RecursiveBacktracking(neighbor);
            }
        }
    }

    private List<Cell> GetUnvisitedNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();

        // Direcciones: [dx, dy]
        var directions = new Tuple<int, int>[]
        {
            new Tuple<int, int>(0, -1), // Arriba
            new Tuple<int, int>(1, 0),  // Derecha
            new Tuple<int, int>(0, 1),  // Abajo
            new Tuple<int, int>(-1, 0)  // Izquierda
        };

        foreach (var dir in directions)
        {
            Cell neighbor = maze.GetNeighbour(cell, dir.Item1, dir.Item2);
            if (neighbor != null && !neighbor.Visited)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private void RemoveWalls(Cell current, Cell neighbor)
    {
        int dx = neighbor.X - current.X;
        int dy = neighbor.Y - current.Y;

        if (dx == 0 && dy == -1) // Arriba
        {
            current.Walls[0] = false; // Eliminar pared superior
            neighbor.Walls[2] = false; // Eliminar pared inferior del vecino
        }
        else if (dx == 1 && dy == 0) // Derecha
        {
            current.Walls[1] = false; // Eliminar pared derecha
            neighbor.Walls[3] = false; // Eliminar pared izquierda del vecino
        }
        else if (dx == 0 && dy == 1) // Abajo
        {
            current.Walls[2] = false; // Eliminar pared inferior
            neighbor.Walls[0] = false; // Eliminar pared superior del vecino
        }
        else if (dx == -1 && dy == 0) // Izquierda
        {
            current.Walls[3] = false; // Eliminar pared izquierda
            neighbor.Walls[1] = false; // Eliminar pared derecha del vecino
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void CreateEntrancesAndVictoryCondition()
    {
        // Crear entradas para los jugadores en las esquinas opuestas.
        maze.Grid[0, 0].IsEntrance = true;               // Entrada superior izquierda.
        maze.Grid[maze.Height - 1, maze.Width - 1].IsEntrance = true; // Entrada inferior derecha.

        // Marcar la celda central como el objetivo de victoria.
        var centerX = maze.Width / 2;
        var centerY = maze.Height / 2;
        maze.Grid[centerY, centerX].IsExit = true;       // Condición de victoria.
    }
}