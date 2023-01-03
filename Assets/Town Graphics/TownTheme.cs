using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Town Theme", menuName = "Town/TownTheme")]
public class TownTheme : ScriptableObject
{
    [SerializeField] public TileSprite EmptyTile;
    [SerializeField] public SeamlessTile BuildingTile;
    [SerializeField] public SeamlessTile RoadTile;
}
