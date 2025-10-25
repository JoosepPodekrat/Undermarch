using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Undermarch.Simulation.Combat;
namespace Undermarch
{
    public class Character
    {

        // base stats, i think these should default to 10, so every point increase is roughly a 10% increase in effectiveness of the stat.
        public Faction faction; // { Hero, Defender, Neutral, ProjectileHero, ProjectileDefender }
        public int agility; // Influences speed, increases armor, increases crit 
        public int intelligence; // Increases maximum mana, casting modifiers
        public int stamina; // hp and health regen
        public int strength; // melee damage
        public int spirit; //  mana regeneration and morale

        // effective stats
        public int maxHP = 1; // some k * stamine
        public int currentHP = 1; // if 0, dies



        public int maxMorale = 1; // if hero and 0, leaves
        public int currentMorale = 1;

        public int maxMana = 0;
        public int currentMana = 0;

        public int healthRegen = 0;
        public int manaRegen = 0;

        public int armor = 0; // flat damage reduction
        public int damageReduction = 1; //percentage damage reduction , dmg taken = (X / damagereduction ) - armor, 

        public int damageModifier = 1; // default 1.
        //equipment
        public IWeapon charWeapon;
        public IArmor charArmor;
        public IHelmet charHelmet;
        public IAccessory charAccessory;
        // buffs
        public List<Buff> buffs;
        public void ApplyBuffs()
        {
            foreach (Buff buff in buffs)
            {
                buff.Apply(this);
                buff.duration -= 1;
            }
        }
        //debuffs
        public List<Debuff> debuffs;
        public void ApplyDebuffs()
        {
            foreach (Debuff debuff in debuffs)
            {
                debuff.Apply(this);
                debuff.duration -= 1;
            }
        }
        // debuffs[x].apply(); should be called for each debuff
        // actions that interact with anything else

        public void CalculateStats()
        {
            ApplyBuffs();
            ApplyDebuffs();
        }
        public void Attack(Character target)
        {
            float damage = 0;
            damage += charWeapon.damage;
            DamageType weaponDamageType = charWeapon.damageType;
            damage += strength;
            damage *= damageModifier;
            int cleanDamage = (int) Math.Round(damage);

            DamagePacket damagePacket = new DamagePacket();
            damagePacket.Add(weaponDamageType, cleanDamage);


        }

        // 

    }
}
