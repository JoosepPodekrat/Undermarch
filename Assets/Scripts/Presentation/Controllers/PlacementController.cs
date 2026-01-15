using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Traps;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using Undermarch.Simulation.Interfaces;
using Undermarch.Presentation.Rendering;

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

        private PlacementType _selectedType = PlacementType.None;
        private AudioController audioController;
        private TilemapRenderer _tilemapRenderer;
        
        private Vector2 _rightMouseStartPos;
        private bool _isRightClicking;
   

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

switch (_selectedType)
{
    case PlacementType.Slime:
        if (GameManager.Instance.GameState.CanAfford(50))
        {
            var slime = CharacterDatabase.slimeMonster.Clone();
            slime.spawnSound = "slimeSound";
            slime.hurtSound = "slimeSound";
            slime.attackSound = "slimeSound";
            slime.deathSound = "slimeSound";

            GameManager.Instance.Board.AddEntity(tilePos, slime);
            CharacterEvents.RaiseSpawn(slime);
            GameManager.Instance.GameState.SpendGold(50);
            audioController.PlaySlimeSound();
            placed = true;
            Debug.Log($"Placed Slime at {tilePos.x},{tilePos.y} for 50 gold");
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
            CharacterEvents.RaiseSpawn(archer);
            GameManager.Instance.GameState.SpendGold(80);
            placed = true;
            Debug.Log($"Placed Archer at {tilePos.x},{tilePos.y} for 80 gold");
        }
        break;

    case PlacementType.Goblin:
        if (GameManager.Instance.GameState.CanAfford(50))
        {
            var goblin = CharacterDatabase.goblin1.Clone();
            GameManager.Instance.Board.AddEntity(tilePos, goblin);
            CharacterEvents.RaiseSpawn(goblin);
            GameManager.Instance.GameState.SpendGold(50);
            placed = true;
            Debug.Log($"Placed Goblin at {tilePos.x},{tilePos.y} for 50 gold");
        }
        break;

    case PlacementType.SpikeTrap:
        if (GameManager.Instance.GameState.CanAfford(30))
        {
            GameManager.Instance.Board.AddInteractable(tilePos, new SpikeTrap());
            GameManager.Instance.GameState.SpendGold(30);
            placed = true;
            Debug.Log($"Placed Spike Trap at {tilePos.x},{tilePos.y} for 30 gold");
        }
        break;

    case PlacementType.BearTrap:
        if (GameManager.Instance.GameState.CanAfford(50))
        {
            GameManager.Instance.Board.AddInteractable(tilePos, new BearTrap());
            GameManager.Instance.GameState.SpendGold(50);
            placed = true;
            Debug.Log($"Placed Bear Trap at {tilePos.x},{tilePos.y} for 50 gold");
        }
        break;
}

if (placed)
    _selectedType = PlacementType.None;

        }
    }
}
}

