using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownSystem : MonoBehaviour
{
    public static TownSystem instance;
    public List<TileGroup> tileGroups = new();
    [HideInInspector] public Vector2Int mapSize;
    public TownRenderer townRenderer;
    TileGroup selectedGroup;

    [SerializeField] public TownTheme theme;
    [SerializeField] public TextAsset townFile;

    private void Awake()
    {
        instance = this;
        mapSize = new Vector2Int(32, 24);

        LoadFromFile(townFile);

        townRenderer = new(this);
    }

    private void Start()
    {

        townRenderer.GenerateSprites();
        TownAgentManager.instance.GenerateAgents(this);
        townRenderer.Render();

        /*
        List<Vector2Int> path = RoadSystem.instance.FindShortestPath(new(1, 6), new(15, 3));
        foreach (Vector2Int tile in path)
            townRenderer.GetTileSprite(tile).Highlight();
        */
    }

    private void LoadFromFile(TextAsset file)
    {
        List<Vector2Int> tiles = new();
        TileGroup.GroupType type = 0;

        bool newGroup = true;
        
        foreach (string line in file.text.Split("\n"))
        {
            if (string.IsNullOrEmpty(line))
                break; // Finish if we've reached the end of the file

            if (line[0] == 13) { // If reached the end of the group in the file
                tileGroups.Add(TileGroup.Construct(tiles, type)); // Add a new group to the system
                newGroup = true;
                continue;
            }

            if (newGroup) { // If start of a new group
                type = (TileGroup.GroupType)int.Parse(line); // Determine the group type
                tiles = new(); // Reset the current tiles list
                newGroup = false;
                continue;
            }

            // Determine the x and y given from the file and create a new tile
            int x = int.Parse(line.Split(", ")[0]);
            int y = int.Parse(line.Split(", ")[1]);

            tiles.Add(new Vector2Int(x, y));
        }
    }

    public TileGroup PositionToGroup(Vector3 position)
    {
        Vector2Int vector2Pos = townRenderer.PositionToTile(position);
        foreach (TileGroup group in tileGroups)
            if (group.tiles.Contains(vector2Pos))
                return group;
        return null;
    }

    public void SelectGroup(TileGroup tileGroup)
    {
        if (selectedGroup != null)
            selectedGroup.Highlight(false);

        if (Time.frameCount == Inspector.instance.frameOfLastInspection) {
        } else if (selectedGroup == tileGroup) {
            Inspector.instance.SetVisibility(false);
        } else if (tileGroup is not null and IInspectable) {
            tileGroup.Highlight(true);
            Inspector.instance.SetVisibility(true);
            Inspector.instance.Inspect((IInspectable)tileGroup);
        } else { 
            Inspector.instance.SetVisibility(false);
        }
        selectedGroup = tileGroup;

    }
}
