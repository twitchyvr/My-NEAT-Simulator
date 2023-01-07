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
//using System;
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


public class PopulationManager : MonoBehaviour {

    #region Settable Variables
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private int _populationSize = 50;
    [SerializeField] private float _elapsed = 0;
    [SerializeField] private int _trialTime = 10;
    [SerializeField] private int _generation = 1;
    [SerializeField] private List<GameObject> _agents = new List<GameObject>();
    [SerializeField] private GameObject _target;
    [SerializeField] private bool _allAgentsDead = false;
    [SerializeField] private float _elapsedSinceLastBest = 0;
    [SerializeField] private float _timeBetweenBest = 5;
    [SerializeField] private int _bestGenome = 0;
    [SerializeField] private int _bestFitness = 0;
    [SerializeField] private int _bestTime = 0;
    [SerializeField] private int _bestDistance = 0;
    [SerializeField] private int _bestScore = 0;
    [SerializeField] private int _bestGeneration = 0;
    [SerializeField] private int _bestTrial = 0;
    [SerializeField] private int _bestTrialTime = 0;
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
    public GameObject Target { get => _target; set => _target = value; }
    public bool AllAgentsDead { get => _allAgentsDead; set => _allAgentsDead = value; }
    public float ElapsedSinceLastBest { get => _elapsedSinceLastBest; set => _elapsedSinceLastBest = value; }
    public float TimeBetweenBest { get => _timeBetweenBest; set => _timeBetweenBest = value; }
    public int BestGenome { get => _bestGenome; set => _bestGenome = value; }
    public int BestFitness { get => _bestFitness; set => _bestFitness = value; }
    public int BestTime { get => _bestTime; set => _bestTime = value; }
    public int BestDistance { get => _bestDistance; set => _bestDistance = value; }
    public int BestScore { get => _bestScore; set => _bestScore = value; }
    public int BestGeneration { get => _bestGeneration; set => _bestGeneration = value; }
    public int BestTrial { get => _bestTrial; set => _bestTrial = value; }
    public int BestTrialTime { get => _bestTrialTime; set => _bestTrialTime = value; }
        #endregion

    #region Init
    // Awake is called before Start.
    // It is used to initialize any variables or game state before the game starts.
    // Awake is called only once during the lifetime of the script instance.
    protected void Awake() {

    }
    // Start is called before the first frame update.
    protected void Start() {
        // This method is used to initialize any variables or game state before the game starts.
        // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
        // Start is called only once during the lifetime of the script instance.
        for (int i = 0; i < PopulationSize; i++) {
            Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-2, 2), 0, this.transform.position.z + Random.Range(-2, 2));
            GameObject agent = Instantiate(AgentPrefab, startingPos, this.transform.rotation);
            agent.GetComponent<HumanAgent>().Init();
            agent.GetComponent<HumanAgent>().MyManager = this;
            agent.GetComponent<HumanAgent>().MyBrain = agent.GetComponent<NeuralNetwork>();
            agent.GetComponent<HumanAgent>().MyTarget = Target;
            agent.GetComponent<HumanAgent>().MyNumber = i;
            Agents.Add(agent);
        }
    }

    #endregion

    #region Loop
    // Update is called once per frame.
    protected void Update() {
        // This is the main game loop method.

    }
    // FixedUpdate is called once per physics step.
    protected void FixedUpdate() {
        // This method is used to update physics related variables.

    }
    // LateUpdate is called once per frame after all Update functions have been called, but before rendering.
    protected void LateUpdate() {
        // Called after all Update functions. It is used to order script execution.
        // A Camera or other tracking code should go here for example.

    }

    #endregion

    #region Methods
    // Your custom methods go here

    #endregion
}
