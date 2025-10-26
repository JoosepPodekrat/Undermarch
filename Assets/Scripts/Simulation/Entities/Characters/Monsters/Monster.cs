using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Entities.Characters.Monsters
{
    public class Monster : Character
    {
        public override void Act(Board board)
        {
            // Find the closest hero
            Character target = board.FindClosestTarget(this, Faction.Hero);
            if (target == null) return; // No heroes left

            // Get positions
            TilePos currentPos = board.GetPositionOf(this);
            TilePos targetPos = board.GetPositionOf(target);

            // Determine direction
            int dx = targetPos.x - currentPos.x;
            int dy = targetPos.y - currentPos.y;

            // Move one step in the general direction
            int moveX = System.Math.Sign(dx);
            int moveY = System.Math.Sign(dy);

            TilePos nextPos = new TilePos(currentPos.x + moveX, currentPos.y + moveY);

            // Basic movement: prefer horizontal, then vertical. No diagonal moves yet.
            // Also, no collision detection yet.
            if (moveX != 0)
            {
                nextPos = new TilePos(currentPos.x + moveX, currentPos.y);
            }
            else if (moveY != 0)
            {
                nextPos = new TilePos(currentPos.x, currentPos.y + moveY);
            }
            else
            {
                // We are on the target, do nothing for now
                return;
            }

            // For now, just move. We will add collision detection later.
            board.MoveEntity(currentPos, nextPos);
        }
    }
}
