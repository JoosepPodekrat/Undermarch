using UnityEngine;
using Undermarch.Presentation.Rendering;  
using Undermarch.Simulation.Grid;

public class BoardTestSpawner : MonoBehaviour
{
    public TilemapRendererr tilemapRenderer;

    void Start()
    {
        // Example: create a 10x10 board
        var board = new Board(10, 10);

        // For testing, mark some walls
        // (you can later replace with real simulation data)
        // This assumes Board will expose some method to set walls
        // For now, the HasWall() method in TilemapRenderer always returns false
        // We'll adjust that in a second

        tilemapRenderer.Init(board);
    }
}
