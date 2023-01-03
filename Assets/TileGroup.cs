using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup
{
    public enum GroupType
    {
        Road,
        Building,
    }

    public Vector2 origin;
    public List<Vector2Int> tiles;
    public GroupType type;

    public TileGroup(List<Vector2Int> _tiles, GroupType _type)
    {
        tiles = _tiles;
        type = _type;
    }

    public int NeighboorIndex(Vector2Int position)
    {
        Vector2Int[] neighboors = { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        string bin = "";
        foreach (Vector2Int neighboor in neighboors)
        {
            Vector2Int neighboorTile = neighboor + position;
            if (tiles.Contains(neighboorTile)) bin += '1';
            else bin += '0';
        }
        return System.Convert.ToInt16(bin, 2);
    }
}

public class RoadSystem : TileGroup
{
    public static RoadSystem instance;
    public RoadSystem(List<Vector2Int> _tiles) : base(_tiles, GroupType.Road)
    {
        instance = this;
    }
}