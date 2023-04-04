using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGroup
{
    public enum GroupType
    {
        Road,
        Workplace,
        Housing,
        TownHall,
    }

    public Vector2Int origin;
    public List<Vector2Int> tiles;
    public GroupType type;

    public TileGroup(List<Vector2Int> _tiles, GroupType _type)
    {
        tiles = _tiles;
        type = _type;
    }

    public int NeighbourIndex(Vector2Int position)
    {
        Vector2Int[] neighbours = { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        string bin = "";
        foreach (Vector2Int neighbour in neighbours)
        {
            Vector2Int neighbourTile = neighbour + position;
            if (tiles.Contains(neighbourTile)) bin += '1';
            else bin += '0';
        }
        return System.Convert.ToInt16(bin, 2);
    }

    public void Highlight(bool on)
    {
        foreach (Vector2Int tile in tiles)
            TownSystem.instance.townRenderer.GetTileSprite(tile).Highlight(on);
    }

    public static TileGroup Construct(List<Vector2Int> _tiles, GroupType _type)
    {
        return _type switch
        {
            GroupType.Road => new RoadSystem(_tiles),
            GroupType.Workplace => new Workplace(_tiles),
            GroupType.Housing => new Building(_tiles, _type),
            GroupType.TownHall => new Building(_tiles, _type),
            _ => default,
        };
    }
}

public class RoadSystem : TileGroup
{
    public static RoadSystem instance;
    private Dictionary<Vector2Int, List<Vector2Int>> adjacencies;
    public RoadSystem(List<Vector2Int> _tiles) : base(_tiles, GroupType.Road)
    {
        instance = this;

        adjacencies = new();
        foreach (Vector2Int tile in tiles) {
            adjacencies[tile] = new();
            foreach (Vector2Int possibleNeighbour in tiles)
                if (Vector2Int.Distance(tile, possibleNeighbour) == 1)
                    adjacencies[tile].Add(possibleNeighbour);
        }

    }

    public List<Vector2Int> FindShortestPath(Vector2Int origin, Vector2Int destination) {
        Dictionary<Vector2Int, int> distanceDict = new();
        List<Vector2Int> unvisitedTiles = new();
        Dictionary<Vector2Int, Vector2Int?> parents = new();
        foreach (Vector2Int tile in tiles) {
            distanceDict[tile] = int.MaxValue;
            unvisitedTiles.Add(tile);
            parents[tile] = null;
        }

        distanceDict[origin] = 0;

        while (unvisitedTiles.Contains(destination)){
            Vector2Int selectedTile = Vector2Int.zero;
            int selectedDistance = int.MaxValue;
            foreach (Vector2Int tile in unvisitedTiles)
                if (distanceDict[tile] < selectedDistance)
                    selectedTile = tile;
            unvisitedTiles.Remove(selectedTile);
            
            foreach (Vector2Int neighbour in adjacencies[selectedTile])
            {
                int newDistance = selectedDistance + 1;
                if (newDistance < distanceDict[neighbour]) {
                    distanceDict[neighbour] = newDistance;
                    parents[neighbour] = selectedTile;
                }
            }
        }

        Vector2Int nextTile = destination;
        List<Vector2Int> path = new() { destination };

        while (nextTile != origin) {
            nextTile = parents[nextTile].Value;
            path.Add(nextTile);
        }
        path.Reverse();

        return path;
    }
}

public class Building : TileGroup, IInspectable
{
    public string name;

    public Building(List<Vector2Int> _tiles, GroupType _type) : base(_tiles, _type) {
        foreach (Vector2Int rtile in RoadSystem.instance.tiles)
            foreach (Vector2Int btile in tiles)
                if (Vector2Int.Distance(rtile, btile) == 1) {
                    origin = rtile;
                    return;
                }
                    
    }

    public virtual Dictionary<string, string> InspectionData()
    {
        Dictionary<string, string> inspectionData = new();
        inspectionData["title"] = name;
        inspectionData["body"] = $"Residents: {tiles.Count}";
        return inspectionData;
    }
}

public class Workplace : Building
{
    public enum WorkplaceType
    {
        Office,
        School,
        Shop,
    }

    WorkplaceType _workplaceType;
    public int vacancies;

    public WorkplaceType workplaceType {
        get => _workplaceType;
         set {
            _workplaceType = value;
            GenerateName();
        }
    }

    public override Dictionary<string, string> InspectionData()
    {
        Dictionary<string, string> inspectionData = new();
        inspectionData["title"] = name;
        inspectionData["body"] = $"{Mathf.FloorToInt(GetOpeningTimes()):00}:{60*(GetOpeningTimes()%1):00} to {Mathf.FloorToInt(GetClosingTimes()):00}:{60 * (GetClosingTimes() % 1):00}";
        return inspectionData;
    }

    public Workplace(List<Vector2Int> _tiles) : base(_tiles, GroupType.Workplace)
    {
    }

    public void GenerateName()
    {
        name = workplaceType switch
        {
            WorkplaceType.Office => TownAgentManager.instance.OfficeNames.GenerateName(),
            WorkplaceType.School => TownAgentManager.instance.SchoolNames.GenerateName(),
            WorkplaceType.Shop => TownAgentManager.instance.ShopNames.GenerateName(),
            _ => throw new System.NotImplementedException(),
        };
    }

    public float GetOpeningTimes()
    {
        return workplaceType switch
        {
            WorkplaceType.Office => 8f,
            WorkplaceType.School => 9f,
            WorkplaceType.Shop => 10f,
            _ => throw new System.NotImplementedException(),
        };
    }

    public float GetClosingTimes()
    {
        return workplaceType switch
        {
            WorkplaceType.Office => 17f,
            WorkplaceType.School => 15.25f,
            WorkplaceType.Shop => 22.5f,
            _ => throw new System.NotImplementedException(),
        };
    }
}