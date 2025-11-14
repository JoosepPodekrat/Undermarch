using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Pathfinding;

namespace Undermarch.Simulation.Entities.Characters.Heroes
{
    public class Hero : Character
    {
        public int FleeThreshold = 20; // gold / healthPercent > threshold triggers flee
        public int CombatRange = 5; // Distance within which to prioritize combat

        public override void Act(Board board)
        {
            TilePos currentPos = board.GetPositionOf(this);

            // Priority 1: Combat if enemy nearby
            Character nearbyEnemy = FindNearbyEnemy(board, currentPos);
            if (nearbyEnemy != null)
            {
                TilePos enemyPos = board.GetPositionOf(nearbyEnemy);

                // If adjacent, attack
                if (TilePos.ManhattanDistance(currentPos, enemyPos) == 1)
                {
                    Attack(nearbyEnemy);
                    return;
                }

                // Move toward enemy
                var path = Pathfinder.FindPath(board, currentPos, enemyPos);
                if (path != null && path.Count > 1)
                {
                    HandleMove(board, currentPos, path[1]);
                }
                return;
            }

            // Check flee conditions
            if (ShouldFlee())
            {
                FleeToExit(board, currentPos);
                return;
            }

            // Priority 2: Loot chests
            Chest nearestChest = FindNearestUnlootedChest(board);
            if (nearestChest != null)
            {
                MoveTowardAndLoot(board, currentPos, nearestChest);
                return;
            }

            // Priority 3: Attack Dungeon Master
            AttackDungeonMaster(board, currentPos);
        }

        private Character FindNearbyEnemy(Board board, TilePos currentPos)
        {
            Character closest = board.FindClosestTarget(this, Faction.Defender);
            if (closest != null)
            {
                TilePos enemyPos = board.GetPositionOf(closest);
                int distance = TilePos.ManhattanDistance(currentPos, enemyPos);
                if (distance <= CombatRange)
                {
                    return closest;
                }
            }
            return null;
        }

        private bool ShouldFlee()
        {
            float healthPercent = (float)currentHP / maxHP;
            if (healthPercent <= 0) healthPercent = 0.01f; // Avoid division by zero

            float lootToHealthRatio = gold / healthPercent;
            return lootToHealthRatio > FleeThreshold;
        }

        private void FleeToExit(Board board, TilePos currentPos)
        {
            // Head to nearest board edge
            TilePos exitDirection = GetDirectionToNearestEdge(board, currentPos);
            TilePos nextPos = new TilePos(currentPos.x + exitDirection.x, currentPos.y + exitDirection.y);

            if (board.InBounds(nextPos))
            {
                HandleMove(board, currentPos, nextPos);
            }

            // Check if reached edge - remove from board (escaped)
            if (currentPos.x == 0 || currentPos.x == board.Width - 1 ||
                currentPos.y == 0 || currentPos.y == board.Height - 1)
            {
                board.RemoveEntity(currentPos);
                SimulationLog.Log($"{Name} escaped with {gold} gold!");
            }
        }

        private TilePos GetDirectionToNearestEdge(Board board, TilePos pos)
        {
            int distToLeft = pos.x;
            int distToRight = board.Width - 1 - pos.x;
            int distToTop = pos.y;
            int distToBottom = board.Height - 1 - pos.y;

            int minDist = System.Math.Min(System.Math.Min(distToLeft, distToRight),
                                         System.Math.Min(distToTop, distToBottom));

            if (minDist == distToLeft) return new TilePos(-1, 0);
            if (minDist == distToRight) return new TilePos(1, 0);
            if (minDist == distToTop) return new TilePos(0, -1);
            return new TilePos(0, 1);
        }

        private Chest FindNearestUnlootedChest(Board board)
        {
            TilePos myPos = board.GetPositionOf(this);
            Chest nearestChest = null;
            float minDistanceSq = float.MaxValue;

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TilePos pos = new TilePos(x, y);
                    object interactable = board.GetInteractableAt(pos);
                    if (interactable is Chest chest && !chest.Looted)
                    {
                        float distSq = TilePos.DistanceSq(myPos, pos);
                        if (distSq < minDistanceSq)
                        {
                            minDistanceSq = distSq;
                            nearestChest = chest;
                        }
                    }
                }
            }

            return nearestChest;
        }

        private void MoveTowardAndLoot(Board board, TilePos currentPos, Chest chest)
        {
            TilePos chestPos = chest.Position;

            // If on chest, loot it
            if (currentPos.Equals(chestPos))
            {
                chest.Interact(this);
                return;
            }

            // Move toward chest
            var path = Pathfinder.FindPath(board, currentPos, chestPos);
            if (path != null && path.Count > 1)
            {
                HandleMove(board, currentPos, path[1]);
            }
        }

        private void AttackDungeonMaster(Board board, TilePos currentPos)
        {
            Character dungeonMaster = null;
            foreach (var character in board.GetAllCharacters())
            {
                if (character is DungeonMaster.DungeonMaster)
                {
                    dungeonMaster = character;
                    break;
                }
            }

            if (dungeonMaster == null) return;

            TilePos dmPos = board.GetPositionOf(dungeonMaster);

            // If adjacent, attack
            if (TilePos.ManhattanDistance(currentPos, dmPos) == 1)
            {
                Attack(dungeonMaster);
                return;
            }

            // Move toward DM
            var path = Pathfinder.FindPath(board, currentPos, dmPos);
            if (path != null && path.Count > 1)
            {
                HandleMove(board, currentPos, path[1]);
            }
        }
    }
}
