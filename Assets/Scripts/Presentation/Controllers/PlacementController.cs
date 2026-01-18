using Undermarch.Presentation.Managers;
using Undermarch.Presentation.Sounds;
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

        private BuildableDefinition _selectedDefinition;

        // Public property to expose selected type for UI
        public PlacementType SelectedType => _selectedType;
        public BuildableDefinition SelectedDefinition => _selectedDefinition;

        public void SelectBuildable(BuildableDefinition buildable)
        {
            if (buildable == null)
            {
                _selectedType = PlacementType.None;
                _selectedDefinition = null;
                return;
            }

            _selectedType = buildable.placementType;
            _selectedDefinition = buildable;
            
            Debug.Log($"PlacementController: Selected: {buildable.displayName} (Cost: {buildable.goldCost})");
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
                    _selectedDefinition = null;
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
                    _selectedDefinition = null;
                    if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                    return;
                }

                bool placed = false;
                var gameState = GameManager.Instance.GameState;

                // Calculate total cost from definition
                System.Collections.Generic.Dictionary<Undermarch.Simulation.Interfaces.ResourceType, int> totalCost = new ();
                if (_selectedDefinition.goldCost > 0)
                {
                    totalCost.Add(Undermarch.Simulation.Interfaces.ResourceType.Gold, _selectedDefinition.goldCost);
                }
                if (_selectedDefinition.extraCosts != null)
                {
                    foreach(var extra in _selectedDefinition.extraCosts)
                    {
                        if (totalCost.ContainsKey(extra.type))
                            totalCost[extra.type] += extra.amount;
                        else
                            totalCost.Add(extra.type, extra.amount);
                    }
                }

                if (!gameState.CanAfford(totalCost))
                {
                    Debug.Log("Not enough resources!");
                    return; // Early exit if can't afford
                }

                switch (_selectedType)
                {
                    case PlacementType.Slime:
                        {
                            var slime = CharacterDatabase.slimeMonster.Clone();
                            slime.spawnSound = "slimeSound";
                            slime.hurtSound = "slimeSound";
                            slime.attackSound = "slimeSound";
                            slime.deathSound = "slimeSound";

                            GameManager.Instance.Board.AddEntity(tilePos, slime);
                            CharacterEvents.RaiseSpawn(slime);
                            
                            gameState.SpendResources(totalCost); // Spend calculated cost
                            audioController.PlaySlimeSound();
                            placed = true;
                            Debug.Log($"Placed Slime at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.Archer:
                        {
                            var archer = CharacterDatabase.archerMonster.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, archer);
                            CharacterEvents.RaiseSpawn(archer);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Archer at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.Goblin:
                        {
                            var goblin = CharacterDatabase.goblin1.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, goblin);
                            CharacterEvents.RaiseSpawn(goblin);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Goblin at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.SpikeTrap:
                        {
                            GameManager.Instance.Board.AddInteractable(tilePos, new SpikeTrap());
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Spike Trap at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.BearTrap:
                        {
                            GameManager.Instance.Board.AddInteractable(tilePos, new BearTrap());
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Bear Trap at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.MetalSpikeTrap:
                        {
                            GameManager.Instance.Board.AddInteractable(tilePos, new MetalSpikeTrap());
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Metal Spike Trap at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.GasTrap:
                        {
                            GameManager.Instance.Board.AddInteractable(tilePos, new GasTrap());
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Gas Trap at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.RedDemon:
                        {
                            var demon = CharacterDatabase.weakerDemon.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, demon);
                            CharacterEvents.RaiseSpawn(demon);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Red Demon at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.PurpleDemon:
                        {
                            var demon = CharacterDatabase.strongerDemon.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, demon);
                            CharacterEvents.RaiseSpawn(demon);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Purple Demon at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.RedSpider:
                        {
                            var spider = CharacterDatabase.redSpider.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, spider);
                            CharacterEvents.RaiseSpawn(spider);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Red Spider at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.PurpleSpider:
                        {
                            var spider = CharacterDatabase.purpleSpider.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, spider);
                            CharacterEvents.RaiseSpawn(spider);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Purple Spider at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.GreenSlime:
                        {
                            var slime = CharacterDatabase.strongSlime.Clone();
                            GameManager.Instance.Board.AddEntity(tilePos, slime);
                            CharacterEvents.RaiseSpawn(slime);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Green Slime at {tilePos.x},{tilePos.y}");
                            break;
                        }

                    case PlacementType.BlueSlime:
                        {
                            var slime = CharacterDatabase.strongSlime.Clone(); // Or another variant if available
                            GameManager.Instance.Board.AddEntity(tilePos, slime);
                            CharacterEvents.RaiseSpawn(slime);
                            gameState.SpendResources(totalCost);

                            placed = true;
                            Debug.Log($"Placed Blue Slime at {tilePos.x},{tilePos.y}");
                            break;
                        }
                }

                if (placed)
                {
                    // Play placement sound
                    UIAudioManager.Instance?.PlayPlacementSound();
                    
                    // keep selection to allow multiple placements, but maybe check cost again next frame
                    // For now, let's keep it selected until right click deselect
                    // If we want to deselect after one placement: 
                    // _selectedType = PlacementType.None; 
                    // _selectedDefinition = null;
                    // if (_tilemapRenderer != null) _tilemapRenderer.ClearPreview();
                }
            }
        }
    }
}
