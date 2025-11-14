using Undermarch;
using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Entities.Characters.Monsters;
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

        [Header("Tile Assets")]
        public TileBase groundTile;
        public TileBase wallTile;
        public TileBase heroTile;
        public TileBase monsterTile;
        public TileBase dungeonMasterTile;
        public TileBase trapTile;
        public TileBase chestTile;
        public TileBase arrowTile;
        public TileBase poisonCloudTile;
        public TileBase slowZoneTile;
        public TileBase fireZoneTile;
        public TileBase fogZoneTile;

        private Board _board;

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
            RedrawAll();
        }

        void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (_board != null)
            {
                _board.OnBoardChanged -= UpdateTile;
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
            var cellPos = new Vector3Int(pos.x, pos.y, 0);

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
            if (interactable is Trap)
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

            // Entity Layer
            var entity = _board.GetEntityAt(pos);
            if (entity != null)
            {
                TileBase tile = null;
                if (entity is Hero) tile = heroTile;
                else if (entity is Monster) tile = monsterTile;
                else if (entity is DungeonMaster) tile = dungeonMasterTile;
                Debug.Log($"TilemapRenderer: Setting tile for {entity.GetType().Name} at {cellPos}");
                entityTilemap.SetTile(cellPos, tile);
            }
            else
            {
                // Debug.Log($"TilemapRenderer: Clearing entity tile at {cellPos}");
                entityTilemap.SetTile(cellPos, null);
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
        }
    }
}
