public class Saber : Player
{
    public Saber(string name) : base(name, 100, 20, 10) { }

    public override void SpecialAbility()
    {
        if (Cooldown > 0) return;
        
        SetCooldown(4);
        Console.WriteLine($"{Name} usa [gold1]Excalibur[/]: ¡Espada luminosa destruye obstáculos!");
    }

    public void Slash(Maze maze, int dx, int dy)
    {
        for (int i = 0; i < 3; i++) maze.MovePlayer(dx, dy);
    }
}