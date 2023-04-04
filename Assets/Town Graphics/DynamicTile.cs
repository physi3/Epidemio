using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seamless Tile", menuName = "Town/SeamlessTile")]
public class DynamicTile : ScriptableObject
{
    [System.Serializable]
    public struct DynamicTileType
    {
        public int spriteIndex;
        public float rot;
        [HideInInspector] public Sprite sprite;
    }
    [SerializeField] Sprite[] tiles;
    [SerializeField] List<DynamicTileType> rotationStructure;

    public DynamicTileType GetSpriteInfo(int index)
    {
        DynamicTileType s = rotationStructure[index];
        s.sprite = tiles[s.spriteIndex];
        return s;
    }
}
