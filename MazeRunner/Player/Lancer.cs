public class Lancer : Player
{
    public Lancer(string name) : base(name, 90, 22, 8) { }

    public override void SpecialAbility()
    {
        if (Cooldown > 0) return;
        
        SetCooldown(5);
        Console.WriteLine($"{Name} usa [red]Gáe Bolg[/]: ¡Lanza maldita atraviesa todo!");
    }

    public void Charge(Maze maze, int dx, int dy)
    {
        for (int i = 0; i < 4; i++) maze.MovePlayer(dx, dy);
    }
}