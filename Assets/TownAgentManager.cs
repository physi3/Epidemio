using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownAgentManager : MonoBehaviour
{
    public static TownAgentManager instance;
    [SerializeField] VirusData virusData;

    Virus virus;

    [SerializeField] public GameObject agentPrefab;
    [SerializeField] TextAsset surnamesFile;
    [SerializeField] TextAsset firstNamesFile;

    [SerializeField] public WorkplaceNameGenerator SchoolNames;
    [SerializeField] public WorkplaceNameGenerator ShopNames;
    [SerializeField] public WorkplaceNameGenerator OfficeNames;

    [SerializeField] SIRGraph SIRGraph1;
    [SerializeField] SIRGraph SIRGraph2;

    string[] surnames;
    string[] firstnames;

    [HideInInspector] public List<Agent> agents = new();

    List<Building> housing = new();
    List<Workplace> workplaces = new();

    private void Awake()
    {
        instance = this;
        surnames = surnamesFile.text.Split("\n");
        firstnames = firstNamesFile.text.Split("\n");
    }

    private void Update()
    {
        if (GameTime.instance.hoursEllapsed >= SIRGraph1.values.Count) {
            SIRGraph1.AddValue(virus.TotalInfected, true);
            SIRGraph2.AddValue(virus.TotalImmune, true);
        }
    }
    public void GenerateAgents(TownSystem townSystem) {
        int children = 0;
        int workers = 0;

        foreach (TileGroup tileGroup in townSystem.tileGroups)
        {
            if (tileGroup.type != TileGroup.GroupType.Housing) continue;
            Building building = (Building) tileGroup;
            housing.Add(building);
            string HouseholdName = RandomSurname();
            building.name = $"{HouseholdName} Household";

            for (int i = 0; i < building.tiles.Count; i++)
            {
                string firstName = RandomFirstName();
                int age = i >= 2 ? Random.Range(4, 35) : Random.Range(23, 78); // If 2 adults, allow children
                agents.Add(Agent.CreateNewAgent(this, firstName, HouseholdName, age, building));

                // Count ages to determine school to workplace proportion
                if (age <= 18)
                    children++;
                else if (age <= 66)
                    workers++;
            }
        }

        // Count total cells avaliable for workplaces / schools
        int totalWorkplaceCells = 0;
        foreach (TileGroup tileGroup in townSystem.tileGroups)
            if (tileGroup.type == TileGroup.GroupType.Workplace)
            {
                workplaces.Add((Workplace)tileGroup);
                totalWorkplaceCells += tileGroup.tiles.Count;
            }

        // Shuffle workplaces
        for (int n = workplaces.Count - 1; n > 1; --n) {
            int k = Random.Range(0, n + 1);
            (workplaces[n], workplaces[k]) = (workplaces[k], workplaces[n]);
        }

        // Assign buildings 
        int schoolCellsRequired = totalWorkplaceCells * children / (workers + children);
        int shopCellsRequired = Mathf.RoundToInt (workers * 0.2f);
        int totalWorkplaceTiles = 0;
        foreach (Workplace workplace in workplaces)
        {
            totalWorkplaceTiles += workplace.tiles.Count;

            if (schoolCellsRequired > 0) {
                workplace.workplaceType = Workplace.WorkplaceType.School;
                schoolCellsRequired -= workplace.tiles.Count;
            } else if (shopCellsRequired > 0) {
                workplace.workplaceType = Workplace.WorkplaceType.Shop;
                shopCellsRequired -= workplace.tiles.Count;
            } else {
                workplace.workplaceType = Workplace.WorkplaceType.Office;
            }
        }

        foreach (Workplace workplace in workplaces) { 
            workplace.vacancies = workplace.tiles.Count * (workers + children) / totalWorkplaceTiles;
        }

        // Shuffle agents
        for (int n = agents.Count - 1; n > 1; --n) {
            int k = Random.Range(0, n + 1);
            (agents[n], agents[k]) = (agents[k], agents[n]);
        }

        // Assign agents workplaces
        foreach (Agent agent in agents)
        {
            bool CHILD = agent.Age <= 18;
            bool WORKER = agent.Age > 16 && agent.Age <= 66;
            foreach (Workplace workplace in workplaces)
            {
                if (((CHILD && workplace.workplaceType == Workplace.WorkplaceType.School) || 
                    (WORKER && workplace.workplaceType != Workplace.WorkplaceType.School))
                    && workplace.vacancies > 0)
                {
                    workplace.vacancies--;
                    agent.workplace = workplace;
                    goto WorkplaceAssigned;
                }
            }
            WorkplaceAssigned:;
        }

        agents[Random.Range(0, agents.Count)].Virus = virus = new(virusData, agents);
        SIRGraph1.unitFrequency = SIRGraph2.unitFrequency = new Vector2(0, agents.Count);
    }
    
    string RandomSurname()
    {
        return surnames[Random.Range(0, surnames.Length)].TrimEnd();
    }
    string RandomFirstName()
    {
        return firstnames[Random.Range(0, surnames.Length)].TrimEnd();
    }
}

[System.Serializable]
public class WorkplaceNameGenerator
{
    int index = 0;
    public List<string> names = new();

    public string GenerateName()
    {
        if (index >= names.Count) index = 0;
        return names[index++];
    }
}