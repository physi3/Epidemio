using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownRenderer
{
    List<List<TileSprite>> townSprites;
    public TownRenderer() {}

    public void Render(TownSystem town) {
        foreach (TileGroup tileGroup in town.tileGroups)
        {
            foreach (Vector2Int pos in tileGroup.tiles)
            {
                SeamlessTile seamlessTile = tileGroup.type == TileGroup.GroupType.Road ? town.theme.RoadTile : town.theme.BuildingTile ;
                townSprites[pos.y][pos.x].Set(seamlessTile.GetSpriteInfo(tileGroup.NeighboorIndex(pos)));
            }
        }
    }

    public void GenerateTown(Vector2Int size)
    {
        townSprites = new(size.y);
        for (int y = 0; y < size.y; y++) {
            townSprites.Add(new(size.x));
            for (int x = 0; x < size.x; x++) {
                townSprites[y].Add(Object.Instantiate(TownSystem.instance.theme.EmptyTile, new Vector3(x, y), Quaternion.identity));
            }
        }
    }
}
