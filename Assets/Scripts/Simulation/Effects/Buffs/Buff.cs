namespace Undermarch
{
    public class Buff : ICharacterEffect
    {
        public string name { get; set; }
        public string description { get; set; }
        public int duration { get; set; }

        public void Add(Character target)
        {
            target.buffs.Add(this);
        }

        public void Remove(Character target)
        {
            target.buffs.Remove(this);
        }

        // Allow subclasses to define custom behavior
        public virtual void Apply(Character target) { }
    }

    public class AgilityBuff1 : Buff
    {
        public AgilityBuff1()
        {
            name = "Agility Buff Rank 1";
            description = "Increases Agility by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.agility += 1;
        }
    }

    public class IntelligenceBuff1 : Buff
    {
        public IntelligenceBuff1()
        {
            name = "Intelligence Buff Rank 1";
            description = "Increases Intelligence by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.intelligence += 1;
        }
    }

    public class StaminaBuff1 : Buff
    {
        public StaminaBuff1()
        {
            name = "Stamina Buff Rank 1";
            description = "Increases Stamina by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.stamina += 1;
        }
    }

    public class StrengthBuff1 : Buff
    {
        public StrengthBuff1()
        {
            name = "Strength Buff Rank 1";
            description = "Increases Strength by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.strength += 1;
        }
    }

    public class SpiritBuff1 : Buff
    {
        public SpiritBuff1()
        {
            name = "Spirit Buff Rank 1";
            description = "Increases Spirit by 1.";
            duration = 30;
        }

        public override void Apply(Character target)
        {
            target.spirit += 1;
        }
    }
    public class Renew1 : Buff
    {
        public Renew1()
        {
            name = "Renew Rank 1";
            description = "Heals the target for 1 hp per tick.";
            duration = 30;
        }
        public override void Apply(Character target)
        {
            target.currentHP += 1;
        }
    }
}
