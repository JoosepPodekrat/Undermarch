using Codice.Client.Common;
using System.Collections.Generic;

namespace Undermarch
{
    public class Character
    {
        // base stats, i think these should default to 10, so every point increase is roughly a 10% increase in effectiveness of the stat.
        public int agility; // Influences speed, increases armor, increases crit 
        public int intelligence; // Increases maximum mana, casting modifiers
        public int stamina; // hp and so on
        public int strength; // melee damage
        public int spirit; // regeneration 

        // effective stats
        public int maxHP; // some k * stamine
        public int currentHP; // if 0, dies

        public int maxMorale; // if hero and 0, leaves
        public int currentMorale;

        public int maxMana;
        public int currentMana;

        public int healthRegen;
        public int manaRegen;

        public int armor; // flat damage reduction
        public int damageReduction; //percentage damage reduction , dmg taken = (X / damagereduction ) - armor, 
        //equipment
        // public Weapon charWeapon;
        // public Armor charArmor;
        // public Helmet charHelmet;
        // public Accessory charAccessory;
        //buffs
        //public List<Buff> buffs;
        // buffs[x].apply(); should be called for each buff
        //debuffs
        // debuffs[x].apply(); should be called for each debuff
        // actions that interact with anything else

        // 

    }
}
