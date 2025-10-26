using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Grid;
using UnityEngine;

namespace Undermarch.Presentation.Testing
{
    public class BoardTest : MonoBehaviour
    {
        void Start()
        {
            // Get the board from our GameManager
            var board = GameManager.Instance.Board;
            if (board == null)
            {
                Debug.LogError("Board not initialized in GameManager!");
                return;
            }

            // 1. Subscribe to the board changed event
            board.OnBoardChanged += OnBoardChanged;
            Debug.Log("BoardTest: Subscribed to OnBoardChanged event.");

            // 2. Add a wall
            var wallPos = new TilePos(5, 5);
            Debug.Log($"BoardTest: Adding wall at {wallPos}");
            board.AddWall(wallPos);

            // 3. Add an entity
            var character = CharacterDatabase.warrior.Clone();
            var startPos = new TilePos(3, 3);
            Debug.Log($"BoardTest: Adding a character at {startPos}");
            board.AddEntity(startPos, character);

            // 4. Move the entity
            var endPos = new TilePos(4, 4);
            Debug.Log($"BoardTest: Moving entity from {startPos} to {endPos}");
            board.MoveEntity(startPos, endPos);

            // 5. Verify the move
            var movedCharacter = board.GetEntityAt(endPos);
            if (movedCharacter == character)
            {
                Debug.Log($"SUCCESS: Found character at {endPos} as expected.");
            }
            else
            {
                Debug.LogError($"FAILURE: Did not find character at {endPos}.");
            }
        }

        private void OnBoardChanged(TilePos pos)
        {
            Debug.Log($"OnBoardChanged event received for position: {pos}");
        }

        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (GameManager.Instance != null && GameManager.Instance.Board != null)
            {
                GameManager.Instance.Board.OnBoardChanged -= OnBoardChanged;
            }
        }
    }
}
