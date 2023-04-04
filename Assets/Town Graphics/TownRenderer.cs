using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownRenderer
{
    TownSystem town;
    List<List<TileSprite>> townSprites;

    public TownRenderer(TownSystem _town) {
        town = _town;
    }

    public void Render() {
        foreach (TileGroup tileGroup in town.tileGroups)
        {
            foreach (Vector2Int pos in tileGroup.tiles)
            {
                DynamicTile seamlessTile = town.theme.FindTile(tileGroup.type);
                townSprites[pos.y][pos.x].Set(seamlessTile.GetSpriteInfo(tileGroup.NeighbourIndex(pos)));
            }
        }
    }

    public void GenerateSprites()
    {
        townSprites = new(town.mapSize.y);
        for (int y = 0; y < town.mapSize.y; y++) {
            townSprites.Add(new(town.mapSize.x));
            for (int x = 0; x < town.mapSize.x; x++) {
                townSprites[y].Add(Object.Instantiate(town.theme.EmptyTile, new Vector3(x, y), Quaternion.identity, town.transform));
            }
        }
    }

    public TileSprite GetTileSprite(Vector2Int position)
    {
        return townSprites[position.y][position.x];
    }

    public Vector2Int PositionToTile(Vector3 position)
    {
        return new(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }
}
