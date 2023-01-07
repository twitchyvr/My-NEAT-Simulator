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

    #region Settable Variables
    #endregion

    #region Private Variables
    private float _health = 100f;
    private float _maxHealth = 100f;
    private int _age = 0;
    private int _maxAge = 100;
    private float _energy = 100f;
    private float _maxEnergy = 100f;

    #endregion

    #region Properties
    public float Health { get { return _health; } set { _health = value; } }
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public int Age { get { return _age; } }
    public int MaxAge { get { return _maxAge; } }
    public float Energy { get { return _energy; } }
    public float MaxEnergy { get { return _maxEnergy; } }
    #endregion

    #region Methods
    public void AddAge(int age)
    {
        _age += age;
    }

    public void AddHealth(float health)
    {
        _health += health;
    }

    public void SubtractHealth(float health)
    {
        _health -= health;
    }

    public void AddEnergy(float energy)
    {
        _energy += energy;
    }

    public void SubtractEnergy(float energy)
    {
        _energy -= energy;
    }
    #endregion
}
#region Interfaces
public interface ICreature
{
    float Health { get; set; }
    float MaxHealth { get; set; }
    int Age { get; }
    float Energy { get; }
    float MaxEnergy { get; }
    void AddAge(int age);
    void AddHealth(float health);
    void SubtractHealth(float health);
    void AddEnergy(float energy);
    void SubtractEnergy(float energy);
    #endregion
}
