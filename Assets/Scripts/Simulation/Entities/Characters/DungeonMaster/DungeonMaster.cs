using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Entities.Characters.DungeonMaster
{
    public class DungeonMaster : Character
    {
        public override void Act(IBoard board)
        {
            float damage = (charWeapon != null ? charWeapon.damage : 0) + effectiveIntelligence;
            SimulationLog.Log($"Dungeon Master Turn: HP={currentHP}/{maxHP}, Damage={damage}");

            // Find the closest hero
            Character target = board.FindClosestTarget(this, Faction.Hero);
            if (target == null) return; // No heroes left

            TilePos currentPos = board.GetPositionOf(this);
            TilePos targetPos = board.GetPositionOf(target);

            // If we are adjacent to the target, attack
            if (TilePos.DistanceSq(currentPos, targetPos) <= 2)
            {
                Attack(target);
            }
            // Note: The Dungeon Master does not move.
        }
        
        public override Character Clone()
        {
            // We call the base Clone method which correctly calculates stats.
            return base.Clone();
        }
    }
}
