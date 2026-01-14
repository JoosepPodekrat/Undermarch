using System;
using System.Collections.Generic;
using System.Linq;
using Undermarch;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Effects.Buffs;
using Undermarch.Simulation.Effects.Debuffs;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;


namespace Undermarch.Simulation.Entities
{
    public class Character
    {
        bool isDead = false;
        bool isScared = false;
        public string Name = "Character";
        public int gold = 0; // Gold carried by this character
        public string history;
        // base stats, i think these should default to 10, so every point increase is roughly a 10% increase in effectiveness of the stat.
        public Faction faction; // { Hero, Defender, Neutral, ProjectileHero, ProjectileDefender }
        public int agility = 10; // Influences speed, increases armor, increases crit 
        public int intelligence = 10; // Increases maximum mana, casting modifiers
        public int stamina = 10; // hp and health regen
        public int strength = 10; // melee damage
        public int spirit = 10; //  mana regeneration and morale

        // effective stats

        public int effectiveAgility;
        public int effectiveIntelligence;
        public int effectiveStamina;
        public int effectiveStrength;
        public int effectiveSpirit;


        public int maxHP = 1; // some k * stamine
        public int currentHP = 1;
        public float maxHPModifier = 1;
        private const int StaminaToHealthMultiplier = 5;
        // regen


        public int maxMorale = 1; // if hero and 0, leaves
        public int currentMorale = 1;
        public float maxMoraleModifier = 1;

        public int maxMana = 0;
        public int currentMana = 0;
        public float maxManaModifier = 1;

        public int healthRegen = 0;
        public int manaRegen = 0;

        //defensives 
        public int magicresist = 0; //flat damage reduction for magic
        public float magicDamageReduction = 1;
        public int armor = 0; // flat damage reduction
        public float damageReduction = 1; //percentage damage reduction , dmg taken = (X / damagereduction ) - armor, 

        public float damageModifier = 1; // default 1.
        //equipment - calling equipment.equip replaces the slot with new item
        public Weapon charWeapon;
        public Armor charArmor;
        public Helmet charHelmet;
        public Accessory charAccessory;
        // buffs
        public List<Buff> buffs = new();

        //debuffs
        public List<Debuff> debuffs = new();

        public string spawnSound;
        public string hurtSound;
        public string attackSound;
        public string deathSound;

        public void TickBuffsAndDebuffs()
        {
            // Tick and apply each buff
            foreach (Buff buff in buffs.ToList())
            {
                buff.Apply(this); // Per-tick effects like damage-over-time
                buff.Tick();
            }

            // Tick and apply each debuff
            foreach (Debuff debuff in debuffs.ToList())
            {
                debuff.Apply(this); // Per-tick effects like damage-over-time
                debuff.Tick();
            }

            // Remove expired effects
            buffs.RemoveAll(b => b.IsExpired());
            debuffs.RemoveAll(d => d.IsExpired());

            // Recalculate stats after effects change
            GetGearEffect();
        }
        public int GetStatModifier(StatType statType)
        {
            int modifier = 0;

            foreach (Buff buff in buffs)
            {
                if (buff.statModifiers.TryGetValue(statType, out int buffMod))
                {
                    modifier += buffMod;
                }
            }

            foreach (Debuff debuff in debuffs)
            {
                if (debuff.statModifiers.TryGetValue(statType, out int debuffMod))
                {
                    modifier += debuffMod;
                }
            }

            return modifier;
        }

        public void GetGearEffect()
        {
            List<IEquipment> equipment = new() { charWeapon, charArmor, charHelmet, charAccessory };

            effectiveAgility = agility + equipment.Sum(e => e.agility) + GetStatModifier(StatType.Agility);
            effectiveIntelligence = intelligence + equipment.Sum(e => e.intelligence) + GetStatModifier(StatType.Intelligence);
            effectiveStamina = stamina + equipment.Sum(e => e.stamina) + GetStatModifier(StatType.Stamina);
            effectiveStrength = strength + equipment.Sum(e => e.strength) + GetStatModifier(StatType.Strength);
            effectiveSpirit = spirit + equipment.Sum(e => e.spirit) + GetStatModifier(StatType.Spirit);

            armor = effectiveAgility + GetStatModifier(StatType.Armor);
            magicresist = GetStatModifier(StatType.MagicResist);
        }
        public void InitStats() // call after calculate stats when initating character
        {
            this.maxHP = (int) Math.Round(effectiveStamina * maxHPModifier * StaminaToHealthMultiplier);
            this.maxMana = (int) Math.Round(effectiveIntelligence * maxManaModifier);
            this.maxMorale = (int) Math.Round(effectiveSpirit * maxMoraleModifier);
            


        }
        // debuffs[x].apply(); should be called for each debuff
        // actions that interact with anything else

        public void CalculateStats()
        {
            GetGearEffect();
        }
        public string PrintStats()
        {
            string answer =
                $"=== Character Stats ===\n" +
                $"Effective Agility: {effectiveAgility}\n" +
                $"Effective Intelligence: {effectiveIntelligence}\n" +
                $"Effective Stamina: {effectiveStamina}\n" +
                $"Effective Strength: {effectiveStrength}\n" +
                $"Effective Spirit: {effectiveSpirit}\n\n" +

                $"Max HP: {maxHP}\n" +
                $"Current HP: {currentHP}\n" +
                $"Max Mana: {maxMana}\n" +
                $"Current Mana: {currentMana}\n" +
                $"Max Morale: {maxMorale}\n" +
                $"Current Morale: {currentMorale}\n\n" +

                $"Health Regen: {healthRegen}\n" +
                $"Mana Regen: {manaRegen}\n" +
                $"Armor: {armor}\n" +
                $"Damage Reduction: {damageReduction:F2}\n" +
                $"Damage Modifier: {damageModifier:F2}";

            return answer;
        }

        public virtual Character Clone()
        {
            // Create a shallow copy (base stats and primitive fields)
            Character copy = (Character)this.MemberwiseClone();

            // Create new lists so buffs/debuffs aren�t shared between instances
            copy.buffs = new List<Buff>();
            copy.debuffs = new List<Debuff>();

            // Copy equipment references (these can stay shared unless they change dynamically)
            copy.charWeapon = this.charWeapon;
            copy.charArmor = this.charArmor;
            copy.charHelmet = this.charHelmet;
            copy.charAccessory = this.charAccessory;

            // set up stats
            copy.CalculateStats();
            copy.InitStats();

            // Reset runtime stats
            copy.currentHP = copy.maxHP;
            copy.currentMana = copy.maxMana;
            copy.currentMorale = copy.maxMorale;

            return copy;
        }

        public bool IsDead => isDead;

        public virtual void Act(IBoard board)
        {
            // Base character does nothing.
        }

        public void Attack(Character target)
        {
            CharacterEvents.RaiseAttack(this);

            float damage = charWeapon.damage;
            if (charWeapon.damageType == DamageType.Physical)
                damage += effectiveStrength;
            else if (charWeapon.damageType == DamageType.Arcane || charWeapon.damageType == DamageType.Frost || charWeapon.damageType == DamageType.Fire)
                damage += effectiveIntelligence;
            else if (charWeapon.damageType == DamageType.Dark || charWeapon.damageType == DamageType.Light)
                damage += effectiveSpirit;
            damage *= damageModifier;

            DamageType weaponDamageType = charWeapon.damageType;
            int cleanDamage = (int) Math.Round(damage);

            DamagePacket damagePacket = new DamagePacket();
            damagePacket.Add(weaponDamageType, cleanDamage);
            target.TakeDamage(damagePacket);
        }

        public void TakeDamage(DamagePacket damagePacket)
{
    int totalDamageTaken = 0;

    foreach (var (type, rawAmount) in damagePacket.amounts)
    {
        float reduced = 0;
        if (type == DamageType.Physical) { reduced = rawAmount / damageReduction - armor; }
        else if (type == DamageType.Bleed) { reduced = rawAmount / damageReduction; }
        else { reduced = rawAmount / magicDamageReduction - magicresist; }

        if (reduced < 0) reduced = 0;
        totalDamageTaken += (int)Math.Round(reduced);
    }

    currentHP -= totalDamageTaken;

    // Raise audio/event
    CharacterEvents.RaiseHurt(this);

    if (currentHP <= 0)
    {
        currentHP = 0;
        isDead = true;
        CharacterEvents.RaiseDeath(this);
    }
}


        private void ApplyTileEffects(IBoard board, TilePos pos)
        {
            // Check for tile effects at position
            // Note: Board stores effects in a List at each position, but for now we'll check interactables
            var interactable = board.GetInteractableAt(pos);
            if (interactable is TileEffect tileEffect)
            {
                tileEffect.ApplyTo(this);
            }
        }

        private void TriggerTraps(IBoard board, TilePos pos)
        {
            var interactable = board.GetInteractableAt(pos);
            if (interactable is Trap trap)
            {
                // Only heroes can trigger traps for now.
                if (this.faction == Faction.Hero)
                {
                    SimulationLog.Log($"{this.Name} triggered a {trap.Name} at ({pos.x}, {pos.y})!");
                    this.TakeDamage(trap.DamagePacket);
                    trap.Durability--;

                    if (trap.Durability <= 0)
                    {
                        board.RemoveInteractable(pos);
                        SimulationLog.Log($"{trap.Name} was destroyed.");
                    }
                }
            }
        }
        public bool HandleMove(IBoard board, TilePos currentPos, TilePos nextPos)
        {
            if (!board.InBounds(nextPos) || board.HasWallAt(nextPos))
            {
                return false; // Blocked by wall or out of bounds
            }

            var occupant = board.GetEntityAt(nextPos);
            if (occupant != null)
            {
                // If the occupant is an enemy, attack it
                if (occupant.faction != this.faction)
                {
                    Attack(occupant);
                    return true; // Action taken: Attack
                }
                else
                {
                    return false; // Blocked by an ally
                }
            }
            else
            {
                // The tile is empty, so move
                board.MoveEntity(currentPos, nextPos);
                TriggerTraps(board, nextPos); // Check for traps on the new tile
                ApplyTileEffects(board, nextPos); // Check for tile effects
                return true; // Action taken: Move
            }
        }
    }
}
