using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus
{
    readonly float infectionRadius;
    readonly float infectionProbability;
    readonly StateCurve contagiousStateCurve;
    readonly StateCurve symptomaticStateCurve;

    List<Agent> susceptibleAgents;
    public static List<Agent> allAgents;

    public int TotalInfected {
        get {
            int total = 0;
            foreach (Agent agent in allAgents)
                total += agent.currentState >= Agent.ViralState.Infected ? 1 : 0;
            return total;
        }
    }

    public int TotalImmune
    {
        get
        {
            int total = 0;
            foreach (Agent agent in allAgents)
                total += agent.currentState >= Agent.ViralState.Uninfected ? 1 : 0;
            return total;
        }
    }

    public Virus(VirusData virusData, List<Agent> agents) {
        infectionRadius = virusData.infectionRadius;
        infectionProbability = virusData.infectionProbability;
        contagiousStateCurve = new(virusData.contagiousStateCurve);
        symptomaticStateCurve = new(virusData.symptomaticStateCurve);

        susceptibleAgents = new(agents);
        allAgents = new(agents);
    }

    public void SpreadVirus(Agent infectedAgent, float deltaTime)
    {
        List<Agent> localUninfectedAgents = new(susceptibleAgents);
        foreach (Agent uninfectedAgent in localUninfectedAgents)
        {
            if (uninfectedAgent.currentLocation != infectedAgent.currentLocation) continue; // This can be sped up
            if ((uninfectedAgent.transform.position - infectedAgent.transform.position).sqrMagnitude < infectionRadius * infectionRadius)
            { // If within range of infection
                float probability = 1 - Mathf.Pow(1 - infectionProbability, deltaTime);
                // Double probability of infection if agent is symptomatic e.g. coughing
                probability *= infectedAgent.currentState == Agent.ViralState.Symptomatic ? 2f : 1f;
                if (Random.value <= probability)
                    InfectAgent(uninfectedAgent);
            }
        }
            
    }

    public void InfectAgent(Agent agent) {
        agent.Virus = this;
        susceptibleAgents.Remove(agent);
    }

    public CriticalTimes FindCriticalTimes(float health)
    {
        CriticalTimes criticalTimes;
        contagiousStateCurve.StateBounds(health, out criticalTimes.contagiousTime, out criticalTimes.uncontagiousTime);
        symptomaticStateCurve.StateBounds(health, out criticalTimes.symptomaticTime, out criticalTimes.unsymptomaticTime);
        return criticalTimes;
    }

    public struct CriticalTimes
    {
        public float contagiousTime;
        public float symptomaticTime;
        public float unsymptomaticTime;
        public float uncontagiousTime;
    }
}

public class StateCurve
{
    readonly float h; // The maximum of the state curve
    readonly float u; // The output at t = 0
    readonly float T; // The time taken for the curve to equal u for the second time

    public StateCurve(VirusData.StateCurveData stateCurveData) {
        h = stateCurveData.height;
        u = stateCurveData.initialOut;
        T = stateCurveData.period;
    }
    public void StateBounds(float S, out float lowerBound, out float upperBound)
    {
        float normalPeriod = Mathf.Sqrt(Mathf.Log(S) - Mathf.Log(h) / (Mathf.Log(u) - Mathf.Log(h)));
        upperBound = T * (1 + normalPeriod) / 2;
        lowerBound = T * (1 - normalPeriod) / 2;
    }
}

[System.Serializable]
public struct VirusData
{
    public float infectionRadius;
    public float infectionProbability;
    public StateCurveData contagiousStateCurve;
    public StateCurveData symptomaticStateCurve;

    [System.Serializable]
    public struct StateCurveData
    {
        public float height;
        public float initialOut;
        public float period;
    }
}
