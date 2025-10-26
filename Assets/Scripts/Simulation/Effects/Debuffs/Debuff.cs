namespace Undermarch
{
    public class Debuff : ICharacterEffect
    {
        public string name { get; set; }
        public string description { get; set; }
        public int duration { get; set; }

        public void Add(Character target)
        {
            target.debuffs.Add(this);
        }

        public void Remove(Character target)
        {
            target.debuffs.Remove(this);
        }

        // Allow subclasses to define custom behavior
        public virtual void Apply(Character target) { }
    }

    public class AgilityDebuff1 : Debuff
    {
        public AgilityDebuff1()
        {
            name = "Agility Debuff Rank 1";
            description = "Decreases Agility by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.agility -= 1;
        }
    }

    public class IntelligenceDebuff1 : Debuff
    {
        public IntelligenceDebuff1()
        {
            name = "Intelligence Debuff Rank 1";
            description = "Decreases Intelligence by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.intelligence -= 1;
        }
    }

    public class StaminaDebuff1 : Debuff
    {
        public StaminaDebuff1()
        {
            name = "Stamina Debuff Rank 1";
            description = "Decreases Stamina by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.stamina -= 1;
        }
    }

    public class StrengthDebuff1 : Debuff
    {
        public StrengthDebuff1()
        {
            name = "Strength Debuff Rank 1";
            description = "Decreases Strength by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.strength -= 1;
        }
    }

    public class SpiritDebuff1 : Debuff
    {
        public SpiritDebuff1()
        {
            name = "Spirit Debuff Rank 1";
            description = "Decreases Spirit by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.spirit -= 1;
        }
    }

    public class Corruption1 : Debuff
    {
        public Corruption1()
        {
            name = "Corruption Rank 1";
            description = "Deals 1 damage to the target per tick.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.currentHP -= 1;
            if (target.currentHP < 0)
                target.currentHP = 0;
        }
    }
}
