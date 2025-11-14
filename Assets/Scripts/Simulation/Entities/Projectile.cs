using System;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Core;

namespace Undermarch.Simulation.Entities
{
    public class Projectile : IProjectile, IEntity
    {
        public TilePos Position { get; set; }
        public string Name { get; private set; }
        public TilePos Direction { get; private set; }
        public int Speed { get; private set; }
        public Faction Faction { get; private set; }
        public DamagePacket Damage { get; private set; }
        public bool IsActive { get; private set; }

        public int MaxRange { get; private set; }
        private int traveledDistance;

        public Projectile(string name, TilePos position, TilePos direction, int speed, int maxRange, Faction faction, DamagePacket damage)
        {
            Name = name;
            Position = position;
            Direction = direction;
            Speed = speed;
            MaxRange = maxRange;
            Faction = faction;
            Damage = damage;
            IsActive = true;
            traveledDistance = 0;
        }

        public void Tick(IBoard board)
        {
            if (!IsActive) return;

            // Move Speed tiles in Direction
            for (int step = 0; step < Speed && IsActive; step++)
            {
                TilePos nextPos = new TilePos(Position.x + Direction.x, Position.y + Direction.y);

                // Check bounds
                if (!board.InBounds(nextPos))
                {
                    Despawn(board);
                    return;
                }

                // Check wall
                if (board.HasWallAt(nextPos))
                {
                    Despawn(board);
                    return;
                }

                // Check entity hit
                Character hit = board.GetEntityAt(nextPos);
                if (hit != null && IsEnemy(hit))
                {
                    DealDamage(hit);
                    Despawn(board);
                    return;
                }

                // Move projectile
                object currentProjectile = board.GetInteractableAt(Position);
                if (currentProjectile == this)
                {
                    board.RemoveInteractable(Position);
                }

                Position = nextPos;
                board.AddInteractable(Position, this);
                traveledDistance++;

                // Check max range
                if (traveledDistance >= MaxRange)
                {
                    Despawn(board);
                    return;
                }
            }
        }

        private bool IsEnemy(Character target)
        {
            // Projectiles hit opposite faction
            if (Faction == Combat.Faction.ProjectileDefender)
            {
                return target.faction == Combat.Faction.Hero;
            }
            else if (Faction == Combat.Faction.ProjectileHero)
            {
                return target.faction == Combat.Faction.Defender || target.faction == Combat.Faction.Neutral;
            }

            return false;
        }

        private void DealDamage(Character target)
        {
            SimulationLog.Log($"{Name} hit {target.Name} for {Damage.TotalDamage()} damage!");
            target.TakeDamage(Damage);
        }

        private void Despawn(IBoard board)
        {
            IsActive = false;
            object currentProjectile = board.GetInteractableAt(Position);
            if (currentProjectile == this)
            {
                board.RemoveInteractable(Position);
            }
        }
    }
}
