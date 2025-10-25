using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Undermarch.Simulation.Grid;

namespace Undermarch.Presentation.Rendering
{
    public sealed class TilemapRendererr : MonoBehaviour
    {
        [Header("Tilemap Layers (assign in inspector)")]
        public Tilemap groundTilemap;
        public Tilemap wallTilemap;
        public Tilemap interactableTilemap;
        public Tilemap entityTilemap;
        public Tilemap effectsTilemap;

        [Header("Tile Assets (temporary placeholders)")]
        public TileBase groundTile;
        public TileBase wallTile;
        public TileBase interactableTile;
        public TileBase entityTile;
        public TileBase effectTile;

        [Header("Test Walls (for debugging)")]
        public List<TilePosSerializable> testWalls; // assign some in inspector

        private Board _board;

        /// <summary>
        /// Initialize with a simulation board.
        /// </summary>
        public void Init(Board board)
        {
            _board = board;
            RedrawAll();
        }

        /// <summary>
        /// Redraw the whole tilemap.
        /// </summary>
        public void RedrawAll()
        {
            if (_board == null) return;

            ClearAll();

            for (int y = 0; y < _board.Height; y++)
            {
                for (int x = 0; x < _board.Width; x++)
                {
                    var pos = new TilePos(x, y);
                    var cellPos = new Vector3Int(x, y, 0);

                    // draw ground always
                    if (groundTilemap != null && groundTile != null)
                        groundTilemap.SetTile(cellPos, groundTile);

                    // draw wall if present
                    if (HasWall(pos) && wallTilemap != null && wallTile != null)
                        wallTilemap.SetTile(cellPos, wallTile);
                }
            }
        }

        /// <summary>
        /// Clear all tilemaps.
        /// </summary>
        private void ClearAll()
        {
            if (groundTilemap != null) groundTilemap.ClearAllTiles();
            if (wallTilemap != null) wallTilemap.ClearAllTiles();
            if (interactableTilemap != null) interactableTilemap.ClearAllTiles();
            if (entityTilemap != null) entityTilemap.ClearAllTiles();
            if (effectsTilemap != null) effectsTilemap.ClearAllTiles();
        }

        /// <summary>
        /// Temporary test wall check using inspector list.
        /// </summary>
        private bool HasWall(TilePos pos)
        {
            if (testWalls == null) return false;

            foreach (var t in testWalls)
            {
                if (t.x == pos.x && t.y == pos.y)
                    return true;
            }

            return false;
        }
    }
}
