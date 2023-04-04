using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour, IInspectable
{
    [SerializeField] Car carPrefab;

    public string FirstName;
    public string Surname;
    public int Age;
    public float Health;

    // Routine
    public Building currentLocation;
    public Building household;
    public Workplace workplace;

    Car currentCar;

    SpriteRenderer spriteRenderer;
    public enum ViralState {
        Recovered, Uninfected, Infected, Contagious, Symptomatic
    } 

    Virus _virus = null;
    float timeOfInfection;
    public ViralState currentState = ViralState.Uninfected;
    Virus.CriticalTimes virusCriticalTimes;
    public Virus Virus {
        get => _virus;
        set
        {
            if (_virus != null) return;
            timeOfInfection = GameTime.instance.hoursEllapsed;
            currentState = ViralState.Infected;
            _virus = value;
            virusCriticalTimes = _virus.FindCriticalTimes(1f);
        }
    }
    float TimeSinceInfection {
        get => GameTime.instance.hoursEllapsed - timeOfInfection;
    }

    public static Agent CreateNewAgent(TownAgentManager manager, string _firstName, string _surname, int _age, Building _household)
    {
        Agent agent = Instantiate(manager.agentPrefab).GetComponent<Agent>();
        agent.FirstName = _firstName;
        agent.Surname = _surname;
        agent.Age = _age;
        agent.household = _household;
        agent.name = $"{agent.FirstName} {agent.Surname}";
        manager.agents.Add(agent);

        agent.currentLocation = agent.household;
        agent.transform.position = (Vector3Int)agent.currentLocation.tiles[Random.Range(0, agent.currentLocation.tiles.Count)];
        agent.transform.position += new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));

        agent.Health = 0.5f + Random.Range(0.5f, 1.5f) - (agent.Age / 70f);

        return agent;
    }

    // Random walk
    public Vector3 velocity = new();
    float angle;
    public float jitter = 0.1f;

    // Measures
    public bool socialDistancing = false;
    public float socialDistance = 1f;

    private void Awake()
    {
        angle = Random.Range(0, 2 * Mathf.PI); // Start with a random angle of movement

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Follow Routine
        if (workplace != null) { // If has workplace
            if (currentLocation == household && GameTime.TimeOfDay - Health >= workplace.GetOpeningTimes() && GameTime.TimeOfDay - Health < workplace.GetClosingTimes())
            {
                // Go to work
                PathFollow(currentLocation.origin, workplace.origin);
                currentLocation = workplace;
            } else if (currentLocation == workplace && GameTime.TimeOfDay - Health >= workplace.GetClosingTimes())
            {
                // Go home
                PathFollow(currentLocation.origin, household.origin);
                currentLocation = household;
            }
        }

        
        if (currentCar == null) // If not in car
        {
            // Random Walk
            angle += Random.Range(-1f, 1f); // Nudge the angle by a random amount
            velocity += new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * jitter; // Accelerate the agent in that direction
            velocity.Normalize();

            velocity *= 1f + (0.9f*GameTime.SunHeight);
            // Social Distancing
            if (socialDistancing)
                foreach (Agent agent in Virus.allAgents)
                    if (agent != this && agent.currentLocation == currentLocation)
                    {
                        Vector3 delta = transform.position - agent.transform.position;
                        if (delta.sqrMagnitude < socialDistance * socialDistance)
                            velocity += delta.normalized * (socialDistance - delta.magnitude) * 2f;
                    }


            // Move into their current building
            Vector3 predictedPos = transform.position + velocity * GameTime.DeltaTime;
            if (!currentLocation.tiles.Contains(TownSystem.instance.townRenderer.PositionToTile(predictedPos)))
                velocity = (((Vector3)(Vector2)currentLocation.tiles[Random.Range(0, currentLocation.tiles.Count)]) - transform.position) * 2f;
            

            // Update the position
            transform.position += velocity * GameTime.DeltaTime;
        }


        // Update state
        if (Virus != null) { 
            if (TimeSinceInfection >= virusCriticalTimes.uncontagiousTime)
                currentState = ViralState.Recovered;
            else if (TimeSinceInfection >= virusCriticalTimes.unsymptomaticTime)
                currentState = ViralState.Contagious;
            else if (TimeSinceInfection >= virusCriticalTimes.symptomaticTime)
                currentState = ViralState.Symptomatic;
            else if (TimeSinceInfection >= virusCriticalTimes.contagiousTime)
                currentState = ViralState.Contagious;
        }

        // Spread virus if contagious
        if (currentState >= ViralState.Contagious) {
            Virus.SpreadVirus(this, GameTime.DeltaTime);
        }

        // Change colour based on state
        switch (currentState)
        {
            case ViralState.Recovered:
                spriteRenderer.color = new(0.27f, 0.26f, 0.27f);
                break;
            case ViralState.Infected:
            case ViralState.Symptomatic:
            case ViralState.Contagious:
                spriteRenderer.color = new(0.96f, 0.4f, 0.33f);
                break;
            case ViralState.Uninfected:
                spriteRenderer.color = new(1, 1, 1);
                break;
        }
    }

    public void PathFollow(Vector2Int origin, Vector2Int destination)
    {
        currentCar = Instantiate(carPrefab);
        currentCar.MakeJouney(origin, destination, DismountCar);

        transform.position = currentCar.transform.position;
        transform.SetParent(currentCar.transform, true);
    }

    public void DismountCar()
    {
        transform.SetParent(null, true);
    }

    private void OnMouseUp()
    {
        Inspector.instance.Inspect(this);
        Inspector.instance.SetVisibility(true);
    }

    public Dictionary<string, string> InspectionData()
    {
        Dictionary<string, string> inspectionData = new();
        inspectionData["title"] = $"{FirstName} {Surname}";
        inspectionData["body"] = $"Age: {Age}\n Health: {Health*50:0}%\nStatus: {currentState}\n{(workplace != null ? workplace.name : "Unemployed")}";
        return inspectionData;
    }
}
