using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Pathfinding;

namespace Undermarch.Simulation.Entities.Characters.Heroes
{
    public class Hero : Character
    {
                        public override void Act(Board board)
                        {
                            if (this.Name == "Peasant")
                            {
                                float damage = (charWeapon != null ? charWeapon.damage : 0) + effectiveStrength;
                                SimulationLog.Log($"Peasant Turn: HP={currentHP}/{maxHP}, Damage={damage}");
                            }
                
                            // Target monsters first
                            Character target = board.FindClosestTarget(this, Faction.Defender);
        
                    // If no monsters, target the Dungeon Master
                    if (target == null)
                    { 
                        // This is not efficient, but for now it's fine.
                        foreach (var character in board.GetAllCharacters())
                        {
                            if (character is DungeonMaster.DungeonMaster)
                            {
                                target = character;
                                break;
                            }
                        }
                    }
        
                    if (target == null) return; // No targets left
        
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
                }    }
}
