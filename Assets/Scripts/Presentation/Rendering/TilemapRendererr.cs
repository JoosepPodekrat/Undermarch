using UnityEngine;
using UnityEngine.Tilemaps;
using Undermarch.Simulation.Grid;

public class TilemapRendererr : MonoBehaviour
{
    public Tilemap groundTilemap, wallTilemap, interactableTilemap, entityTilemap, effectsTilemap;
    public TileBase groundTile, wallTile, interactableTile, entityTile, effectTile;

    private Board _board;

    public void Init(Board board)
    {
        _board = board;
        RedrawAll();
    }

    public void RedrawAll()
    {
        if (_board == null) return;

        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        interactableTilemap.ClearAllTiles();
        entityTilemap.ClearAllTiles();
        effectsTilemap.ClearAllTiles();

        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                var pos = new TilePos(x, y);
                var cell = new Vector3Int(x, y, 0);

                groundTilemap.SetTile(cell, groundTile);
                if (_board.HasWallAt(pos)) wallTilemap.SetTile(cell, wallTile);
                if (_board.HasEntity(pos)) entityTilemap.SetTile(cell, entityTile);
                if (_board.HasInteractable(pos)) interactableTilemap.SetTile(cell, interactableTile);
                if (_board.GetEffects(pos).Count > 0) effectsTilemap.SetTile(cell, effectTile);
            }
        }
    }
}
