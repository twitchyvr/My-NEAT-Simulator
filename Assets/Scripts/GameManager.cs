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
using System.IO;
using System.Linq;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion


public class GameManager : MonoBehaviour
{
    public static bool IsPaused = false;
    public GameObject SelectedCreature;
    public GameObject FoodPrefab;
    public GameObject Canvas;
    public float FoodSpawnMaxX = 80;
    public float FoodSpawnMaxZ = 80f;
    public int CreatureSpeciesId = 0;
    public float CreatureBrainFitness = 0f;
    public float CreatureAgentFitness = 0f;
    public int FoodCount = 10;
    public float CreatureHealth = 0f;
    public float CreatureAge = 0f;
    public float CreatureEnergy = 0f;
    public string CreatureName = "";

    #region Init
    protected void Awake()
    {
        // Instantiate food prefabs around the map randomly within a specific x and z range.
        for (int i = 0; i < FoodCount; i++)
        {
            Instantiate(FoodPrefab, new Vector3(UnityEngine.Random.Range(-FoodSpawnMaxX, FoodSpawnMaxX), 0, UnityEngine.Random.Range(-FoodSpawnMaxZ, FoodSpawnMaxZ)), Quaternion.identity);
        }
    }
    #endregion

    #region Loop
    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetCreatureInfo();
        }

        // Automatically select the creature with the highest fitness.
        try
        {
            SelectedCreature = GameObject.FindGameObjectsWithTag("Creature").OrderByDescending(x => x.GetComponent<HumanAgent>().MyBrain.Fitness).First();
        }
        catch (Exception)
        {
            SelectedCreature = null;
        }
        if (SelectedCreature == null) return;

        SelectedCreature.GetComponent<HumanAgent>().MyManager.AgentSelected(SelectedCreature);
        if (SelectedCreature.TryGetComponent(out HumanAgent creature))
        {
            CreatureName = SelectedCreature.name;
            CreatureHealth = creature.Health;
            CreatureAge = creature.Age;
            CreatureEnergy = creature.Energy;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectedCreature.GetComponent<HumanAgent>().Save(creature.name + "-CreatureSave.json");
            Debug.Log("Saved Creature: " + creature.name + " to file: " + creature.name + "-CreatureSave.json");

            //SelectedCreature.GetComponent<HumanAgent>().MyBrain.Save(creature.name + "-BrainSave.json");
            //Debug.Log("Saved Brain " + creature.name + " to file: " + creature.name + "-BrainSave.json");
        }
    }

    private void GetCreatureInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name, hit.collider.gameObject);
            if (hit.collider.gameObject.CompareTag("Creature"))
            {
                print("Hit was a Creature.");
                if (hit.collider.gameObject.TryGetComponent(out HumanAgent creature))
                {
                    // When the object is clicked, output this to the console
                    Debug.Log("You selected the " + hit.collider.gameObject.name); // ensure you picked right object

                    SelectedCreature = hit.collider.gameObject;
                    if (SelectedCreature == null) return;
                    SelectedCreature.GetComponent<HumanAgent>().MyManager.AgentSelected(SelectedCreature); // Effectively this makes the selected agent the only one that can move.
                    CreatureName = hit.collider.gameObject.name;
                    CreatureHealth = creature.Health;
                    CreatureAge = creature.Age;
                    CreatureEnergy = creature.Energy;
                    CreatureSpeciesId = creature.SpeciesId;
                    CreatureBrainFitness = creature.MyBrain.Fitness;
                    CreatureAgentFitness = creature.AgentFitness;
                }
            }
        }
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Creature: {CreatureName}");
        GUI.Label(new Rect(10, 25, 300, 20), $"Health: {CreatureHealth}");
        GUI.Label(new Rect(10, 40, 300, 20), $"Age: {CreatureAge}");
        GUI.Label(new Rect(10, 55, 300, 20), $"Energy: {CreatureEnergy}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Species: {CreatureSpeciesId}");
        GUI.Label(new Rect(10, 85, 300, 20), $"BrainFitness: {CreatureBrainFitness}");
        GUI.Label(new Rect(10, 100, 300, 20), $"AgentFitness: {CreatureAgentFitness}");
        int nodePos = 115;
        // Show the selected creature's nodes and connections.
        if (SelectedCreature != null)
        {
            if (SelectedCreature.TryGetComponent(out HumanAgent creature))
            {
                // 1. Create a list of nodes, sorted by their layer
                // 2. Create and position small squares that represent the nodes on the GUI, based on their layer.
                // 3. The input nodes are on the left, and the lowest number starts on top.
                // 4. The output nodes are on the right, and the lowest number starts on top.
                // 5. The bias node is at the bottom of the row of input nodes.
                // 6. The hidden nodes are in the middle, and the lowest number starts on top.
                // 7. Display the values of the nodes on the GUI next to the node squares without overlapping.
                // 8. Display the connections on the GUI as lines
                // 9. Display the node values on the GUI as small text
                // 10. Display the connection weights on the GUI as small text


                // Check how many layers and how many nodes per layer we have
                // Create a list of nodes, sorted by their layer
                // Layers define how many columns are needed on the canvas, and the nodes per layer define how many rows are needed on the column
                // Create a 2D array of nodes
                // Create a 2D array of connection lines

                // Define how many columns we need on the canvas based on the contents of their node and connection arrays
                // Check how many layers and how many nodes per layer we have
                // Define how many columns we need on the canvas, and how many rows we need in each column
                // Then we use that information to position and draw the nodes on the screen, and label them
                // Then we go through the connection array and draw the lines between the nodes - and color code them for clarity
                // We do not want to normally display disabled connections but the option to do so should be available

                int currentLayerNumber = 0;
                int currentLayerWidth = 300;
                Dictionary<int, Vector3> nodePositions = new();
                foreach (var node in creature.MyBrain.Nodes)
                {
                    if (currentLayerNumber != node.NodeLayer)
                    {
                        int oldLayerNum = currentLayerNumber;
                        nodePos = 15;
                        currentLayerWidth -= 100;
                        currentLayerNumber = node.NodeLayer;
                    }
                    // Draw the node, based on its layer and the position within its layer, then label it with the Id.
                    GUI.Label(new Rect(Screen.width - currentLayerWidth, nodePos, 300, 20), $"N{node.Id}");
                    // Draw a 5x5 square at the position of the node
                    GUI.Box(new Rect(Screen.width - currentLayerWidth - 5, nodePos + 8, 5, 5), "");
                    // Add the node to the dictionary
                    nodePositions.Add(node.Id, new Vector3(Screen.width - currentLayerWidth - 2, 0, nodePos + 4));
                    nodePos += 15;
                    // reset nodePos to 15 after each layer
                }

                LineRenderer line = Canvas.GetComponent<LineRenderer>();

                // Look at each connection, and determine which nodes it connects
                foreach (var connection in creature.MyBrain.Connections)
                {
                    if (connection == null) return;
                    if (!connection.Enabled) return;
                    if (!nodePositions.ContainsKey(connection.FromNodeId) || !nodePositions.ContainsKey(connection.ToNodeId)) return;
                    // Get the nodes from the Dictionary
                    Vector2 node1 = nodePositions[connection.FromNodeId];
                    Vector2 node2 = nodePositions[connection.ToNodeId];
                    // Draw a line between the nodes
                    line.SetPosition(0, node1);
                    line.SetPosition(1, node2);
                    line.startWidth = 1f;
                    line.endWidth = 1f;

                    // Draw the weight of the connection on the line
                    line.material.SetColor("_Color", Color.red);
                    line.material.SetColor("_EmissionColor", Color.red);
                    line.material.EnableKeyword("_EMISSION");
                    line.useWorldSpace = false;
                    line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    line.receiveShadows = false;
                    line.gameObject.layer = 0;
                    line.material.renderQueue = 3000;

                    // Remove alpha transparency from the line
                    line.material.SetFloat("_Mode", 2);
                }
            }
        }
    }

    public void Save(string path)
    {
        // Save the HumanAgent creature to a file
        using StreamWriter writer = new(path);
        writer.WriteLine(JsonUtility.ToJson(SelectedCreature));
        writer.Close();
    }
    #endregion
    #region Methods
    // Your custom methods go here

    #endregion
}
