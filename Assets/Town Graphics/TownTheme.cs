using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileGroup;

[CreateAssetMenu(fileName = "New Town Theme", menuName = "Town/TownTheme")]
public class TownTheme : ScriptableObject
{
    public TileSprite EmptyTile;
    [SerializeField] DynamicTile BuildingTile;
    [SerializeField] DynamicTile RoadTile;
    [SerializeField] DynamicTile HousingTile;

    public DynamicTile FindTile(GroupType type)
    {
        return type switch
        {
            GroupType.Road => RoadTile,
            GroupType.Workplace => BuildingTile,
            GroupType.Housing => HousingTile,
            GroupType.TownHall => BuildingTile,
            _ => default,
        };
    }
}
