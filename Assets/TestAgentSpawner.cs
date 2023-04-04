using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgentSpawner : MonoBehaviour
{
    [SerializeField] GameObject agentPrefab;
    [SerializeField] int number;
    [SerializeField] VirusData virusData;
    [SerializeField] SIRGraph SIRGraph1;
    [SerializeField] SIRGraph SIRGraph2;

    Virus virus;
    private void Start()
    {
        List<Agent> agents = new();
        for (int i = 0; i < number; i++)
        {
            Agent agent = Instantiate(agentPrefab).GetComponent<Agent>();

            // Give the agent a random position and define its bounding box
            agent.transform.position = transform.position + new Vector3(
                Random.Range(-transform.lossyScale.x / 2.1f, transform.lossyScale.x / 2.1f), 
                Random.Range(-transform.lossyScale.y / 2.1f, transform.lossyScale.y / 2.1f), -1f);
            //agent.boundingBody = GetComponent<Rigidbody2D>();

            agents.Add(agent);
        }

        agents[0].Virus = virus = new(virusData, agents);


        SIRGraph1.unitFrequency = SIRGraph2.unitFrequency = new Vector2(0, number);
    }

    private void Update()
    {
        SIRGraph1.AddValue(virus.TotalInfected, true);
        SIRGraph2.AddValue(virus.TotalImmune, true);
    }
}
