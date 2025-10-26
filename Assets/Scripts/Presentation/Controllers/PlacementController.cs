using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Traps;
using Undermarch.Simulation.Grid;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Undermarch.Presentation.Controllers
{
    public class PlacementController : MonoBehaviour
    {
        public enum PlacementType
        {
            None,
            Slime,
            SpikeTrap
        }

        private PlacementType _selectedType = PlacementType.None;

        public void SelectSlime()
        {
            _selectedType = PlacementType.Slime;
            Debug.Log("Selected: Slime");
        }

        public void SelectSpikeTrap()
        {
            _selectedType = PlacementType.SpikeTrap;
            Debug.Log("Selected: Spike Trap");
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Placement) return;
            if (_selectedType == PlacementType.None) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                TilePos tilePos = new TilePos(Mathf.FloorToInt(mouseWorldPos.x), Mathf.FloorToInt(mouseWorldPos.y));

                if (GameManager.Instance.Board.InBounds(tilePos) && 
                    !GameManager.Instance.Board.HasWallAt(tilePos) &&
                    GameManager.Instance.Board.GetEntityAt(tilePos) == null &&
                    GameManager.Instance.Board.GetInteractableAt(tilePos) == null)
                {
                    switch (_selectedType)
                    {
                        case PlacementType.Slime:
                            var monster = CharacterDatabase.slimeMonster.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, monster);
                            Debug.Log($"Placed Slime at {tilePos.x},{tilePos.y}");
                            break;
                        case PlacementType.SpikeTrap:
                            GameManager.Instance.Board.AddInteractable(tilePos, new SpikeTrap());
                            Debug.Log($"Placed Spike Trap at {tilePos.x},{tilePos.y}");
                            break;
                    }
                    // Deselect after placing
                    _selectedType = PlacementType.None;
                }
            }
        }
    }
}
