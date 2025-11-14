using NUnit.Framework;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Combat;

namespace Undermarch.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void Board_InitializesWithCorrectDimensions()
        {
            IBoard board = new Board(20, 15);

            Assert.AreEqual(20, board.Width);
            Assert.AreEqual(15, board.Height);
        }

        [Test]
        public void InBounds_WithValidPosition_ReturnsTrue()
        {
            IBoard board = new Board(10, 10);

            Assert.IsTrue(board.InBounds(new TilePos(0, 0)));
            Assert.IsTrue(board.InBounds(new TilePos(5, 5)));
            Assert.IsTrue(board.InBounds(new TilePos(9, 9)));
        }

        [Test]
        public void InBounds_WithInvalidPosition_ReturnsFalse()
        {
            IBoard board = new Board(10, 10);

            Assert.IsFalse(board.InBounds(new TilePos(-1, 0)));
            Assert.IsFalse(board.InBounds(new TilePos(0, -1)));
            Assert.IsFalse(board.InBounds(new TilePos(10, 0)));
            Assert.IsFalse(board.InBounds(new TilePos(0, 10)));
        }

        [Test]
        public void AddWall_CreatesWallAtPosition()
        {
            IBoard board = new Board(10, 10);
            TilePos wallPos = new TilePos(5, 5);

            board.AddWall(wallPos);

            Assert.IsTrue(board.HasWallAt(wallPos));
        }

        [Test]
        public void AddEntity_PlacesCharacterOnBoard()
        {
            IBoard board = new Board(10, 10);
            Character character = new Character { Name = "TestChar" };
            TilePos pos = new TilePos(3, 3);

            board.AddEntity(pos, character);

            Assert.AreEqual(character, board.GetEntityAt(pos));
        }

        [Test]
        public void RemoveEntity_RemovesCharacterFromBoard()
        {
            IBoard board = new Board(10, 10);
            Character character = new Character { Name = "TestChar" };
            TilePos pos = new TilePos(3, 3);

            board.AddEntity(pos, character);
            board.RemoveEntity(pos);

            Assert.IsNull(board.GetEntityAt(pos));
        }

        [Test]
        public void MoveEntity_MovesCharacterToNewPosition()
        {
            IBoard board = new Board(10, 10);
            Character character = new Character { Name = "TestChar" };
            TilePos from = new TilePos(3, 3);
            TilePos to = new TilePos(4, 4);

            board.AddEntity(from, character);
            board.MoveEntity(from, to);

            Assert.IsNull(board.GetEntityAt(from));
            Assert.AreEqual(character, board.GetEntityAt(to));
        }

        [Test]
        public void GetPositionOf_ReturnsCorrectPosition()
        {
            IBoard board = new Board(10, 10);
            Character character = new Character { Name = "TestChar" };
            TilePos pos = new TilePos(7, 2);

            board.AddEntity(pos, character);

            Assert.AreEqual(pos, board.GetPositionOf(character));
        }

        [Test]
        public void FindClosestTarget_ReturnsNearestEnemyOfFaction()
        {
            IBoard board = new Board(10, 10);

            Character seeker = new Character { Name = "Seeker", faction = Faction.Defender };
            Character target1 = new Character { Name = "Far", faction = Faction.Hero };
            Character target2 = new Character { Name = "Close", faction = Faction.Hero };

            board.AddEntity(new TilePos(5, 5), seeker);
            board.AddEntity(new TilePos(8, 8), target1);
            board.AddEntity(new TilePos(6, 5), target2);

            Character closest = board.FindClosestTarget(seeker, Faction.Hero);

            Assert.AreEqual(target2, closest);
        }

        [Test]
        public void GetAllCharacters_ReturnsAllEntities()
        {
            IBoard board = new Board(10, 10);

            Character char1 = new Character { Name = "Char1" };
            Character char2 = new Character { Name = "Char2" };

            board.AddEntity(new TilePos(1, 1), char1);
            board.AddEntity(new TilePos(2, 2), char2);

            var allChars = board.GetAllCharacters();
            int count = 0;
            foreach (var _ in allChars) count++;

            Assert.AreEqual(2, count);
        }
    }
}
