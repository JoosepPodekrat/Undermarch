using System;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Pathfinding;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Entities.Characters.Monsters
{
    public class ArcherMonster : Monster
    {
        public int AttackRange = 5;
        public int ShootCooldown = 0;
        public int CooldownTicks = 2;
        public int ArrowSpeed = 3;
        public int ArrowRange = 10;

        public override void Act(IBoard board)
        {
            if (ShootCooldown > 0)
            {
                ShootCooldown--;
            }

            TilePos myPos = board.GetPositionOf(this);
            if (!myPos.IsValid())
            {
                return;
            }

            // Find closest hero
            Character target = board.FindClosestTarget(this, Faction.Hero);

            if (target == null)
            {
                return;
            }

            TilePos targetPos = board.GetPositionOf(target);
            if (!targetPos.IsValid())
            {
                return;
            }

            int distance = TilePos.ManhattanDistance(myPos, targetPos);

            // If in range and cooldown is ready, shoot
            if (distance <= AttackRange && ShootCooldown == 0)
            {
                ShootArrow(board, targetPos);
                ShootCooldown = CooldownTicks;
            }
            else
            {
                // Move toward target if out of range or on cooldown
                var path = Pathfinder.FindPath(board, myPos, targetPos);
                if (path != null && path.Count > 1)
                {
                    TilePos nextStep = path[1];
                    bool moved = HandleMove(board, myPos, nextStep);

                    if (!moved && path.Count > 2)
                    {
                        nextStep = path[2];
                        HandleMove(board, myPos, nextStep);
                    }
                }
            }
        }

        private void ShootArrow(IBoard board, TilePos targetPos)
        {
            TilePos myPos = board.GetPositionOf(this);

            // Calculate direction (normalized to cardinal/diagonal)
            TilePos direction = GetDirection(myPos, targetPos);

            // Create arrow damage
            DamagePacket arrowDamage = new DamagePacket();
            arrowDamage.Add(DamageType.Physical, 5 + effectiveAgility / 2);

            // Create projectile
            Projectile arrow = new Projectile(
                name: "Arrow",
                position: myPos,
                direction: direction,
                speed: ArrowSpeed,
                maxRange: ArrowRange,
                faction: Faction.ProjectileDefender,
                damage: arrowDamage
            );

            board.AddInteractable(myPos, arrow);
            SimulationLog.Log($"{Name} shoots an arrow!");
        }

        private TilePos GetDirection(TilePos from, TilePos to)
        {
            int dx = to.x - from.x;
            int dy = to.y - from.y;

            // Normalize to -1, 0, or 1
            int dirX = dx == 0 ? 0 : (dx > 0 ? 1 : -1);
            int dirY = dy == 0 ? 0 : (dy > 0 ? 1 : -1);

            return new TilePos(dirX, dirY);
        }
    }
}
