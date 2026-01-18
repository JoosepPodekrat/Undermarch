using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Traps;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Undermarch.Simulation.Interfaces;
using Undermarch.Presentation.Rendering;

using Undermarch.Presentation.Rendering;
using Undermarch.Data;

namespace Undermarch.Presentation.Controllers
{
    public class PlacementController : MonoBehaviour
    {
        // PlacementType moved to Undermarch.Data namespace

        private PlacementType _selectedType = PlacementType.None;
        private AudioController audioController;
        private TilemapRenderer _tilemapRenderer;

        private Vector2 _rightMouseStartPos;
        private bool _isRightClicking;

        // Public property to expose selected type for UI
        public PlacementType SelectedType => _selectedType;


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

        public void SelectMetalSpikeTrap()
        {
            _selectedType = PlacementType.MetalSpikeTrap;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectGasTrap()
        {
            _selectedType = PlacementType.GasTrap;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectRedDemon()
        {
            _selectedType = PlacementType.RedDemon;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectPurpleDemon()
        {
            _selectedType = PlacementType.PurpleDemon;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectRedSpider()
        {
            _selectedType = PlacementType.RedSpider;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectPurpleSpider()
        {
            _selectedType = PlacementType.PurpleSpider;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectGreenSlime()
        {
            _selectedType = PlacementType.GreenSlime;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }
        public void SelectBlueSlime()
        {
            _selectedType = PlacementType.BlueSlime;
            Debug.Log("PlacementController: Selected: Bear Trap (Cost: 50 gold)");
        }

        private void Start()
        {
            audioController = FindFirstObjectByType<AudioController>();
            if (audioController == null)
                Debug.LogError("PlacementController: No AudioController found in scene!");

            _tilemapRenderer = FindFirstObjectByType<TilemapRenderer>();
        }



        private void Update()
        {
            if (_selectedType == PlacementType.None)
            {
                if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                return;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _rightMouseStartPos = Mouse.current.position.ReadValue();
                _isRightClicking = true;
            }

            if (_isRightClicking && Mouse.current.rightButton.wasReleasedThisFrame)
            {
                _isRightClicking = false;
                if (Vector2.Distance(_rightMouseStartPos, Mouse.current.position.ReadValue()) < 10f)
                {
                    _selectedType = PlacementType.None;
                    if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                    return;
                }
            }

            if (Camera.main == null) return;
            if (GameManager.Instance?.Board == null || GameManager.Instance.GameState == null) return;
            if (GameManager.Instance.CurrentPhase != GamePhase.Placement &&
                GameManager.Instance.CurrentPhase != GamePhase.Combat &&
                GameManager.Instance.CurrentPhase != GamePhase.BuildingPhase2)
            {
                if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                return;
            }

            // Raycast for Ghost & Input
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane boardPlane = new Plane(Vector3.back, Vector3.zero);

            if (!boardPlane.Raycast(ray, out float enter))
            {
                if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                return;
            }

            Vector3 mouseWorldPos = ray.GetPoint(enter);
            int boardWidth = GameManager.Instance.Board.Width;
            int boardHeight = GameManager.Instance.Board.Height;

            TilePos tilePos = new TilePos(
                Mathf.FloorToInt(mouseWorldPos.x + boardWidth / 2f),
                Mathf.FloorToInt(mouseWorldPos.y + boardHeight / 2f)
            );

            bool inBounds = GameManager.Instance.Board.InBounds(tilePos);
            bool isValid = false;

            if (inBounds)
            {
                bool hasWall = GameManager.Instance.Board.HasWallAt(tilePos);
                var entity = GameManager.Instance.Board.GetEntityAt(tilePos);
                var interactable = GameManager.Instance.Board.GetInteractableAt(tilePos);
                isValid = !hasWall && entity == null && interactable == null;
            }

            // Update Preview
            if (_tilemapRenderer != null)
            {
                if (inBounds) _tilemapRenderer.DrawPreview(tilePos, _selectedType, isValid);
                else _tilemapRenderer.ClearPreview();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {

                if (!inBounds) return;

                if (!isValid)
                {
                    _selectedType = PlacementType.None;
                    if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                    return;
                }

                bool placed = false;
                var gameState = GameManager.Instance.GameState;
                switch (_selectedType)
                {
                    case PlacementType.Slime:
                        {
                            var cost = gameState.PlacementCosts["SlimeMonster"];
                            if (!gameState.CanAfford(cost))
                            {
                                Debug.Log("Not enough resources to place Slime!");
                                break;
                            }

                            var slime = CharacterDatabase.slimeMonster.Clone();
                            slime.spawnSound = "slimeSound";
                            slime.hurtSound = "slimeSound";
                            slime.attackSound = "slimeSound";
                            slime.deathSound = "slimeSound";

                            GameManager.Instance.Board.AddEntity(tilePos, slime);
                            CharacterEvents.RaiseSpawn(slime);
                            gameState.SpendResources(cost);

                            audioController.PlaySlimeSound();
                            placed = true;
                            Debug.Log($"Placed Slime at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.Archer:
                        {
                            var cost = gameState.PlacementCosts["ArcherMonster"];
                            if (!gameState.CanAfford(cost))
                                break;

                            var archer = CharacterDatabase.archerMonster.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, archer);
                            CharacterEvents.RaiseSpawn(archer);
                            gameState.SpendResources(cost);

                            placed = true;
                            Debug.Log($"Placed Archer at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.Goblin:
                        {
                            var cost = gameState.PlacementCosts["Goblin"];
                            if (!gameState.CanAfford(cost))
                                break;

                            var goblin = CharacterDatabase.goblin1.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, goblin);
                            CharacterEvents.RaiseSpawn(goblin);
                            gameState.SpendResources(cost);

                            placed = true;
                            Debug.Log($"Placed Goblin at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.SpikeTrap:
                        {
                            var cost = gameState.PlacementCosts["SpikeTrap"];
                            if (!gameState.CanAfford(cost))
                                break;

                            GameManager.Instance.Board.AddInteractable(tilePos, new SpikeTrap());
                            gameState.SpendResources(cost);

                            placed = true;
                            Debug.Log($"Placed Spike Trap at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.BearTrap:
                        {
                            var cost = gameState.PlacementCosts["BearTrap"];
                            if (!gameState.CanAfford(cost))
                                break;

                            GameManager.Instance.Board.AddInteractable(tilePos, new BearTrap());
                            gameState.SpendResources(cost);

                            placed = true;
                            Debug.Log($"Placed Bear Trap at {tilePos.x},{tilePos.y}");
                            break;
                        }
                }

                if (placed)
                {
                    _selectedType = PlacementType.None;
                    if (_tilemapRenderer != null)
                        _tilemapRenderer.ClearPreview();
                }
            }
        }
    }
}

