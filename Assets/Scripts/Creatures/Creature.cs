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
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

public class Creature : MonoBehaviour
{
    #region Properties
    public float Age { get; private set; } = 0;
    public float MaxAge { get; private set; } = 100;
    public float Energy { get; private set; } = 100f;
    public float Health { get; private set; } = 100f;
    public float MinHealth { get; private set; } = 0f;
    public float MinEnergy { get; private set; } = 0f;
    public float MaxHealth { get; private set; } = 100f;
    public float MaxEnergy { get; private set; } = 100f;
    #endregion

    #region Methods
    public void AddAge(int age)
    {
        if (Age + age > MaxAge)
        {
            Age = MaxAge;
        }
        else
        {
            Age += age;
        }
    }

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
        }
        else
        {
            Health -= health;
        }
    }

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

    public void SubtractEnergy(float energy)
    {
        if (Energy - energy < 0)
        {
            Energy = 0;
        }
        else
        {
            Energy -= energy;
        }
    }
    #endregion
}
#region Interfaces
public interface ICreature
{
    float Age { get; }
    float Health { get; }
    float Energy { get; }
    float MaxAge { get; set; }
    float MinHealth { get; set; }
    float MaxHealth { get; set; }
    float MinEnergy { get; set; }
    float MaxEnergy { get; set; }
    int MyNumber { get; set; }

    PopulationManager MyManager { get; set; }

    void AddAge(float age);
    void AddHealth(float health);
    void SubtractHealth(float health);
    void AddEnergy(float energy);
    void SubtractEnergy(float energy);
}
#endregion
