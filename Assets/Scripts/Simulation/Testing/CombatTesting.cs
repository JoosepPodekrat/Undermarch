using System;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities;

namespace Undermarch
{
    public class CombatTesting
    {
        public static void Main(string[] args)
        {
            // Spawn characters (clones of the database templates)
            Character goblin = CharacterDatabase.goblin.Clone();
            Character skeletonMage = CharacterDatabase.skeletonMage.Clone();
            Character rogue = CharacterDatabase.rogue.Clone();
            Character mage = CharacterDatabase.apprenticeMage.Clone();

            // Initialize stats and HP
            goblin.CalculateStats();
            goblin.InitStats();

            skeletonMage.CalculateStats();
            skeletonMage.InitStats();

            rogue.CalculateStats();
            rogue.InitStats();

            mage.CalculateStats();
            mage.InitStats();

            // Simple combat test
            Console.WriteLine("=== Combat Test ===");
            Console.WriteLine($"Goblin HP: {goblin.currentHP}");
            Console.WriteLine($"Rogue attacks Goblin!");

            rogue.Attack(goblin);

            Console.WriteLine($"Goblin HP after attack: {goblin.currentHP}");

            Console.WriteLine("Goblin attacks Rogue!");
            goblin.Attack(rogue);

            Console.WriteLine($"Rogue HP after attack: {rogue.currentHP}");

            // Keep console open
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
