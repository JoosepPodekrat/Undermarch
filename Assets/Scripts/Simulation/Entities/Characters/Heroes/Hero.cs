using Undermarch.Simulation.Combat;
using System.Collections.Generic;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Pathfinding;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Core;
using ResourceType = Undermarch.Simulation.Interfaces.ResourceType;

namespace Undermarch.Simulation.Entities.Characters.Heroes
{
    public enum HeroState
    {
        Idle,
        MovingToChest,
        LootingChest,
        MovingToDungeonMaster,
        Fighting
    }


    public class Hero : Character
    {
        private HeroState state = HeroState.Idle;
        private TilePos? currentTarget = null;
        private List<TilePos> currentPath = null;


        public int FleeThreshold = 20; // gold / healthPercent > threshold triggers flee
        public int CombatRange = 5; // Distance within which to prioritize combat
        public Dictionary<ResourceType, int> ResourcesGiven { get; set; } = new();

        public override void Act(IBoard board)
        {
            TilePos currentPos = board.GetPositionOf(this);

            // 1. Combat interrupt (always highest priority)
            Character nearbyEnemy = FindNearbyEnemy(board, currentPos);
            if (nearbyEnemy != null)
            {
                state = HeroState.Fighting;

                TilePos enemyPos = board.GetPositionOf(nearbyEnemy);

                if (TilePos.ManhattanDistance(currentPos, enemyPos) == 1)
                {
                    Attack(nearbyEnemy);
                    return;
                }

                var path = Pathfinder.FindPath(board, currentPos, enemyPos);
                if (path != null && path.Count > 1)
                    HandleMove(board, currentPos, path[1]);

                return;
            }

            // If we were fighting but combat is over, resume previous task
            if (state == HeroState.Fighting)
            {
                state = HeroState.Idle;
                currentPath = null;
                currentTarget = null;
            }

            // 2. Flee if needed
            if (ShouldFlee())
            {
                FleeToExit(board, currentPos);
                return;
            }

            // 3. Handle chest or dungeon master depending on state
            switch (state)
            {
                case HeroState.Idle:
                    SelectNewTask(board, currentPos);
                    break;

                case HeroState.MovingToChest:
                    ContinueChestMovement(board, currentPos);
                    break;

                case HeroState.MovingToDungeonMaster:
                    ContinueDungeonMasterMovement(board, currentPos);
                    break;
            }
        }
        private void SelectNewTask(IBoard board, TilePos currentPos)
        {
            Chest chest = FindNearestUnlootedChest(board);
            if (chest != null)
            {
                state = HeroState.MovingToChest;
                currentTarget = chest.Position;
                currentPath = Pathfinder.FindPath(board, currentPos, chest.Position);
                return;
            }

            // No chests? Go DM.
            Character dm = FindDungeonMaster(board);
            if (dm != null)
            {
                state = HeroState.MovingToDungeonMaster;
                currentTarget = board.GetPositionOf(dm);
                currentPath = Pathfinder.FindPath(board, currentPos, (TilePos)currentTarget);
            }
        }
        private void ContinueChestMovement(IBoard board, TilePos currentPos)
        {
            if (currentTarget == null)
            {
                state = HeroState.Idle;
                return;
            }

            // Already at chest?
            // Already at chest?
            if (currentPos.Equals(currentTarget.Value))
            {
                Chest chest = board.GetInteractableAt(currentTarget.Value) as Chest;
                if (chest != null && !chest.Looted)
                    chest.Interact(this);

                // Reset current task
                currentPath = null;
                currentTarget = null;

                // Immediately select a new task (DON'T stop)
                state = HeroState.Idle;
                SelectNewTask(board, currentPos);
                return;
            }


            // Need new path?
            if (currentPath == null || currentPath.Count < 2)
            {
                currentPath = Pathfinder.FindPath(board, currentPos, currentTarget.Value);
                if (currentPath == null)
                {
                    state = HeroState.Idle;
                    return;
                }
            }

            HandleMove(board, currentPos, currentPath[1]);
            currentPath.RemoveAt(0);
        }
        private DungeonMaster.DungeonMaster FindDungeonMaster(IBoard board)
        {
            foreach (var character in board.GetAllCharacters())
            {
                if (character is DungeonMaster.DungeonMaster dm)
                    return dm;
            }
            return null;
        }

        private void ContinueDungeonMasterMovement(IBoard board, TilePos currentPos)
        {
            if (currentTarget == null)
            {
                state = HeroState.Idle;
                return;
            }

            if (currentPos.Equals(currentTarget.Value))
            {
                state = HeroState.Idle;
                return;
            }

            if (currentPath == null || currentPath.Count < 2)
                currentPath = Pathfinder.FindPath(board, currentPos, currentTarget.Value);

            if (currentPath == null)
            {
                state = HeroState.Idle;
                return;
            }

            HandleMove(board, currentPos, currentPath[1]);
            currentPath.RemoveAt(0);
        }




        private bool ShouldFlee()
        {
            if (FleeThreshold == int.MaxValue) return false; // never flee for final wave

            float healthPercent = (float)currentHP / maxHP;
            if (gold / healthPercent > FleeThreshold)
            {
                return true;
            }
            return healthPercent < 0.2f; // e.g., flee if below 20% HP  

        }



        private Character FindNearbyEnemy(IBoard board, TilePos currentPos)
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


        private void FleeToExit(IBoard board, TilePos currentPos)
        {
            TilePos exitPos = FindExit(board);

            // Already at the exit? Remove hero from board
            if (currentPos.Equals(exitPos))
            {
                board.RemoveEntity(currentPos);
                SimulationLog.Log($"{Name} escaped with {gold} gold!");
                return;
            }

            // Pathfind toward exit
            var path = Pathfinder.FindPath(board, currentPos, exitPos);
            if (path == null || path.Count == 0)
            {
                // blocked or no path, will try again next tick
                return;
            }

            // Move up to 2 steps along the path
            int steps = System.Math.Min(2, path.Count);
            for (int i = 0; i < steps; i++)
            {
                HandleMove(board, currentPos, path[i]);
                currentPos = path[i]; // update currentPos for next step
                                      // Check if reached exit
                if (currentPos.Equals(exitPos))
                {
                    board.RemoveEntity(currentPos);
                    SimulationLog.Log($"{Name} escaped with {gold} gold!");
                    return;
                }
            }
        }




        private Chest FindNearestUnlootedChest(IBoard board)
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
                    if (interactable is Chest chest && !chest.Looted && chest.IsActive)
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

        private void MoveTowardAndLoot(IBoard board, TilePos currentPos, Chest chest)
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

        private void AttackDungeonMaster(IBoard board, TilePos currentPos)
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
        private TilePos FindExit(IBoard board)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var pos = new TilePos(x, y);
                    if (board.GetInteractableAt(pos) is Door door && door.IsExit)
                    {
                        return pos;
                    }
                }
            }

            // fallback if no exit found
            return new TilePos(0, 0);
        }


    }
}
