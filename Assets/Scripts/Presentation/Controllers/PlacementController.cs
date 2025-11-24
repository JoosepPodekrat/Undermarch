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
            Goblin,
            SpikeTrap,
            BearTrap
        }
        AudioController audioController = new AudioController();

        private PlacementType _selectedType = PlacementType.None;
   

        public void SelectSlime()
        {
            _selectedType = PlacementType.Slime;
            Debug.Log("PlacementController: Selected: Slime (Cost: 50 gold)");
        }

        public void SelectArcher()
        {
            _selectedType = PlacementType.Archer;
            Debug.Log("PlacementController: Selected: Archer (Cost: 80 gold)");
        }

        public void SelectGoblin()
        {
            _selectedType = PlacementType.Goblin;
            Debug.Log("PlacementController: Selected: Goblin (Cost: 50 gold)");
        }

        public void SelectSpikeTrap()
        {
            _selectedType = PlacementType.SpikeTrap;
            Debug.Log("PlacementController: Selected: Spike Trap (Cost: 30 gold)");
        }

        public void SelectBearTrap()
        {
            _selectedType = PlacementType.BearTrap;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }

        private void Start()
        {
            Debug.Log("PlacementController: Start()");
        }

        private void Update()
        {
            if (GameManager.Instance == null)
            {
                // Reduced spam: only log warning once or rarely? For now, commenting out spam
                // Debug.LogWarning("PlacementController: GameManager is null");
                return;
            }
            if (GameManager.Instance.Board == null)
            {
               // Debug.LogWarning("PlacementController: Board is null");
                return;
            }
            if (GameManager.Instance.GameState == null)
            {
               // Debug.LogWarning("PlacementController: GameState is null");
                return;
            }

            if (GameManager.Instance.CurrentPhase != GamePhase.Placement &&
                GameManager.Instance.CurrentPhase != GamePhase.Combat &&
                GameManager.Instance.CurrentPhase != GamePhase.BuildingPhase2) return;

            if (_selectedType == PlacementType.None) return;

            if (Camera.main == null) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Use Plane Raycast for robust world position finding regardless of Camera Z/Rotation
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Plane boardPlane = new Plane(Vector3.back, Vector3.zero); // Plane at Z=0 facing Camera

                if (boardPlane.Raycast(ray, out float enter))
                {
                    Vector3 mouseWorldPos = ray.GetPoint(enter);

                    // Adjust for visual offset (centered board)
                    int boardWidth = GameManager.Instance.Board.Width;
                    int boardHeight = GameManager.Instance.Board.Height;
                    TilePos tilePos = new TilePos(
                        Mathf.FloorToInt(mouseWorldPos.x + boardWidth / 2f), 
                        Mathf.FloorToInt(mouseWorldPos.y + boardHeight / 2f)
                    );

                    Debug.Log($"PlacementController: Click at World {mouseWorldPos}, Tile {tilePos}");

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
                                        audioController.PlaySlimeSound();
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
                                case PlacementType.Goblin:
                                    if (GameManager.Instance.GameState.CanAfford(50))
                                    {
                                        var goblin = CharacterDatabase.goblin1.Clone();
                                        GameManager.Instance.GameState.SpendGold(50);
                                        GameManager.Instance.Board.AddEntity(tilePos, goblin);
                                        Debug.Log($"Placed Goblin {tilePos.x},{tilePos.y} for 50 gold");
                                        placed = true;
                                       
                                    }
                                    else
                                    {
                                        Debug.Log("Not enough gold to place Goblin (need 50)!");
                                    } break;

                                case PlacementType.BearTrap:
                                    if (GameManager.Instance.GameState.CanAfford(50))
                                    {
                                        GameManager.Instance.Board.AddInteractable(tilePos, new BearTrap());
                                        GameManager.Instance.GameState.SpendGold(50);
                                        Debug.Log($"Placed Bear Trap at {tilePos.x},{tilePos.y} for 50 gold");
                                        placed = true;
                                    }
                                    else
                                    {
                                        Debug.Log("Not enough gold to place Bear Trap (need 50)!");
                                    }
                                    break;

                            }

                            if (placed) { _selectedType = PlacementType.None; }
                        }
                        else
                        {
                            Debug.Log($"PlacementController: Invalid tile {tilePos}. Wall: {hasWall}, Entity: {entity}, Interactable: {interactable}");
                        }
                    }
                    else
                    {
                        Debug.Log($"PlacementController: Click out of bounds {tilePos}");
                    }
                }
            }
        }
    }
}