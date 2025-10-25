using System;
using UnityEngine;

[Serializable]
public struct TilePosSerializable
{
    public int x;
    public int y;

    public TilePosSerializable(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // convert to simulation TilePos
    public Undermarch.Simulation.Grid.TilePos ToTilePos()
    {
        return new Undermarch.Simulation.Grid.TilePos(x, y);
    }
}
