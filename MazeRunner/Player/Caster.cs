public class Caster : Player
{
    public Caster(string name) : base(name, 70, 15, 7) { }

    public override void SpecialAbility()
    {
        if (Cooldown > 0) return;
        
        SetCooldown(6);
        Console.WriteLine($"{Name} usa [darkorange]Prelati's Spellbook[/]: ¡Teletransportación caótica!");
    }

    public void Teleport(Maze maze) => maze.TeleportPlayer();
}