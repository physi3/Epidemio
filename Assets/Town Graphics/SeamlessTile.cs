using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Seamless Tile", menuName = "Town/SeamlessTile")]
public class SeamlessTile : ScriptableObject
{
    [System.Serializable]
    public struct RotationStruct
    {
        public int spriteIndex;
        public float rot;
        [HideInInspector] public Sprite sprite;
    }
    [SerializeField] Sprite[] tiles;
    [SerializeField] List<RotationStruct> rotationStructure;

    public RotationStruct GetSpriteInfo(int index)
    {
        RotationStruct s = rotationStructure[index];
        s.sprite = tiles[s.spriteIndex];
        return s;
    }
}
