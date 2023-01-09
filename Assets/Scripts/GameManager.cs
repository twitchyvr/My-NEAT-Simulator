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


public class GameManager : MonoBehaviour
{
    public Texture NodeTexture;
    public static bool IsPaused = false;
    public GameObject SelectedCreature;
    //    public GameObject CreatureInfoPanel;
    public GameObject FoodPrefab;
    public float FoodSpawnMaxX = 80;
    public float FoodSpawnMaxZ = 80f;
    public int FoodCount = 10;
    float CreatureHealth = 0f;
    float CreatureAge = 0f;
    float CreatureEnergy = 0f;
    string CreatureName = "";


    #region Settable Variables
    #endregion

    #region Private Variables
    #endregion

    #region Properties
    #endregion

    #region Init
    protected void Awake()
    {
        // Instantiate food prefabs around the map randomly within a specific x and z range.
        for (int i = 0; i < FoodCount; i++)
        {
            Instantiate(FoodPrefab, new Vector3(Random.Range(-FoodSpawnMaxX, FoodSpawnMaxX), 0, Random.Range(-FoodSpawnMaxZ, FoodSpawnMaxZ)), Quaternion.identity);
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
        if (SelectedCreature != null)
        {
            if (SelectedCreature.TryGetComponent(out HumanAgent creature))
            {
                CreatureName = SelectedCreature.name;
                CreatureHealth = creature.Health;
                CreatureAge = creature.Age;
                CreatureEnergy = creature.Energy;
            }
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
        int nodePos = 85;
        int connPos = 10;
        // Show the selected creature's nodes and connections.
        if (SelectedCreature != null)
        {
            if (SelectedCreature.TryGetComponent(out HumanAgent creature))
            {
                foreach ((int currentNodeId, Node currentNode) in creature.MyBrain.Nodes)
                {
                    GUI.Label(new Rect(10, nodePos, 300, 20), $"Node: {currentNode.Id} - Value: {currentNode.Value}");
                    nodePos += 15;

                    foreach ((int currentConnId, Connection currentConnection) in currentNode.Connections)
                    {
                        // Place GUI.Label on right side of screen.
                        GUI.Label(new Rect(Screen.width - 170, connPos, 300, 20), $"Node {currentNode.Id} - {currentConnection}");
                        connPos += 15;
                    }

                }
            }
        }
    }

    #endregion
    #region Methods
    // Your custom methods go here

    #endregion
}
