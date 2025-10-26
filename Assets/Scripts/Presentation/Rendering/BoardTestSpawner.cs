using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Undermarch.Simulation.Grid;
using Undermarch;

public class BoardTestSpawner : MonoBehaviour
{
    public TilemapRendererr tilemapRenderer;
    public int boardWidth = 10;
    public int boardHeight = 10;
    public int entityCount = 3;
    public float tickInterval = 1f; // seconds per tick

    private Board board;
    private List<Entity> entities = new List<Entity>();

    void Start()
    {
        board = new Board(boardWidth, boardHeight);

        // Spawn entities at random positions
        for (int i = 0; i < entityCount; i++)
        {
            TilePos pos;
            do
            {
                pos = new TilePos(Random.Range(0, board.Width), Random.Range(0, board.Height));
            } while (board.HasEntity(pos));

            var charData = new Character(Faction.Hero, pos);
            var entity = new Entity(charData, pos);

            board.TryPlaceEntity(entity, pos);
            entities.Add(entity);
        }

        // Initialize renderer
        tilemapRenderer.Init(board);

        // Start the tick loop
        StartCoroutine(TickLoop());
    }

    private IEnumerator TickLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);

            // Move each entity randomly
            foreach (var e in entities)
            {
                TryRandomMove(e);
            }

            // Redraw board
            tilemapRenderer.RedrawAll();
        }
    }

    private void TryRandomMove(Entity e)
    {
        // 4-directional movement (up, down, left, right)
        var directions = new List<TilePos>
        {
            new TilePos(0, 1),
            new TilePos(0, -1),
            new TilePos(1, 0),
            new TilePos(-1, 0)
        };

        // Shuffle directions
        for (int i = 0; i < directions.Count; i++)
        {
            int j = Random.Range(0, directions.Count);
            var temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        // Try to move in the first valid direction
        foreach (var d in directions)
        {
            var target = new TilePos(e.Position.x + d.x, e.Position.y + d.y);
            if (board.TryMoveEntity(e, target))
                break;
        }
    }
}
