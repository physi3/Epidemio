using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Set(DynamicTile.DynamicTileType rs)
    {
        spriteRenderer.sprite = rs.sprite;
        transform.localRotation = Quaternion.Euler(0, 0, rs.rot * -180);
        spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
    }
    public void Highlight(bool on)
    {
        spriteRenderer.color = on ? Color.white : new Color(0.5f, 0.5f, 0.5f);
    }
}
