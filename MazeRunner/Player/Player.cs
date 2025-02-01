public abstract class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    public int Cooldown { get; private set; } = 0;

    protected Player(string name, int health, int attackPower, int defense)
    {
        Name = name;
        Health = health;
        AttackPower = attackPower;
        Defense = defense;
    }

    public abstract void SpecialAbility();

    public void TakeDamage(int damage)
    {
        int actualDamage = Math.Max(0, damage - Defense);
        Health -= actualDamage;
    }

    public void UpdateCooldown() => Cooldown = Math.Max(0, Cooldown - 1);
    public void SetCooldown(int cooldown) => Cooldown = cooldown;
    public bool IsAlive => Health > 0;
}