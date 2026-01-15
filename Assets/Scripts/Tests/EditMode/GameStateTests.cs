using NUnit.Framework;
using System;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Tests
{
    [TestFixture]
    public class GameStateTests
    {
        [Test]
        public void GameState_InitializesWithCorrectValues()
        {
            IGameState gameState = new GameState(150);

            Assert.AreEqual(150, gameState.CurrentGold);
            Assert.AreEqual(GamePhase.Placement, gameState.Phase);
        }

        [Test]
        public void CanAfford_WithSufficientGold_ReturnsTrue()
        {
            IGameState gameState = new GameState(100);

            Assert.IsTrue(gameState.CanAfford(50));
            Assert.IsTrue(gameState.CanAfford(100));
        }

        [Test]
        public void CanAfford_WithInsufficientGold_ReturnsFalse()
        {
            IGameState gameState = new GameState(100);

            Assert.IsFalse(gameState.CanAfford(101));
            Assert.IsFalse(gameState.CanAfford(200));
        }

        [Test]
        public void SpendGold_DeductsCorrectAmount()
        {
            IGameState gameState = new GameState(100);

            gameState.SpendGold(30);

            Assert.AreEqual(70, gameState.CurrentGold);
        }

        [Test]
        public void SpendGold_WithInsufficientGold_ThrowsException()
        {
            IGameState gameState = new GameState(100);

            Assert.Throws<InvalidOperationException>(() => gameState.SpendGold(150));
        }

        [Test]
        public void EarnGold_AddsCorrectAmount()
        {
            IGameState gameState = new GameState(100);

            gameState.EarnGold(50);

            Assert.AreEqual(150, gameState.CurrentGold);
        }

        [Test]
        public void SpendGold_TriggersResourcesChangedEvent()
        {
            IGameState gameState = new GameState(100);
            bool eventTriggered = false;
            gameState.OnResourcesChanged += () => eventTriggered = true;

            gameState.SpendGold(10);

            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void EarnGold_TriggersResourcesChangedEvent()
        {
            IGameState gameState = new GameState(100);
            bool eventTriggered = false;
            gameState.OnResourcesChanged += () => eventTriggered = true;

            gameState.EarnGold(20);

            Assert.IsTrue(eventTriggered);
        }
    }
}
