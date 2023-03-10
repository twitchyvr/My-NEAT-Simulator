/* Copyright (c) 2022-2023, Matthew A. Rogers
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Matthew A. Rogers nor the
 *       names of any contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#region Usings
using System;
//using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

[Serializable]
public class PopulationManager : MonoBehaviour
{

    #region Settable Variables
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private int _populationSize = 50;
    [SerializeField] private float _elapsed = 0;
    [SerializeField] private int _trialTime = 10;
    [SerializeField] private int _generation = 1;
    [SerializeField] private List<GameObject> _agents = new();
    [SerializeField] private List<NeuralNetwork> _agentNets = new List<NeuralNetwork>();
    [SerializeField] private List<NeuralNetwork> _newSpeciesNets = new();
    [SerializeField] private bool _allAgentsDead = false;
    [SerializeField] private float _elapsedSinceLastBest = 0;
    [SerializeField] private float _timeBetweenBest = 5;
    [SerializeField] private int _bestGenome = 0;
    [SerializeField] private float _bestFitness = 0;
    [SerializeField] private float _bestTime = 0;
    [SerializeField] private float _bestDistance = 0;
    [SerializeField] private int _bestScore = 0;
    [SerializeField] private int _bestGeneration = 0;
    [SerializeField] private int _bestTrial = 0;
    [SerializeField] private int _bestTrialTime = 0;
    public float GenerationFitness = -1f;
    #endregion

    #region Private Variables
    #endregion

    #region Properties
    public GameObject AgentPrefab { get => _agentPrefab; set => _agentPrefab = value; }
    public int PopulationSize { get => _populationSize; set => _populationSize = value; }
    public float Elapsed { get => _elapsed; set => _elapsed = value; }
    public int TrialTime { get => _trialTime; set => _trialTime = value; }
    public int Generation { get => _generation; set => _generation = value; }
    public List<GameObject> Agents { get => _agents; set => _agents = value; }
    public List<NeuralNetwork> AgentNets { get => _agentNets; set => _agentNets = value; }
    public List<NeuralNetwork> NewSpeciesNets { get => _newSpeciesNets; set => _newSpeciesNets = value; }
    public bool AllAgentsDead { get => _allAgentsDead; set => _allAgentsDead = value; }
    public float ElapsedSinceLastBest { get => _elapsedSinceLastBest; set => _elapsedSinceLastBest = value; }
    public float TimeBetweenBest { get => _timeBetweenBest; set => _timeBetweenBest = value; }
    public int BestGenome { get => _bestGenome; set => _bestGenome = value; }
    public float BestFitness { get => _bestFitness; set => _bestFitness = value; }
    public float BestTime { get => _bestTime; set => _bestTime = value; }
    public float BestDistance { get => _bestDistance; set => _bestDistance = value; }
    public int BestScore { get => _bestScore; set => _bestScore = value; }
    public int BestGeneration { get => _bestGeneration; set => _bestGeneration = value; }
    public int BestTrial { get => _bestTrial; set => _bestTrial = value; }
    public int BestTrialTime { get => _bestTrialTime; set => _bestTrialTime = value; }
    public GameObject Instance;
    public GameObject SelectedAgent;
    #endregion

    #region Init
    protected void Start()
    {
        for (int i = 0; i < PopulationSize; i++)
        {
            Instance = this.gameObject;
            Vector3 startingPos = new(this.transform.position.x + UnityEngine.Random.Range(-2, 2), 0, this.transform.position.z + UnityEngine.Random.Range(-2, 2));
            GameObject agent = Instantiate(AgentPrefab, startingPos, this.transform.rotation);
            agent.name = "Agent " + i;
            agent.GetComponent<HumanAgent>().MyBrain = new NeuralNetwork(agent.GetComponent<HumanAgent>().BrainInputNodesCount, agent.GetComponent<HumanAgent>().BrainOutputNodesCount);
            agent.GetComponent<HumanAgent>().MyManager = this;
            agent.GetComponent<HumanAgent>().MyNumber = i;
            Agents.Add(agent);
        }
    }

    protected void Update()
    {
        if (Agents.Count == 0)
        {
            AllAgentsDead = true;
            Generation++;

            Speciation();
            return;
        }
        GenerationFitness = CalculateGenerationFitness();

        if (SelectedAgent != null)
        {
            Camera.main.transform.position = new Vector3(SelectedAgent.transform.position.x, Camera.main.transform.position.y, SelectedAgent.transform.position.z);
        }

    }

    /// <summary>
    /// This method facilitates the species comparison process.
    /// </summary>
    private void Speciation()
    {
        // Select a random agent from the population of AgentNets, and add it to the NewSpeciesNets list as the first species.
        int randomAgentNum = UnityEngine.Random.Range(0, AgentNets.Count);
        NeuralNetwork randomAgent;
        int speciesNum = 1;

        AgentNets = AgentNets;
        while (AgentNets.Count > 0)
        {
            randomAgent = AgentNets[randomAgentNum];
            NewSpeciesNets.Add(randomAgent);
            AgentNets.Remove(randomAgent);
            for (int i = 0; i < AgentNets.Count; i++)
            {
                if (AgentNets[i] == randomAgent) continue;
                if (AgentNets[i] != randomAgent)
                {
                    if (AgentNets[i].CompareTopologies(randomAgent) == 1f)
                    {
                        NewSpeciesNets.Add(AgentNets[i]);
                        AgentNets.Remove(AgentNets[i]);
                    }
                }
            }
            speciesNum++;
        }

        // Repopulate the population with new agents.
        RepopulateNew();
        // Clear the AgentNets list.
        AgentNets.Clear();
    }


    /// <summary>
    /// This method repopulates the population with new agents.
    /// </summary>
    public void RepopulateNew()
    {
        for (int i = 0; i < PopulationSize; i++)
        {
            Vector3 startingPos = new(this.transform.position.x + UnityEngine.Random.Range(-2, 2), 0, this.transform.position.z + UnityEngine.Random.Range(-2, 2));
            GameObject agent = Instantiate(AgentPrefab, startingPos, this.transform.rotation);
            agent.name = "Agent " + i;
            agent.GetComponent<HumanAgent>().MyBrain = new NeuralNetwork(agent.GetComponent<HumanAgent>().BrainInputNodesCount, agent.GetComponent<HumanAgent>().BrainOutputNodesCount);
            agent.GetComponent<HumanAgent>().MyManager = this;
            agent.GetComponent<HumanAgent>().MyNumber = i;
            agent.GetComponent<HumanAgent>().MyBrain.SpeciesId = Generation;
            Agents.Add(agent);
        }

        if (NewSpeciesNets.Count > 0)
        {
            for (int i = 0; i < NewSpeciesNets.Count; i++)
            {
                if (PopulationSize == 0 || NewSpeciesNets.Count == 0) return;
                int newSpeciesPopSize = 0;
                try { newSpeciesPopSize = PopulationSize / NewSpeciesNets.Count; }
                catch (DivideByZeroException e) { Debug.Log(e); }
                for (int j = 0; j < newSpeciesPopSize; j++)
                {
                    Vector3 startingPos = new(this.transform.position.x + UnityEngine.Random.Range(-2, 2), 0, this.transform.position.z + UnityEngine.Random.Range(-2, 2));
                    GameObject agent = Instantiate(AgentPrefab, startingPos, this.transform.rotation);
                    agent.name = "Agent " + i;
                    agent.GetComponent<HumanAgent>().MyBrain = new NeuralNetwork(agent.GetComponent<HumanAgent>().BrainInputNodesCount, agent.GetComponent<HumanAgent>().BrainOutputNodesCount);
                    agent.GetComponent<HumanAgent>().MyManager = this;
                    agent.GetComponent<HumanAgent>().MyNumber = i;
                    agent.GetComponent<HumanAgent>().MyBrain.SpeciesId = Generation;
                    agent.GetComponent<HumanAgent>().MyBrain = NewSpeciesNets[i];
                    agent.GetComponent<HumanAgent>().MyBrain.Mutate();
                    if (UnityEngine.Random.Range(0, 100) < 5)
                        agent.GetComponent<HumanAgent>().MyBrain.AddANode();
                    if (UnityEngine.Random.Range(0, 100) < 5)
                        agent.GetComponent<HumanAgent>().MyBrain.AddAConnection();
                    Agents.Add(agent);
                    AgentNets.Remove(NewSpeciesNets[i]);
                }
            }
        }

        AllAgentsDead = false;
    }

    public void AgentSelected(GameObject agent)
    {
        SelectedAgent = agent;
    }

    public float CalculateGenerationFitness()
    {
        float total = 0;
        foreach (GameObject agent in Agents)
        {
            if (agent != null)
                total += agent.GetComponent<HumanAgent>().BrainFitness;
        }
        return total / Agents.Count;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(210, 10, 200, 20), "Generation: " + Generation);
        GUI.Label(new Rect(210, 30, 200, 20), "Best Fitness: " + BestFitness);
        GUI.Label(new Rect(210, 50, 200, 20), "Best Generation: " + BestGeneration);
        GUI.Label(new Rect(210, 70, 200, 20), "Best Trial: " + BestTrial);
        GUI.Label(new Rect(210, 90, 200, 20), "Best Trial Time: " + BestTrialTime);
        GUI.Label(new Rect(210, 110, 200, 20), "Best Time: " + BestTime);
        GUI.Label(new Rect(210, 130, 200, 20), "Best Distance: " + BestDistance);
        GUI.Label(new Rect(210, 150, 200, 20), "Best Score: " + BestScore);
    }
    #endregion
}
