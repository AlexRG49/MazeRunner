using Spectre.Console;
using System;
using System.Threading;

class Program
{
    private static Player player1;
    private static Player player2;
    private static Maze maze;
    private static int turnTime = 15; // Tiempo por turno en segundos

    static void Main(string[] args)
    {
        while (true)
        {
            AnsiConsole.Clear();
            DisplayTitle();
            var choice = ShowMainMenu();

            switch (choice)
            {
                case "Nueva Partida":
                    StartNewGame();
                    break;
                case "Cargar Partida":
                    LoadGame();
                    break;
                case "Ayuda":
                    ShowHelp();
                    break;
                case "Créditos":
                    ShowCredits();
                    break;
                case "Salir":
                    return;
            }
        }
    }

    static void DisplayTitle()
    {
        var title = new FigletText("MazeServant")
            .Centered()
            .Color(Color.Red);

        var rule = new Rule("[blue]Laberinto Mágico[/]").RuleStyle(Style.Parse("blue dim"));

        AnsiConsole.Write(title);
        AnsiConsole.Write(rule);
        AnsiConsole.WriteLine();
    }

    static string ShowMainMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Menú Principal[/]")
                .PageSize(5)
                .HighlightStyle(new Style(foreground: Color.Blue))
                .AddChoices(new[]
                {
                    "Nueva Partida",
                    "Cargar Partida",
                    "Ayuda",
                    "Créditos",
                    "Salir"
                }));
    }

    static void StartNewGame()
    {
        const int width = 15;
        const int height = 15;

        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[blue]Nueva Partida[/]").RuleStyle(Style.Parse("blue")));

        maze = new Maze(width, height);
        var generator = new MazeGenerator(maze);

        AnsiConsole.Status()
            .Start("Generando laberinto...", ctx =>
            {
                generator.Generate();
            });

        RenderMaze(maze);

        // Selección de personajes
        player1 = SelectPlayer("Jugador 1");
        player2 = SelectPlayer("Jugador 2");

        // Lógica del juego por turnos
        PlayGame();
    }

    static Player SelectPlayer(string playerName)
    {
        var playerType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Selecciona el personaje para {playerName}")
                .AddChoices(new[] { 
                    "Rider (Alejandro Magno)", 
                    "Saber (Arturia Pendragon)", 
                    "Caster (Gilles de Rais)", 
                    "Lancer (Cú Chulainn)", 
                    "Archer (Gilgamesh)" 
                }));

        return playerType switch
        {
            "Rider (Alejandro Magno)" => new Rider("Rider"),
            "Saber (Arturia Pendragon)" => new Saber("Saber"),
            "Caster (Gilles de Rais)" => new Caster("Caster"),
            "Lancer (Cú Chulainn)" => new Lancer("Lancer"),
            "Archer (Gilgamesh)" => new Archer("Archer"),
            _ => throw new InvalidOperationException("Jugador no válido.")
        };
    }

   static void PlayGame()
{
    int currentPlayer = 1; // 1 para Jugador 1, 2 para Jugador 2
    Rider rider = null;
    Saber saber = null;
    Lancer lancer = null;
    Archer archer = null;

    while (true)
    {
        AnsiConsole.Clear();
        RenderMaze(maze);
        RenderPlayerInfo();

        var current = currentPlayer == 1 ? player1 : player2;
        AnsiConsole.MarkupLine($"[yellow]Turno de {current.Name}[/]");

        // Temporizador de turno
        var cts = new CancellationTokenSource();
        var timerTask = Task.Run(() => TurnTimer(cts.Token));

        // Captura de teclas
        ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
        cts.Cancel(); // Detener el temporizador

        // Assign the current player to the appropriate variable
        rider = current as Rider;
        saber = current as Saber;
        lancer = current as Lancer;
        archer = current as Archer;

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (rider != null) rider.Move(maze, 0, -1);
                else if (saber != null) saber.Slash(maze, 0, -1);
                else if (lancer != null) lancer.Charge(maze, 0, -1);
                else if (archer != null) archer.Shoot(maze, 0, -1);
                break;

            case ConsoleKey.DownArrow:
                if (rider != null) rider.Move(maze, 0, 1);
                else if (saber != null) saber.Slash(maze, 0, 1);
                else if (lancer != null) lancer.Charge(maze, 0, 1);
                else if (archer != null) archer.Shoot(maze, 0, 1);
                break;

            case ConsoleKey.LeftArrow:
                if (rider != null) rider.Move(maze, -1, 0);
                else if (saber != null) saber.Slash(maze, -1, 0);
                else if (lancer != null) lancer.Charge(maze, -1, 0);
                else if (archer != null) archer.Shoot(maze, -1, 0);
                break;

            case ConsoleKey.RightArrow:
                if (rider != null) rider.Move(maze, 1, 0);
                else if (saber != null) saber.Slash(maze, 1, 0);
                else if (lancer != null) lancer.Charge(maze, 1, 0);
                else if (archer != null) archer.Shoot(maze, 1, 0);
                break;

            case ConsoleKey.H:
                current.SpecialAbility();
                if (current is Caster caster) caster.Teleport(maze);
                break;

            case ConsoleKey.Escape:
                return; // Salir del juego
        }

        // Verificar si el jugador ha llegado al objetivo
        if (maze.IsPlayerAtExit())
        {
            AnsiConsole.MarkupLine($"[green]¡{current.Name} ha ganado![/]");
            Console.ReadKey();
            return;
        }

        // Cambiar de turno
        currentPlayer = currentPlayer == 1 ? 2 : 1;
    }
}


    static void TurnTimer(CancellationToken token)
    {
        int timeLeft = turnTime;
        while (timeLeft > 0 && !token.IsCancellationRequested)
        {
            AnsiConsole.Cursor.SetPosition(0, Console.WindowHeight - 2);
            AnsiConsole.MarkupLine($"[yellow]Tiempo restante: {timeLeft} segundos[/]");
            Thread.Sleep(1000);
            timeLeft--;
        }

        if (timeLeft == 0)
        {
            AnsiConsole.Cursor.SetPosition(0, Console.WindowHeight - 2);
            AnsiConsole.MarkupLine("[red]¡Tiempo agotado![/]");
            Thread.Sleep(1000);
        }
    }

    static void RenderPlayerInfo()
    {
        // Mostrar salud de los jugadores
        AnsiConsole.MarkupLine($"[green]{player1.Name}: {GetHearts(player1.Health)}[/]");
        AnsiConsole.MarkupLine($"[blue]{player2.Name}: {GetHearts(player2.Health)}[/]");

        // Mostrar tiempo restante
        AnsiConsole.Cursor.SetPosition(0, Console.WindowHeight - 2);
        AnsiConsole.MarkupLine($"[yellow]Tiempo restante: {turnTime} segundos[/]");
    }

    static string GetHearts(int health)
    {
        string hearts = "";
        for (int i = 0; i < health / 10; i++)
            hearts += "❤️";
        if (health % 10 >= 5)
            hearts += "💔";
        return hearts;
    }

    static void LoadGame()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[blue]Cargar Partida[/]").RuleStyle(Style.Parse("blue")));
        AnsiConsole.MarkupLine("\n[yellow]No hay partidas guardadas disponibles.[/]");
        AnsiConsole.MarkupLine("\nPresiona cualquier tecla para volver al menú...");
        Console.ReadKey();
    }

    static void ShowHelp()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[blue]Ayuda[/]").RuleStyle(Style.Parse("blue")));

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Comando")
            .AddColumn("Descripción");

        table.AddRow("[blue]►[/]", "Entrada del laberinto");
        table.AddRow("[red]◄[/]", "Salida del laberinto");
        table.AddRow("[blue]◊[/]", "Objetivo central");
        table.AddRow("║ ══", "Paredes del laberinto");

        AnsiConsole.Write(table);

        AnsiConsole.MarkupLine("\nPresiona cualquier tecla para volver al menú...");
        Console.ReadKey();
    }

    static void ShowCredits()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[white]Créditos[/]").RuleStyle(Style.Parse("white")));

        var panel = new Panel(
            Align.Center(
                new Markup(
                    "[white]Desarrollado con esfuerzo pero sobre todo apuro[/]\n" +
                    "[white]Especiales agradecimientos a:[/]\n" +
                    "[white]• El Café: de esta desarrollo insomnio[/]\n" +
                    "[white]• El Cigarro: que no ayudo en nada que subiera tanto[/]\n" +
                    "[white]• El Katrib: que la mitad de veces no entendi[/]\n" +
                    "[white]• Perplexity: 5mentarios[/]\n" +
                    "[white]Versión 1.0, en un futuro si paso de semestre hago algo mas respetable profe[/]"
                )
            ))
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("white"))
            .Padding(2, 1);

        AnsiConsole.Write(panel);

        AnsiConsole.MarkupLine("\n[white]Presiona cualquier tecla para volver al menú...[/]");
        Console.ReadKey();
    }

    static void RenderMaze(Maze maze)
    {
        const string WALL_VERTICAL = "║";
        const string WALL_HORIZONTAL = "══";
        const string CORNER_TOP_LEFT = "╔";
        const string CORNER_TOP_RIGHT = "╗";
        const string CORNER_BOTTOM_LEFT = "╚";
        const string CORNER_BOTTOM_RIGHT = "╝";
        const string JUNCTION_LEFT = "╠";
        const string JUNCTION_RIGHT = "╣";
        const string JUNCTION_TOP = "╦";
        const string JUNCTION_BOTTOM = "╩";
        const string CROSS = "╬";
        const string EMPTY = "  ";

        var table = new Table()
            .HideHeaders()
            .BorderStyle(Style.Plain)
            .MinimalBorder()
            .NoBorder();

        // Añadir columnas al table
        for (int x = 0; x < maze.Width; x++)
            table.AddColumn(new TableColumn("").PadRight(0).PadLeft(0));

        // Primera fila (borde superior)
        var firstRow = new List<string>();
        for (int x = 0; x < maze.Width; x++)
        {
            if (x == 0)
                firstRow.Add($"[white]{CORNER_TOP_LEFT}{WALL_HORIZONTAL}[/]");
            else if (x == maze.Width - 1)
                firstRow.Add($"[white]{WALL_HORIZONTAL}{CORNER_TOP_RIGHT}[/]");
            else
                firstRow.Add($"[white]{WALL_HORIZONTAL}{JUNCTION_TOP}[/]");
        }
        table.AddRow(firstRow.ToArray());

        // Filas de celdas y paredes verticales
        for (int y = 0; y < maze.Height; y++)
        {
            var row = new List<string>();
            for (int x = 0; x < maze.Width; x++)
            {
                Cell cell = maze.Grid[y, x];
                string content = cell.IsEntrance ? "[green]►[/]" :
                                cell.IsExit ? "[red]◄[/]" :
                                (cell.X == maze.Width / 2 && cell.Y == maze.Height / 2) ? "[blue]◊[/]" :
                                EMPTY;

                // Pared izquierda de la celda
                string cellDisplay = cell.Walls[3] ? $"[white]{WALL_VERTICAL}[/]" : " ";
                cellDisplay += content;

                // Pared derecha de la celda
                if (x == maze.Width - 1)
                {
                    cellDisplay += cell.Walls[1] ? $"[white]{WALL_VERTICAL}[/]" : " ";
                }
                else
                {
                    Cell rightCell = maze.Grid[y, x + 1];
                    cellDisplay += (cell.Walls[1] || rightCell.Walls[3]) ? $"[white]{WALL_VERTICAL}[/]" : " ";
                }

                row.Add(cellDisplay);
            }
            table.AddRow(row.ToArray());

            // Filas de paredes horizontales entre celdas
            if (y < maze.Height - 1)
            {
                var wallRow = new List<string>();
                for (int x = 0; x < maze.Width; x++)
                {
                    Cell cell = maze.Grid[y, x];
                    Cell bottomCell = maze.Grid[y + 1, x];

                    if (x == 0)
                    {
                        // Borde izquierdo
                        wallRow.Add(cell.Walls[2] ? $"[white]{JUNCTION_LEFT}{WALL_HORIZONTAL}[/]" : $"[white]{WALL_VERTICAL}[/] ");
                    }
                    else if (x == maze.Width - 1)
                    {
                        // Borde derecho
                        wallRow.Add(cell.Walls[2] ? $"[white]{WALL_HORIZONTAL}{JUNCTION_RIGHT}[/]" : $" [white]{WALL_VERTICAL}[/]");
                    }
                    else
                    {
                        // Celdas intermedias
                        bool hasBottomWall = cell.Walls[2];
                        bool hasRightWall = cell.Walls[1];
                        bool hasLeftWall = cell.Walls[3];

                        if (hasBottomWall)
                        {
                            if (hasRightWall && hasLeftWall)
                                wallRow.Add($"[white]{WALL_HORIZONTAL}{CROSS}[/]");
                            else if (hasRightWall)
                                wallRow.Add($"[white]{WALL_HORIZONTAL}{JUNCTION_TOP}[/]");
                            else if (hasLeftWall)
                                wallRow.Add($"[white]{JUNCTION_TOP}{WALL_HORIZONTAL}[/]");
                            else
                                wallRow.Add($"[white]{WALL_HORIZONTAL}{WALL_HORIZONTAL}[/]");
                        }
                        else
                        {
                            wallRow.Add("  ");
                        }
                    }
                }
                table.AddRow(wallRow.ToArray());
            }
        }

        // Última fila (borde inferior)
        var lastRow = new List<string>();
        for (int x = 0; x < maze.Width; x++)
        {
            if (x == 0)
                lastRow.Add($"[white]{CORNER_BOTTOM_LEFT}{WALL_HORIZONTAL}[/]");
            else if (x == maze.Width - 1)
                lastRow.Add($"[white]{WALL_HORIZONTAL}{CORNER_BOTTOM_RIGHT}[/]");
            else
                lastRow.Add($"[white]{WALL_HORIZONTAL}{JUNCTION_BOTTOM}[/]");
        }
        table.AddRow(lastRow.ToArray());

        AnsiConsole.Write(table);
    }
}