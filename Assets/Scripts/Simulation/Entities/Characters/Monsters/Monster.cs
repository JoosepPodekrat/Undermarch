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

            // If we are on the target, do nothing.
            if (moveX == 0 && moveY == 0) return;

            // Try to move horizontally first
            if (moveX != 0)
            {
                var nextPos = new TilePos(currentPos.x + moveX, currentPos.y);
                if (HandleMove(board, currentPos, nextPos))
                {
                    return; // Action was taken (move or attack)
                }
            }

            // If horizontal move was blocked or not possible, try vertical
            if (moveY != 0)
            {
                var nextPos = new TilePos(currentPos.x, currentPos.y + moveY);
                if (HandleMove(board, currentPos, nextPos))
                {
                    return; // Action was taken (move or attack)
                }
            }
        }
    }
}
