public class Archer : Player
{
    public Archer(string name) : base(name, 80, 25, 5) { }

    public override void SpecialAbility()
    {
        if (Cooldown > 0) return;
        
        SetCooldown(4);
        Console.WriteLine($"{Name} usa [gold3]Puerta de Babilonia[/]: ¡Lluvia de armas nobles!");
    }

    public void Shoot(Maze maze, int dx, int dy)
    {
        maze.RemoveWall(dx, dy);
        maze.MovePlayer(dx, dy);
        maze.MovePlayer(dx, dy);
    }
}