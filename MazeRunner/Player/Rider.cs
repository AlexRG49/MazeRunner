public class Rider : Player
{
    public int SpeedBoost { get; private set; }

    public Rider(string name) : base(name, 85, 18, 6) { }

    public override void SpecialAbility()
    {
        if (Cooldown > 0) return;
        
        SpeedBoost = 3;
        SetCooldown(5);
        Console.WriteLine($"{Name} usa [gold1]Ionioi Hetairoi[/]: ¡Ejército de conquista aumenta su velocidad!");
    }

    public void Move(Maze maze, int dx, int dy)
    {
        maze.MovePlayer(dx, dy);
        if (SpeedBoost > 0) SpeedBoost--;
    }
}