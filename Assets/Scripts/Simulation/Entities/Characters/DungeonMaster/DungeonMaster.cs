namespace Undermarch.Simulation.Entities.Characters.DungeonMaster
{
    public class DungeonMaster : Character
    {
        public override Character Clone()
        {
            return (DungeonMaster)this.MemberwiseClone();
        }
    }
}
