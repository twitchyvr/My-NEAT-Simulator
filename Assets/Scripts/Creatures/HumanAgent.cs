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


public class HumanAgent : MonoBehaviour, ICreature
{

    #region Settable Variables
    [SerializeField] Camera _cam;
    [SerializeField] private int[] _neuralNetworkLayers = new int[] { 8, 2 };
    public float Health = 100f;
    public float MaxHealth = 100f;
    public int Age = 0;
    public float Energy = 100f;
    public float MaxEnergy = 100f;
    public PopulationManager MyManager;
    public NeuralNetwork MyBrain { get { return _neuralNetwork; } set { _neuralNetwork = value; } }
    public GameObject MyTarget;

    public int MyNumber = 0;
    #endregion

    #region Private Variables
    private Transform _transform;
    private Rigidbody _rigidbody;
    private NeuralNetwork _neuralNetwork;
    #endregion

    #region Properties
    public Transform GetTransform { get { return _transform; } } // Example getter for the _transform property
    public Rigidbody GetRigidbody { get { return _rigidbody; } } // Example getter for the _rigidbody property

    public float GetHealth { get { return Health; } }
    public float GetMaxHealth { get { return MaxHealth; } }
    public int GetAge { get { return Age; } }
    public float GetEnergy { get { return Energy; } }
    public float GetMaxEnergy { get { return MaxEnergy; } }
    public NeuralNetwork GetNeuralNetwork { get { return _neuralNetwork; } }
    public int[] GetNeuralNetworkLayers { get { return _neuralNetworkLayers; } set { _neuralNetworkLayers = value; } }
    float ICreature.Health { get { return Health; } set { Health = value; } }
    float ICreature.MaxHealth { get { return MaxHealth; } set { MaxHealth = value; } }
    int ICreature.Age { get { return Age; } }
    float ICreature.Energy { get { return Energy; } }
    float ICreature.MaxEnergy { get { return MaxEnergy; } }
    #endregion

    #region Init
    protected void Awake()
    {
        _cam = _cam != null ? _cam : Camera.main;    // If a camera is not set here, use the default one.
        _transform = transform;                         // This will only be ran once
        _rigidbody = GetComponent<Rigidbody>();

    }
    #endregion

    #region Loop
    // FixedUpdate is called once per physics step.
    protected void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Move(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        }
    }
    // LateUpdate is called once per frame after all Update functions have been called, but before rendering.
    protected void LateUpdate()
    {

    }

    public void AddAge(int age)
    {
        throw new System.NotImplementedException();
    }

    public void AddHealth(float health)
    {
        throw new System.NotImplementedException();
    }

    public void SubtractHealth(float health)
    {
        throw new System.NotImplementedException();
    }

    public void AddEnergy(float energy)
    {
        throw new System.NotImplementedException();
    }

    public void SubtractEnergy(float energy)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Methods
    public void Init()
    {
        _neuralNetwork = new NeuralNetwork(_neuralNetworkLayers);
    }

    public void Move(float a, float t)
    {
        _transform.Translate(a * Time.deltaTime * Vector3.forward);
        _transform.Rotate(t * Time.deltaTime * Vector3.up * 100);
    }
    #endregion
}
