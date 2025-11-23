using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Traps;
using Undermarch.Simulation.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Undermarch.Simulation.Interfaces;
using UnityEditor.Experimental.GraphView;

namespace Undermarch.Presentation.Controllers
{
    public class PlacementController : MonoBehaviour
    {
        public enum PlacementType
        {
            None,
            Slime,
            Archer,
            SpikeTrap
        }

        private PlacementType _selectedType = PlacementType.None;
   

        public void SelectSlime()
        {
            _selectedType = PlacementType.Slime;
            Debug.Log("Selected: Slime (Cost: 50 gold)");
        }

        public void SelectArcher()
        {
            _selectedType = PlacementType.Archer;
            Debug.Log("Selected: Archer (Cost: 80 gold)");
        }

        public void SelectSpikeTrap()
        {
            _selectedType = PlacementType.SpikeTrap;
            Debug.Log("Selected: Spike Trap (Cost: 30 gold)");
        }

        private void Start()
        {
            Debug.Log("PlacementController: Start()");
        }

        private void Update()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("PlacementController: GameManager is null");
                return;
            }
            if (GameManager.Instance.Board == null)
            {
                Debug.LogWarning("PlacementController: Board is null");
                return;
            }
            if (GameManager.Instance.GameState == null)
            {
                Debug.LogWarning("PlacementController: GameState is null");
                return;
            }

            if (GameManager.Instance.CurrentPhase != GamePhase.Placement &&
                GameManager.Instance.CurrentPhase != GamePhase.Combat) return;

            if (_selectedType == PlacementType.None) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
                
                // Adjust for visual offset (centered board)
                int boardWidth = GameManager.Instance.Board.Width;
                int boardHeight = GameManager.Instance.Board.Height;
                TilePos tilePos = new TilePos(
                    Mathf.FloorToInt(mouseWorldPos.x + boardWidth / 2f), 
                    Mathf.FloorToInt(mouseWorldPos.y + boardHeight / 2f)
                );

                Debug.Log($"PlacementController: Click at Screen {mouseScreenPos}, World {mouseWorldPos}, Tile {tilePos}");

                if (GameManager.Instance.Board.InBounds(tilePos))
                {
                    bool hasWall = GameManager.Instance.Board.HasWallAt(tilePos);
                    var entity = GameManager.Instance.Board.GetEntityAt(tilePos);
                    var interactable = GameManager.Instance.Board.GetInteractableAt(tilePos);

                    if (!hasWall && entity == null && interactable == null)
                    {
                        Debug.Log($"PlacementController: Valid tile {tilePos}. Attempting to place {_selectedType}");
                        // Check cost and place entity
                        bool placed = false;

                        switch (_selectedType)
                    {
                        case PlacementType.Slime:
                            if (GameManager.Instance.GameState.CanAfford(50))
                            {
                                var slime = CharacterDatabase.slimeMonster.Clone();
                                GameManager.Instance.Board.AddEntity(tilePos, slime);
                                GameManager.Instance.GameState.SpendGold(50);
                                Debug.Log($"Placed Slime at {tilePos.x},{tilePos.y} for 50 gold");
                                placed = true;
                            }
                            else
                            {
                                Debug.Log("Not enough gold to place Slime (need 50)!");
                            }
                            break;

                        case PlacementType.Archer:
                            if (GameManager.Instance.GameState.CanAfford(80))
                            {
                                var archer = CharacterDatabase.archerMonster.Clone();
                                GameManager.Instance.Board.AddEntity(tilePos, archer);
                                GameManager.Instance.GameState.SpendGold(80);
                                Debug.Log($"Placed Archer at {tilePos.x},{tilePos.y} for 80 gold");
                                placed = true;
                            }
                            else
                            {
                                Debug.Log("Not enough gold to place Archer (need 80)!");
                            }
                            break;

                        case PlacementType.SpikeTrap:
                            if (GameManager.Instance.GameState.CanAfford(30))
                            {
                                GameManager.Instance.Board.AddInteractable(tilePos, new SpikeTrap());
                                GameManager.Instance.GameState.SpendGold(30);
                                Debug.Log($"Placed Spike Trap at {tilePos.x},{tilePos.y} for 30 gold");
                                placed = true;
                            }
                            else
                            {
                                Debug.Log("Not enough gold to place Spike Trap (need 30)!");
                            }
                            break;
                    }

                    // Deselect after successful placement
                    // I don't think we want this behavior, makes you rechoose every time if you want the same defense again. I'll disable it for now
                    // Need to make it so clicking on the UI doesn't try to place one down behind the UI - couldn't get it working properly.
                    if (placed) { _selectedType = PlacementType.None;}
                    }
                }
            }
        }
    }
}
