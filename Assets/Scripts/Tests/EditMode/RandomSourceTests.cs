using NUnit.Framework;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Tests
{
    [TestFixture]
    public class RandomSourceTests
    {
        [Test]
        public void SeededRandom_SameSeed_ProducesSameSequence()
        {
            IRandomSource rng1 = new SeededRandom(12345);
            IRandomSource rng2 = new SeededRandom(12345);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(rng1.Next(), rng2.Next(), $"Mismatch at iteration {i}");
            }
        }

        [Test]
        public void SeededRandom_DifferentSeeds_ProduceDifferentSequences()
        {
            IRandomSource rng1 = new SeededRandom(12345);
            IRandomSource rng2 = new SeededRandom(54321);

            bool foundDifference = false;
            for (int i = 0; i < 10; i++)
            {
                if (rng1.Next() != rng2.Next())
                {
                    foundDifference = true;
                    break;
                }
            }

            Assert.IsTrue(foundDifference, "Different seeds should produce different sequences");
        }

        [Test]
        public void SeededRandom_NextWithMax_StaysInRange()
        {
            IRandomSource rng = new SeededRandom(12345);

            for (int i = 0; i < 100; i++)
            {
                int value = rng.Next(10);
                Assert.GreaterOrEqual(value, 0);
                Assert.Less(value, 10);
            }
        }

        [Test]
        public void SeededRandom_NextWithMinMax_StaysInRange()
        {
            IRandomSource rng = new SeededRandom(12345);

            for (int i = 0; i < 100; i++)
            {
                int value = rng.Next(5, 15);
                Assert.GreaterOrEqual(value, 5);
                Assert.Less(value, 15);
            }
        }

        [Test]
        public void SeededRandom_NextFloat_StaysInRange()
        {
            IRandomSource rng = new SeededRandom(12345);

            for (int i = 0; i < 100; i++)
            {
                float value = rng.NextFloat();
                Assert.GreaterOrEqual(value, 0.0f);
                Assert.Less(value, 1.0f);
            }
        }

        [Test]
        public void SeededRandom_Reset_RestartsSequence()
        {
            SeededRandom rng = new SeededRandom(12345);

            int[] firstSequence = new int[10];
            for (int i = 0; i < 10; i++)
            {
                firstSequence[i] = rng.Next();
            }

            rng.Reset();

            int[] secondSequence = new int[10];
            for (int i = 0; i < 10; i++)
            {
                secondSequence[i] = rng.Next();
            }

            CollectionAssert.AreEqual(firstSequence, secondSequence);
        }
    }
}
