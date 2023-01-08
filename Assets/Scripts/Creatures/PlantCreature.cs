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

public class PlantCreature : MonoBehaviour, ICreature
{
    #region Settable Variables
    public float FoodEnergySupplied { get { return _foodEnergySupplied; } set { _foodEnergySupplied = value; } }
    // How much energy this creature supplies when eaten.
    [SerializeField] int MyNumber = 0;
    [SerializeField] float Age = 0f;
    [SerializeField] float Health = 0f;
    [SerializeField] float Energy = 0f;
    [SerializeField] float MaxAge = 0f;
    [SerializeField] float MinHealth = 0f;
    [SerializeField] float MaxHealth = 0f;
    [SerializeField] float MaxEnergy = 0f;
    [SerializeField] float MinEnergy = 0f;
    [SerializeField] PopulationManager MyManager = null;
    #endregion
    #region Private Variables
    private float _foodEnergySupplied = 25f;
    #endregion
    #region Properties
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

    }

    #endregion
    #region Loop
    // Loop methods go here

    #endregion
    #region Methods
    public void Init()
    {
        Age = 0f;
        Health = MaxHealth;
        Energy = MaxEnergy;
    }

    public void AddAge(float age)
    {
        Age += age;
    }

    public void AddHealth(float health)
    {
        Health += health;
    }

    public void AddEnergy(float energy)
    {
        Energy += energy;
    }

    public void SubtractHealth(float health)
    {
        Health -= health;
    }

    public void SubtractEnergy(float energy)
    {
        Energy -= energy;
    }

    public void SetFoodEnergySupplied(float energySupplied)
    {
        _foodEnergySupplied = energySupplied;
    }

    #endregion
}

