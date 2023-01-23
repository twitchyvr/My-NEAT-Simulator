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
[Serializable]
public class HumanAgent : MonoBehaviour, ICreature
{
    #region Settable Variables
    [SerializeField] Camera _cam;
    [Range(-1f, 1f)] public float AgentMoveAcceleration = 0f;
    [Range(-1f, 1f)] public float AgentTurnTorque = 0f;
    [SerializeField][Range(0.0001f, 100f)] private float _moveMultiplier = 50f;
    [SerializeField][Range(0.0001f, 1000f)] private float _turnMultiplier = 500f;
    [SerializeField][Range(0.0001f, 10f)] private float _energyMultiplier = 0.010f;
    [SerializeField][Range(0.0001f, 1f)] private float _inputBias = 0.25f;
    public float MaxFoodDistance = 5f;
    public float MaxWallDistance = 5f;
    public float BrainFitness = 0f;
    public float AgentFitness = 0f;
    public float Age = 0;
    public float MaxAge = 1000f;
    public float Health = 1000f;
    public float MaxHealth = 1000f;
    public float MinHealth = 0f;
    public float Energy = 1000f;
    public float MaxEnergy = 1000f;
    public float MinEnergy = 0f;
    public PopulationManager MyManager;
    public NeuralNetwork MyBrain;
    public int BrainInputNodesCount = 10;
    public int BrainOutputNodesCount = 4;
    public GameObject MyTarget;
    public int MyNumber = 0;
    #endregion

    #region Private Variables
    private Transform _transform;
    private Rigidbody _rigidbody;
    private float _timeSinceBirth = 0f;
    private float _birthTime = 0f;
    private float _lastUpdateTime = 0f;
    private int _speciesId = 0;
    private Vector3 _startingPosition;
    #endregion

    #region Properties
    public bool isDead = false;
    public float GetAge { get { return Age; } }
    public float GetEnergy { get { return Energy; } }
    public float GetHealth { get { return Health; } }
    public float GetMaxHealth { get { return MaxHealth; } }
    public float GetMaxEnergy { get { return MaxEnergy; } }
    public Transform GetTransform { get { return _transform; } }
    public Rigidbody GetRigidbody { get { return _rigidbody; } }
    public int SpeciesId { get { return _speciesId; } set { _speciesId = value; } }
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
        _startingPosition = _transform.position;
        _rigidbody = GetComponent<Rigidbody>();
        _birthTime = Time.time;
        _timeSinceBirth = 0f;
    }
    #endregion

    #region Loop
    protected void FixedUpdate()
    {

        // Move the creature based on user input using horizontal and vertical axises
        //f (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        //{
        //    Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        //}

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

    /// <summary>
    /// The Move method will move the creature based on the input.
    /// </summary>
    /// <param name="a">The amount to move forward.</param>
    /// <param name="t">The amount to turn.</param>
    public void Move(float a, float t)
    {
        // If the creature is dead, do not move
        if (isDead) return;


        // Move in the direction the creature is facing, so that it looks like it is naturally moving forward in the direction it is facing at all times.  AddForce alone feels awkward.
        // Using a better method than AddForce, such as AddRelativeForce, will cause the creature to drift sideways, which is not what we want.
        _rigidbody.AddRelativeForce(_moveMultiplier * a * Vector3.forward, ForceMode.Acceleration);

        //_rigidbody.AddForce(_moveMultiplier * a * _transform.forward, ForceMode.Acceleration);

        // Turn the creature, without drifting sideways so that the creature moves in the direction it is facing
        _rigidbody.AddRelativeTorque(_turnMultiplier * t * Vector3.up, ForceMode.Acceleration);
        //_rigidbody.AddTorque(_turnMultiplier * t * _transform.up, ForceMode.Acceleration);

        // Subtract a slight amount of energy based on the amount of movement
        SubtractEnergy((Mathf.Abs(a) + Mathf.Abs(t)) * _energyMultiplier);
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
        MyBrain = neuralNetwork;
    }

    public void CreatureUpdate()
    {
        if (isDead) return;
        // Update time since birth
        _timeSinceBirth = Time.time - _birthTime;

        // Add age based on seconds lived
        AddAge(Time.deltaTime);

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

        Node[] outputs = ProcessInputs();
        MyBrain.RunTheNetwork();

        // Find the first value in the output nodes
        BrainFitness = 0f;
        AgentFitness = 0f;
        // Count the number of output nodes, and add the value of each output node and then divide by the number of nodes to get the BrainFitness
        foreach (Node node in outputs)
        {
            BrainFitness += node.Value;
        }

        BrainFitness /= outputs.Length;

        // Add the fitness of the creature based on the distance from the starting position
        AgentFitness += Vector3.Distance(_transform.position, _startingPosition);

        // If the outputs array has a value for the first and second output nodes, then use those values to move the creature
        if (outputs.Length > 0)
        {
            AgentMoveAcceleration = outputs[0].Value;
            AgentTurnTorque = outputs[1].Value;
        }

        MyBrain.Fitness = BrainFitness;

        Move(AgentMoveAcceleration, AgentTurnTorque);
        _lastUpdateTime = Time.time;
    }

    void Death(int reason = 0)
    {
        if (isDead) return;
        isDead = true;
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

        // Add MyBrain to the MyManager.AgentNets array
        NeuralNetwork[] temp = new NeuralNetwork[MyManager.AgentNets.Length + 1];
        MyManager.AgentNets.CopyTo(temp, 0);
        temp[MyManager.AgentNets.Length] = MyBrain;
        MyManager.AgentNets = temp;
        Destroy(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (isDead) return;
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

    public Node[] ProcessInputs()
    {
        // Get the inputs
        float[] inputs = new float[BrainInputNodesCount];
        inputs[0] = 0;                      // unused
        inputs[1] = _transform.position.x;  // x position sensor
        inputs[2] = _transform.position.z;  // z position sensor
        inputs[3] = 0;                      // food sensor
        inputs[4] = 0;                      // wall sensor 1 forward+left
        inputs[5] = 0;                      // wall sensor 2 forward+right
        inputs[6] = GetEnergy;              // energy sensor
        inputs[7] = GetHealth;              // health sensor
        inputs[8] = GetAge;                 // age sensor
        inputs[9] = BrainFitness;           // brain fitness sensor
        inputs[10] = _inputBias;             // bias

        Vector3 forward = _transform.forward;
        Vector3 right = _transform.right;
        Vector3 left = -_transform.right;
        Vector3 forwardRight = forward + right;
        Vector3 forwardLeft = forward + left;

        // Adjust the raycast so it is angled slightly down toward the ground
        forward.y = -0.01f;

        // Raycast to find the closest food using input[2]
        if (Physics.Raycast(_transform.position, forward, out RaycastHit hit, MaxFoodDistance))
        {
            if (hit.collider.gameObject.CompareTag("Food"))
            {
                Debug.DrawLine(_transform.position, hit.point, Color.green, 0.2f);
                inputs[2] = hit.distance / MaxFoodDistance;
            }
        }

        // Raycast to find the closest walls diagonally using input[3] and input [4]
        if (Physics.Raycast(_transform.position, forwardLeft, out RaycastHit hit1, MaxWallDistance))
        {
            if (hit1.collider.gameObject.CompareTag("Wall"))
            {
                Debug.DrawLine(_transform.position, hit1.point, Color.red, 0.2f);
                inputs[3] = hit1.distance / MaxWallDistance;
            }
        }

        if (Physics.Raycast(_transform.position, forwardRight, out RaycastHit hit2, MaxWallDistance))
        {
            if (hit2.collider.gameObject.CompareTag("Wall"))
            {
                Debug.DrawLine(_transform.position, hit2.point, Color.red, 0.2f);
                inputs[4] = hit2.distance / MaxWallDistance;
            }
        }

        // Create a new Node array that is the same size as the inputs array
        Node[] inputArray = new Node[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            // If the input is the last one, it is the bias node
            if (i == inputs.Length - 1)
            {
                inputArray[i] = (new Node(i, Node.NodeType.Bias, 0, inputs[i]));
            }
            // Otherwise, it is an input node
            else
            {
                inputArray[i] = (new Node(i, Node.NodeType.Input, 0, inputs[i]));
            }
        }

        // Load the inputs into the neural network's input nodes
        Node[] inputNodesOutputValues = MyBrain.LoadInputs(inputArray);

        return inputNodesOutputValues;
    }

    public void Save(string path)
    {
        string jsonString = JsonUtility.ToJson(MyBrain);
        using StreamWriter writer = new(path);
        foreach (Node node in MyBrain.Nodes)
        {
            if (node.Value != 0)
                jsonString += JsonUtility.ToJson(node);
        }
        foreach (Connection connection in MyBrain.Connections)
        {
            if (connection.Weight != 0)
                jsonString += JsonUtility.ToJson(connection);
        }
        //
        writer.Write(jsonString);
        writer.Close();
    }

    public void Load(string path)
    {
        using StreamReader reader = new(path);
        JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), this);
        reader.Close();
    }

    #endregion
}
