using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Undermarch.Simulation.Combat;
namespace Undermarch
{
    public abstract class Character
    {

        // base stats, i think these should default to 10, so every point increase is roughly a 10% increase in effectiveness of the stat.
        public Faction faction; // { Hero, Defender, Neutral, ProjectileHero, ProjectileDefender }
        public int agility; // Influences speed, increases armor, increases crit 
        public int intelligence; // Increases maximum mana, casting modifiers
        public int stamina; // hp and health regen
        public int strength; // melee damage
        public int spirit; //  mana regeneration and morale

        // effective stats

        public int effectiveAgility;
        public int effectiveIntelligence;
        public int effectiveStamina;
        public int effectiveStrength;
        public int effectiveSpirit;


        public int maxHP = 1; // some k * stamine
        public int currentHP = 1; // if 0, dies
        public float maxHPModifier = 1;



        public int maxMorale = 1; // if hero and 0, leaves
        public int currentMorale = 1;

        public int maxMana = 0;
        public int currentMana = 0;
        public float maxManaModifier = 1;

        public int healthRegen = 0;
        public int manaRegen = 0;

        public int armor = 0; // flat damage reduction
        public float damageReduction = 1; //percentage damage reduction , dmg taken = (X / damagereduction ) - armor, 

        public float damageModifier = 1; // default 1.
        //equipment - calling equipment.equip replaces the slot with new item
        public Weapon charWeapon;
        public Armor charArmor;
        public Helmet charHelmet;
        public Accessory charAccessory;
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
        public void GetGearEffect()
        {
            List<IEquipment> equipment = new() { charWeapon, charArmor, charHelmet, charAccessory };

            effectiveAgility = agility + equipment.Sum(e => e.agility);
            effectiveIntelligence = intelligence + equipment.Sum(e => e.intelligence);
            effectiveStamina = stamina + equipment.Sum(e => e.stamina);
            effectiveStrength = strength + equipment.Sum(e => e.strength);
            effectiveSpirit = spirit + equipment.Sum(e => e.spirit);

        }
        public void InitStats() // call after calculate stats
        {
            this.maxHP = (int) Math.Round(effectiveStamina * maxHPModifier);
            this.maxMana = (int) Math.Round(effectiveIntelligence * maxManaModifier);

        }
        // debuffs[x].apply(); should be called for each debuff
        // actions that interact with anything else

        public void CalculateStats()
        {
            GetGearEffect();
            ApplyBuffs();
            ApplyDebuffs();
        }
        public void Attack(Character target)
        {
            float damage = 0;
            damage += charWeapon.damage;
            DamageType weaponDamageType = charWeapon.damageType;
            damage += effectiveStrength;
            damage *= damageModifier;
            int cleanDamage = (int) Math.Round(damage);

            DamagePacket damagePacket = new DamagePacket();
            damagePacket.Add(weaponDamageType, cleanDamage);
            target.TakeDamage(damagePacket);
        }

        public void TakeDamage (DamagePacket damagePacket)
        {
            int totalDamageTaken = 0;

            foreach (var (type, rawAmount) in damagePacket.amounts)
            {
                // Apply percentage-based reduction first
                float reduced = rawAmount / damageReduction;

                // Apply flat armor (can’t reduce below 0)
                reduced -= armor;
                if (reduced < 0)
                    reduced = 0;

                int finalDamage = (int)Math.Round(reduced);
                totalDamageTaken += finalDamage;
            }

            // Apply damage to HP
            currentHP -= totalDamageTaken;

            // Clamp at 0
            if (currentHP <= 0)
                currentHP = 0;
        }
    }
}
