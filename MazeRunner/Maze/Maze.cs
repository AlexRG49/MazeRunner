using System;
using System.Collections.Generic;

public class Maze
{
    public int Width { get; }
    public int Height { get; }
    public Cell[,] Grid { get; }
    public (int X, int Y) PlayerPosition { get; private set; }

    public Maze(int width, int height)
    {
        Width = width;
        Height = height;
        Grid = new Cell[Height, Width];
        PlayerPosition = (0, 0); // Inicializar posición del jugador
    }

    public void MovePlayer(int dx, int dy)
    {
        int newX = PlayerPosition.X + dx;
        int newY = PlayerPosition.Y + dy;

        if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
        {
            PlayerPosition = (newX, newY);
        }
    }

    public void TeleportPlayer()
    {
        var random = new Random();
        PlayerPosition = (random.Next(Width), random.Next(Height));
    }

    public void RemoveWall(int dx, int dy)
    {
        int newX = PlayerPosition.X + dx;
        int newY = PlayerPosition.Y + dy;

        if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
        {
            // Eliminar pared entre la celda actual y la nueva
            if (dx == 1) Grid[PlayerPosition.Y, PlayerPosition.X].Walls[1] = false; // Pared derecha
            if (dx == -1) Grid[PlayerPosition.Y, PlayerPosition.X].Walls[3] = false; // Pared izquierda
            if (dy == 1) Grid[PlayerPosition.Y, PlayerPosition.X].Walls[2] = false; // Pared inferior
            if (dy == -1) Grid[PlayerPosition.Y, PlayerPosition.X].Walls[0] = false; // Pared superior
        }
    }

    public bool IsPlayerAtExit()
    {
        return PlayerPosition.X == Width / 2 && PlayerPosition.Y == Height / 2;
    }

    public Cell GetNeighbour(Cell cell, int dx, int dy)
    {
        int nx = cell.X + dx;
        int ny = cell.Y + dy;

        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
            return Grid[ny, nx];
        return null!; // Usamos null! para evitar el warning CS8603
    }

    // Método para verificar si el laberinto es completamente conectado
    public bool IsFullyConnected()
    {
        // Matriz para marcar las celdas visitadas
        bool[,] visited = new bool[Height, Width];

        // Pila para el recorrido DFS
        Stack<Cell> stack = new Stack<Cell>();

        // Comenzar desde la primera celda (entrada superior izquierda)
        stack.Push(Grid[0, 0]);
        visited[0, 0] = true;

        // Recorrer el laberinto usando DFS
        while (stack.Count > 0)
        {
            Cell current = stack.Pop();

            // Obtener vecinos no visitados
            var neighbors = new List<Cell>
            {
                GetNeighbour(current, 0, -1), // Arriba
                GetNeighbour(current, 1, 0),  // Derecha
                GetNeighbour(current, 0, 1),  // Abajo
                GetNeighbour(current, -1, 0)  // Izquierda
            };

            foreach (var neighbor in neighbors)
            {
                if (neighbor != null && !visited[neighbor.Y, neighbor.X])
                {
                    // Verificar si no hay pared entre la celda actual y el vecino
                    if (!HasWallBetween(current, neighbor))
                    {
                        visited[neighbor.Y, neighbor.X] = true;
                        stack.Push(neighbor);
                    }
                }
            }
        }

        // Verificar si todas las celdas fueron visitadas
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!visited[y, x])
                    return false; // Hay celdas inalcanzables
            }
        }

        return true; // Todas las celdas son accesibles
    }

    // Método para verificar si hay una pared entre dos celdas adyacentes
    private bool HasWallBetween(Cell current, Cell neighbor)
    {
        if (neighbor.X == current.X + 1) // Vecino a la derecha
            return current.Walls[1]; // Pared derecha de la celda actual
        if (neighbor.X == current.X - 1) // Vecino a la izquierda
            return current.Walls[3]; // Pared izquierda de la celda actual
        if (neighbor.Y == current.Y + 1) // Vecino abajo
            return current.Walls[2]; // Pared inferior de la celda actual
        if (neighbor.Y == current.Y - 1) // Vecino arriba
            return current.Walls[0]; // Pared superior de la celda actual

        return false;
    }
}