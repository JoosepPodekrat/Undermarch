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
    public float tickInterval = 1f; 

    private Board board;
    private List<Entity> entities = new List<Entity>();

    void Start()
    {
        board = new Board(boardWidth, boardHeight);

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

        tilemapRenderer.Init(board);
        StartCoroutine(TickLoop());
    }

    private IEnumerator TickLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            foreach (var e in entities)
            {
                TryRandomMove(e);
            }
            tilemapRenderer.RedrawAll();
        }
    }

    private void TryRandomMove(Entity e)
    {
        var directions = new List<TilePos>
        {
            new TilePos(0, 1),
            new TilePos(0, -1),
            new TilePos(1, 0),
            new TilePos(-1, 0)
        };
        for (int i = 0; i < directions.Count; i++)
        {
            int j = Random.Range(0, directions.Count);
            var temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        foreach (var d in directions)
        {
            var target = new TilePos(e.Position.x + d.x, e.Position.y + d.y);
            if (board.TryMoveEntity(e, target))
                break;
        }
    }
}
