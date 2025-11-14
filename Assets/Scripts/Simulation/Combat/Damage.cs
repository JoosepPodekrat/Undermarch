using System.Collections.Generic;

namespace Undermarch.Simulation.Combat
{
    public enum DamageType { Physical, Fire, Frost, Poison, Arcane, Light, Dark, Bleed }

    public sealed class DamagePacket
    {
        public readonly Dictionary<DamageType, int> amounts = new();
        public void Add(DamageType type, int amount) => amounts[type] = (amounts.TryGetValue(type, out var v) ? v : 0) + amount;

        public int TotalDamage()
        {
            int total = 0;
            foreach (var amount in amounts.Values)
            {
                total += amount;
            }
            return total;
        }
    }

    public interface IEffect { int RemainingTicks { get; } void OnApply(); void OnTick(); void OnExpire(); }
    public enum Faction { Hero, Defender, Neutral, ProjectileHero, ProjectileDefender }

    public static class InteractionMatrix
    {
        public static bool TrapTriggersOn(Faction trapOwner, Faction target)
        {
            // Example: a defender trap triggers on heroes only
            if (trapOwner == Faction.Defender && target == Faction.Hero) return true;
            if (trapOwner == Faction.Hero && target == Faction.Defender) return true; //support for heroes laying traps, implement later if at all
            return false;
        }
        public static bool ProjectileHits(Faction projectile, Faction target)
        {
            if (projectile == Faction.ProjectileHero && target == Faction.Hero) return false;
            if (projectile == Faction.ProjectileDefender && target == Faction.Defender) return false; // no friendly fire for either faction with projectiles
            return true;
        }
    }
}