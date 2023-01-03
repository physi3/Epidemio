using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSystem : MonoBehaviour
{
    public static TownSystem instance;
    public List<TileGroup> tileGroups = new();
    [SerializeField] Vector2Int mapSize = new Vector2Int(16, 8);
    TownRenderer townRenderer;
    [SerializeField] public TownTheme theme;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        townRenderer = new();
        townRenderer.GenerateTown(mapSize);

        tileGroups.Add(new RoadSystem(new() {
            new(0, 6),
            new(1, 6),
            new(2, 6),
            new(3, 6),
            new(4, 6),
            new(5, 6),
            new(5, 5),
            new(5, 4),
            new(5, 3),
            new(5, 2),
            new(5, 1),
            new(5, 0),
            new(6, 2),
            new(7, 2),
            new(8, 2),
            new(9, 2),
            new(10, 2),
            new(11, 2),
            new(12, 2),
            new(12, 1),
            new(12, 0),
            new(12, 3),
            new(12, 4),
            new(13, 4),
            new(14, 4),
            new(15, 4)
        }));
        tileGroups.Add(new(new() {
            new(8, 3),
            new(8, 4),
            new(9, 3),
            new(9, 4)
        }, TileGroup.GroupType.Building));

        townRenderer.Render(this);
    }
}
