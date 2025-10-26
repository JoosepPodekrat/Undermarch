using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undermarch.Assets.Scripts.Simulation.Entities
{
    internal class Player
    {
        public string name { get; set; }
        public int score { get; set; }
        
        public int stage { get; set; }

        public int goblinKnowledge { get; set; }
        public int skeletonKnowledge { get; set; }
        public int ghoulKnowledge { get; set; }

        public int werebeastKnowledge { get; set; }
        public int ghostKnowledge { get; set; }
        public int plantKnowledge { get; set; }
        public int slimeKnowledge { get; set; }

        
        public Player() { }

    }
}
