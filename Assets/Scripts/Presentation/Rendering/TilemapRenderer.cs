using System.Collections;
using System.Collections.Generic;
using Undermarch;
using Undermarch.Presentation.Controllers;
using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Entities.Characters.Monsters;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Undermarch.Presentation.Rendering
{
    public sealed class TilemapRenderer : MonoBehaviour
    {
        [Header("Tilemap Layers")]
        public Tilemap groundTilemap;
        public Tilemap wallTilemap;
        public Tilemap interactableTilemap;
        public Tilemap entityTilemap;
        public Tilemap effectsTilemap;
        public Tilemap previewTilemap;

        [Header("Tile Assets")]
        public TileBase groundTile;
        public TileBase wallTile;
        public TileBase heroTile;
        public TileBase trapTile;
        public TileBase bearTrapTile;
        public TileBase chestTile;
        public TileBase arrowTile;
        public TileBase poisonCloudTile;
        public TileBase slowZoneTile;
        public TileBase fireZoneTile;
        public TileBase fogZoneTile;
        [Header("Knights")]
        public TileBase orangeKnightTile;
        public TileBase orangeKnightFightingTile;

        public TileBase purpleKnightTile;
        public TileBase purpleKnightFightingTile;

        public TileBase blueKnightTile;
        public TileBase blueKnightFightingTile;

        [Header("Humanoids")]
        public TileBase peasantTile;
        public TileBase peasantFightingTile;

        public TileBase wizardTile;
        public TileBase wizardFightingTile;

        public TileBase priestessTile;
        public TileBase priestessFightingTile;

        [Header("Monsters")]
        public TileBase goblinTile;
        public TileBase goblinFightingTile;

        public TileBase skeletonTile;
        public TileBase skeletonFightingTile;

        public TileBase slimeTile;
        public TileBase slimeFightingTile;
        public TileBase archerTile;

        [Header("Dungeon Master")]
        public TileBase dungeonMasterTile;
        public TileBase dungeonMasterFightingTile;

        [Header("Traps")]
        public TileBase woodenSpikeTrapTile;
        public TileBase ironSpikeTrapTile;
        public TileBase gasTrapTile;
        public TileBase pressurePlateTile;

        private Board _board;
        private TilePos? _lastPreviewPos;
        private Dictionary<Character, TileBase> _overrideTiles = new Dictionary<Character, TileBase>();
        private Dictionary<Character, Color> _overrideColors = new Dictionary<Character, Color>();

        [Header("Breathing Animation")]
        private Dictionary<Character, BreathingState> _breathingStates = new Dictionary<Character, BreathingState>();
        private System.Random _rng = new System.Random();

        private class BreathingState
        {
            public float offset;           // Random start offset (0-1)
            public float speed;            // Animation speed (approx 1 cycle/sec)
            public float amplitude;        // Scale amount (0.05 = 5% squash/stretch)

            public BreathingState(System.Random rng)
            {
                offset = (float)rng.NextDouble();
                speed = 0.5f + (float)rng.NextDouble() * 0.2f;  // 0.5-0.7 sec (slower)
                amplitude = 0.015f + (float)rng.NextDouble() * 0.02f;  // 1.5-3.5% scale (less)
            }
        }

        void Start()
        {
            // Get the board from the GameManager
            _board = GameManager.Instance.Board;
            if (_board == null)
            {
                Debug.LogError("TilemapRenderer could not get Board from GameManager.");
                return;
            }

            // Subscribe to board changes and do an initial full redraw
            _board.OnBoardChanged += UpdateTile;

            CharacterEvents.OnCharacterAttacked += HandleAttack;
            CharacterEvents.OnCharacterHurt += HandleHurt;
            CharacterEvents.OnCharacterSpawned += HandleCharacterSpawned;

            RedrawAll();

            // Initialize breathing states for all existing characters
            foreach (var character in _board.GetAllCharacters())
            {
                InitializeBreathingState(character);
            }
        }

        void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (_board != null)
            {
                _board.OnBoardChanged -= UpdateTile;
            }

            CharacterEvents.OnCharacterAttacked -= HandleAttack;
            CharacterEvents.OnCharacterHurt -= HandleHurt;
            CharacterEvents.OnCharacterSpawned -= HandleCharacterSpawned;
        }

        private void InitializeBreathingState(Character character)
        {
            if (!_breathingStates.ContainsKey(character))
            {
                _breathingStates[character] = new BreathingState(_rng);
            }
        }

        private void HandleCharacterSpawned(Character character)
        {
            InitializeBreathingState(character);
        }

        void Update()
        {
            if (_board == null || entityTilemap == null) return;

            float time = Time.time;

            // Clean up dead characters from breathing states
            var toRemove = new List<Character>();
            foreach (var kvp in _breathingStates)
            {
                // Check if character still exists on board
                TilePos pos = _board.GetPositionOf(kvp.Key);
                if (!_board.InBounds(pos) || _board.GetEntityAt(pos) != kvp.Key)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var c in toRemove)
            {
                _breathingStates.Remove(c);
            }

            // Apply breathing animation
            foreach (var kvp in _breathingStates)
            {
                Character character = kvp.Key;
                BreathingState state = kvp.Value;

                // Get character position
                TilePos pos = _board.GetPositionOf(character);
                if (!_board.InBounds(pos)) continue;

                var cellPos = new Vector3Int(pos.x - _board.Width / 2, pos.y - _board.Height / 2, 0);

                // Calculate breathing scale using sine wave
                float phase = (time * state.speed + state.offset) * Mathf.PI * 2;
                float scaleY = 1f + Mathf.Sin(phase) * state.amplitude;
                float scaleX = 1f - Mathf.Sin(phase) * (state.amplitude * 0.5f); // Preserve volume: shrink X when Y grows

                // Create transform matrix scaling from bottom (feet fixed)
                // For a 1x1 tile, center is at (0,0), bottom is at y=-0.5
                Matrix4x4 scaleMatrix = Matrix4x4.TRS(
                    Vector3.zero,
                    Quaternion.identity,
                    new Vector3(scaleX, scaleY, 1f)
                );
                Matrix4x4 translateUp = Matrix4x4.Translate(new Vector3(0, 0.5f, 0));
                Matrix4x4 translateDown = Matrix4x4.Translate(new Vector3(0, -0.5f, 0));

                // Final transform: translateDown * scale * translateUp (scales from bottom anchor)
                Matrix4x4 transform = translateDown * scaleMatrix * translateUp;

                entityTilemap.SetTransformMatrix(cellPos, transform);
            }
        }

        /// <summary>
        /// Redraws all tiles on the map based on the current board state.
        /// </summary>
        private void RedrawAll()
        {
            if (_board == null) return;
            ClearAll();

            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    UpdateTile(new TilePos(x, y));
                }
            }
        }

        /// <summary>
        /// Updates a single tile based on the board state at that position.
        /// This is called by the OnBoardChanged event.
        /// </summary>
        private void UpdateTile(TilePos pos)
        {
            if (_board == null) return;
            var cellPos = new Vector3Int(pos.x - _board.Width / 2, pos.y - _board.Height / 2, 0);

            // Ground Layer (always drawn)
            groundTilemap.SetTile(cellPos, groundTile);

            // Wall Layer
            if (_board.HasWallAt(pos))
            {
                wallTilemap.SetTile(cellPos, wallTile);
            }
            else
            {
                wallTilemap.SetTile(cellPos, null);
            }

            // Interactable Layer (Traps, Chests)
            var interactable = _board.GetInteractableAt(pos);
            if (interactable is Undermarch.Simulation.Entities.Traps.BearTrap)
            {
                interactableTilemap.SetTile(cellPos, bearTrapTile);
            }
            else if (interactable is Trap)
            {
                interactableTilemap.SetTile(cellPos, trapTile);
            }
            else if (interactable is Chest chest && !chest.Looted)
            {
                interactableTilemap.SetTile(cellPos, chestTile);
            }
            else
            {
                interactableTilemap.SetTile(cellPos, null);
            }

            // Effects Layer (Projectiles, Tile Effects)
            // Check for projectiles first (they are stored in interactables)
            if (interactable is Projectile proj && proj.IsActive)
            {
                effectsTilemap.SetTile(cellPos, arrowTile);
            }
            // Check for tile effects
            else if (interactable is TileEffect effect)
            {
                TileBase effectTile = effect.Type switch
                {
                    EffectType.Poison => poisonCloudTile,
                    EffectType.Slow => slowZoneTile,
                    EffectType.Fire => fireZoneTile,
                    EffectType.Fog => fogZoneTile,
                    _ => null
                };
                if (effectTile != null)
                {
                    effectsTilemap.SetTile(cellPos, effectTile);
                }
                else
                {
                    effectsTilemap.SetTile(cellPos, null);
                }
            }
            else
            {
                effectsTilemap.SetTile(cellPos, null);
            }

            var entity = _board.GetEntityAt(pos);

            if (entity is Character character)
            {
                TileBase tile = _overrideTiles.ContainsKey(character) ? _overrideTiles[character] : null;

                if (tile == null)
                {
                    switch (character.Name)
                    {
                        case "Peasant":
                            tile = peasantTile;
                            break;

                        case "Wizard":
                            tile = wizardTile;
                            break;

                        case "Priestess":
                            tile = priestessTile;
                            break;

                        case "OrangeKnight":
                            tile = orangeKnightTile;
                            break;

                        case "PurpleKnight":
                            tile = purpleKnightTile;
                            break;

                        case "BlueKnight":
                            tile = blueKnightTile;
                            break;

                        case "Goblin":
                            tile = goblinTile;
                            break;

                        case "Skeleton":
                            tile = skeletonTile;
                            break;

                        case "Slime":
                            tile = slimeTile;
                            break;

                        case "DungeonMaster":
                            tile = dungeonMasterTile;
                            break;

                        case "WoodenSpikeTrap":
                            tile = woodenSpikeTrapTile;
                            break;

                        case "IronSpikeTrap":
                            tile = ironSpikeTrapTile;
                            break;

                        case "GasTrap":
                            tile = gasTrapTile;
                            break;

                        case "PressurePlate":
                            tile = pressurePlateTile;
                            break;

                        default:
                            Debug.LogWarning(
                                "TilemapRenderer: No tile mapped for character name '" + character.Name + "'"
                            );
                            break;
                    }
                }

                entityTilemap.SetTile(cellPos, tile);

                // Reset transform when tile changes (will be reapplied in Update)
                entityTilemap.SetTransformMatrix(cellPos, Matrix4x4.identity);

                if (_overrideColors.ContainsKey(character))
                {
                    entityTilemap.SetTileFlags(cellPos, TileFlags.None);
                    entityTilemap.SetColor(cellPos, _overrideColors[character]);
                }
                else
                {
                    entityTilemap.SetTileFlags(cellPos, TileFlags.None);
                    entityTilemap.SetColor(cellPos, Color.white);
                }
            }
            else
            {
                entityTilemap.SetTile(cellPos, null);
                entityTilemap.SetTransformMatrix(cellPos, Matrix4x4.identity);
            }

        }

        /// <summary>
        /// Clears all tilemaps.
        /// </summary>
        private void ClearAll()
        {
            groundTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();
            interactableTilemap.ClearAllTiles();
            entityTilemap.ClearAllTiles();
            effectsTilemap.ClearAllTiles();
            if (previewTilemap != null) previewTilemap.ClearAllTiles();
        }

        public void ClearPreview()
        {
            if (previewTilemap == null) return;
            previewTilemap.ClearAllTiles();
            _lastPreviewPos = null;
        }

        public void DrawPreview(TilePos pos, PlacementController.PlacementType type, bool isValid)
        {
            if (previewTilemap == null) return;

            // Only update if position changed to avoid spamming SetTile (optimization)
            // However, type or validity might change at same pos, so we should check those too?
            // For now, simpler to just redraw if anything could be different or just always redraw (it's per frame).
            // Let's clear previous if different position.
            if (_lastPreviewPos.HasValue && _lastPreviewPos.Value != pos)
            {
                var oldCellPos = new Vector3Int(_lastPreviewPos.Value.x - _board.Width / 2, _lastPreviewPos.Value.y - _board.Height / 2, 0);
                previewTilemap.SetTile(oldCellPos, null);
            }

            TileBase tile = null;
            switch (type)
            {
                case PlacementController.PlacementType.Slime:
                    tile = slimeTile;
                    break;
                case PlacementController.PlacementType.Archer:
                    tile = archerTile ?? peasantTile; // Fallback if archerTile is missing
                    break;
                case PlacementController.PlacementType.Goblin:
                    tile = goblinTile;
                    break;
                case PlacementController.PlacementType.SpikeTrap:
                    tile = woodenSpikeTrapTile; // Assumption based on context
                    break;
                case PlacementController.PlacementType.BearTrap:
                    tile = bearTrapTile;
                    break;
            }

            if (tile != null)
            {
                var cellPos = new Vector3Int(pos.x - _board.Width / 2, pos.y - _board.Height / 2, 0);
                previewTilemap.SetTile(cellPos, tile);
                
                // Color
                previewTilemap.SetTileFlags(cellPos, TileFlags.None);
                Color color = isValid ? Color.cyan : Color.red;
                color.a = 0.5f;
                previewTilemap.SetColor(cellPos, color);
                
                _lastPreviewPos = pos;
            }
        }

        private void HandleAttack(Character character)
        {
            StartCoroutine(AnimateAttack(character));
        }

        private void HandleHurt(Character character)
        {
            StartCoroutine(AnimateHurt(character));
        }

        private IEnumerator AnimateAttack(Character character)
        {
            TileBase fightTile = GetFightingTile(character.Name);
            if (fightTile != null)
            {
                _overrideTiles[character] = fightTile;
                RefreshCharacterTile(character);
                yield return new WaitForSeconds(0.2f);
                _overrideTiles.Remove(character);
                RefreshCharacterTile(character);
            }
        }

        private IEnumerator AnimateHurt(Character character)
        {
            _overrideColors[character] = Color.red;
            RefreshCharacterTile(character);
            yield return new WaitForSeconds(0.1f);
            _overrideColors.Remove(character);
            RefreshCharacterTile(character);
        }

        private void RefreshCharacterTile(Character character)
        {
            if (_board == null) return;
            TilePos pos = _board.GetPositionOf(character);
            if (_board.InBounds(pos))
            {
                UpdateTile(pos);
            }
        }

        private TileBase GetFightingTile(string charName)
        {
            return charName switch
            {
                "Peasant" => peasantFightingTile,
                "Wizard" => wizardFightingTile,
                "Priestess" => priestessFightingTile,
                "OrangeKnight" => orangeKnightFightingTile,
                "PurpleKnight" => purpleKnightFightingTile,
                "BlueKnight" => blueKnightFightingTile,
                "Goblin" => goblinFightingTile,
                "Skeleton" => skeletonFightingTile,
                "Slime" => slimeFightingTile,
                "DungeonMaster" => dungeonMasterFightingTile,
                _ => null
            };
        }
    }
}
