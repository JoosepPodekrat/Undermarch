using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Pathfinding;

namespace Undermarch.Simulation.Entities.Characters.Monsters
{
    public class Monster : Character
    {
        public override void Act(Board board)
        {
            if (this.Name == "Slime Monster")
            {
                float damage = (charWeapon != null ? charWeapon.damage : 0) + effectiveStrength;
                SimulationLog.Log($"Slime Monster Turn: HP={currentHP}/{maxHP}, Damage={damage}");
            }

            // Find the closest hero
            Character target = board.FindClosestTarget(this, Faction.Hero);
            if (target == null) return; // No heroes left

            TilePos currentPos = board.GetPositionOf(this);
            TilePos targetPos = board.GetPositionOf(target);

            // If we are adjacent to the target, attack instead of moving.
            if (TilePos.DistanceSq(currentPos, targetPos) <= 2)
            {
                Attack(target);
                return;
            }

            var path = Pathfinder.FindPath(board, currentPos, targetPos);

            if (path != null && path.Count > 0)
            {
                var nextPos = path[0];
                HandleMove(board, currentPos, nextPos);
            }
        }
    }
}
