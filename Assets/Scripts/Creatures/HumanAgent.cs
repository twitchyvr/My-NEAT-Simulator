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

/// <Summary>
/// A HumanAgent is a type of creature.
/// </Summary>
public class HumanAgent : MonoBehaviour, ICreature
{
    #region Settable Variables
    [SerializeField] Camera _cam;
    [SerializeField] private int[] _neuralNetworkLayers = new int[] { 8, 0, 4 };
    [SerializeField][Range(0f, 100f)] private float _moveMultiplier = 50f;
    [SerializeField][Range(0f, 1000f)] private float _turnMultiplier = 500f;
    [SerializeField][Range(0.0001f, 1f)] private float _inputBias = 0.25f;
    public float Age = 0;
    public float MaxAge = 100f;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float MinHealth = 0f;
    public float Energy = 100f;
    public float MaxEnergy = 100f;
    public float MinEnergy = 0f;
    public PopulationManager MyManager;
    public NeuralNetwork MyBrain { get { return _neuralNetwork; } set { _neuralNetwork = value; } }
    public GameObject MyTarget;
    public int MyNumber = 0;
    #endregion

    #region Private Variables
    private Transform _transform;
    private Rigidbody _rigidbody;
    private NeuralNetwork _neuralNetwork;
    private float _timeSinceBirth = 0f;
    private float _birthTime = 0f;
    private float _lastUpdateTime = 0f;
    #endregion

    #region Properties
    public float GetAge { get { return Age; } }
    public float GetEnergy { get { return Energy; } }
    public float GetHealth { get { return Health; } }
    public float GetMaxHealth { get { return MaxHealth; } }
    public float GetMaxEnergy { get { return MaxEnergy; } }
    public Transform GetTransform { get { return _transform; } }
    public Rigidbody GetRigidbody { get { return _rigidbody; } }
    public NeuralNetwork GetNeuralNetwork { get { return _neuralNetwork; } }
    public int[] GetNeuralNetworkLayers { get { return _neuralNetworkLayers; } set { _neuralNetworkLayers = value; } }
    int ICreature.MyNumber { get { return MyNumber; } set { MyNumber = value; } }
    float ICreature.Age { get { return Age; } }
    float ICreature.Health { get { return Health; } }
    float ICreature.Energy { get { return Energy; } }
    float ICreature.MaxAge { get { return MaxAge; } set { MaxAge = value; } }
    float ICreature.MinHealth { get { return MinHealth; } set { MinHealth = value; } }
    float ICreature.MaxHealth { get { return MaxHealth; } set { MaxHealth = value; } }
    float ICreature.MaxEnergy { get { return MaxEnergy; } set { MaxEnergy = value; } }
    float ICreature.MinEnergy { get { return MinEnergy; } set { MinEnergy = value; } }
    PopulationManager ICreature.MyManager { get { return MyManager; } set { MyManager = value; } }
    #endregion

    #region Init
    protected void Awake()
    {
        _cam = _cam != null ? _cam : Camera.main;    // If a camera is not set here, use the default one.
        _transform = transform;                         // This will only be ran once
        _rigidbody = GetComponent<Rigidbody>();
        _birthTime = Time.time;
        _timeSinceBirth = 0f;
    }
    #endregion

    #region Loop
    protected void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        }

        CreatureUpdate();
    }
    #endregion

    #region Methods

    /// <summary>
    /// The AddAge method will add age to the creature. If the age added is greater than the max age, the age will be set to the max age.
    /// </summary>
    /// <param name="age">The amount of age to add.</param>
    public void AddAge(float age)
    {
        if (Age + age > MaxAge)
        {
            Age = MaxAge;
            Death();
        }
        else
        {
            Age += age;
        }
    }

    /// <summary>
    /// The AddHealth method will add health to the creature. If the health added is greater than the max health, the health will be set to the max health.
    /// </summary>
    /// <param name="health">The amount of health to add.</param>
    public void AddHealth(float health)
    {
        if (Health + health > MaxHealth)
        {
            Health = MaxHealth;
        }
        else
        {
            Health += health;
        }
    }

    public void SubtractHealth(float health)
    {
        if (Health - health < MinHealth)
        {
            Health = MinHealth;
            Death(2);
        }
        else
        {
            Health -= health;
        }
    }

    /// <summary>
    /// The AddEnergy method will add energy to the creature. If the energy added is greater than the max energy, the energy will be set to the max energy.
    /// </summary>
    /// <param name="energy">The amount of energy to add.</param>
    public void AddEnergy(float energy)
    {
        if (Energy + energy > MaxEnergy)
        {
            Energy = MaxEnergy;
        }
        else
        {
            Energy += energy;
        }
    }

    /// <summary>
    /// The SubtractEnergy method will subtract energy from the creature. If the energy subtracted is less than 0, the energy will be set to 0.
    /// </summary>
    /// <param name="energy">The amount of energy to subtract.</param>
    public void SubtractEnergy(float energy)
    {
        if (Energy - energy < MinEnergy)
        {
            Energy = MinEnergy;
            SubtractHealth(energy);
        }
        else
        {
            Energy -= energy;
        }
    }

    public void Init()
    {
        _neuralNetwork = new NeuralNetwork(_neuralNetworkLayers);
    }

    /// <summary>
    /// The Move method will move the creature based on the input.
    /// </summary>
    /// <param name="a">The amount to move forward.</param>
    /// <param name="t">The amount to turn.</param>
    public void Move(float a, float t)
    {
        _transform.Translate(_moveMultiplier * a * Time.deltaTime * Vector3.forward);
        _transform.Rotate(_turnMultiplier * t * Time.deltaTime * Vector3.up);
    }

    public void SetTarget(GameObject target)
    {
        MyTarget = target;
    }

    public void SetManager(PopulationManager manager)
    {
        MyManager = manager;
    }

    public void SetNumber(int number)
    {
        MyNumber = number;
    }

    public void SetNeuralNetwork(NeuralNetwork neuralNetwork)
    {
        _neuralNetwork = neuralNetwork;
    }

    public void SetNeuralNetworkLayers(int[] layers)
    {
        _neuralNetworkLayers = layers;
    }

    public void CreatureUpdate()
    {
        // Update time since birth
        _timeSinceBirth = Time.time - _birthTime;
        float timeSinceLastUpdate = Time.time - _lastUpdateTime;

        // Add age based on seconds lived
        AddAge(timeSinceLastUpdate);

        // Remove energy based on seconds lived (1 energy per second)
        SubtractEnergy(Time.deltaTime);

        if (Energy <= MinEnergy)
        {
            SubtractHealth(Time.deltaTime);
        }
        if (Health <= MinHealth)
        {
            Death();
        }
        Dictionary<int, Node> outputs = ProcessInputs();
        Move(outputs[0].Value, outputs[1].Value);


        _lastUpdateTime = Time.time;
    }

    void Death(int reason = 0)
    {
        string myName = this.name;
        switch (reason)
        {
            case 1:
                Debug.Log("Creature " + myName + " died of energy deprivation.");
                break;

            case 2:
                Debug.Log("Creature " + myName + " died of poor health.");
                break;

            case 3:
                Debug.Log("Creature " + myName + " died by colliding with a wall.");
                break;

            default:
                Debug.Log("Creature " + myName + " died of old age.");
                break;
        }

        Destroy(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            AddEnergy(other.gameObject.TryGetComponent<PlantCreature>(out PlantCreature plant) ? plant.FoodEnergySupplied : 0); // If the object has a plant component, use that, otherwise use 0.
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            Death(3);
        }
    }

    public Dictionary<int, Node> ProcessInputs()
    {
        // Get the inputs
        float[] inputs = new float[_neuralNetworkLayers[0]];
        inputs[0] = _transform.position.x;
        inputs[1] = _transform.position.z;
        inputs[2] = 0;
        inputs[3] = 0;
        inputs[4] = 0;
        inputs[5] = 0;
        inputs[6] = 0;
        inputs[7] = _inputBias;

        // Store the inputs in a dictionary
        Dictionary<int, Node> inputDictionary = new();
        for (int i = 0; i < inputs.Length; i++)
        {
            if (i == inputs.Length - 1)
            {
                inputDictionary.Add(i, new Node(i, Node.NodeType.Bias, 0, inputs[i]));
            }
            else
            {
                inputDictionary.Add(i, new Node(i, Node.NodeType.Input, 0, inputs[i]));
            }
        }

        // Get the outputs
        Dictionary<int, Node> outputs = _neuralNetwork.FeedForward(inputDictionary);

        // Process the outputs
        float[] outputArray = new float[outputs.Count];
        for (int i = 0; i < outputs.Count; i++)
        {
            outputArray[i] = outputs[i].Value;
        }

        return outputs;
    }

    #endregion
}
