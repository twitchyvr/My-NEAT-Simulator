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
//using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

using System;

[Serializable]
public class Connection
{
    #region Private Variables
    private int id = -1;
    private int innovationId = -1;
    private int fromNodeId = -1;
    private int toNodeId = -1;
    private float weight = -1f;
    private bool enabled = true;
    private bool isRecurrent = false;
    #endregion

    #region Properties
    public int Id
    {
        get { return id; }
        set { id = value; }
    }
    public int InnovationId
    {
        get { return innovationId; }
        set { innovationId = value; }
    }

    public int FromNodeId
    {
        get { return fromNodeId; }
        set { fromNodeId = value; }
    }

    public int ToNodeId
    {
        get { return toNodeId; }
        set { toNodeId = value; }
    }

    public float Weight
    {
        get { return weight; }
        set { weight = value; }
    }

    public bool Enabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    public bool IsRecurrent
    {
        get { return isRecurrent; }
        set { isRecurrent = value; }
    }

    #endregion

    #region Methods
    public Connection(int connectionId, int innovationId, int fromNodeId, int toNodeId, float weight, bool enabled, bool isRecurrent)
    {
        this.id = connectionId;
        this.innovationId = innovationId;
        this.fromNodeId = fromNodeId;
        this.toNodeId = toNodeId;
        this.weight = weight;
        this.enabled = enabled;
        this.isRecurrent = isRecurrent;
    }

    public Connection(Connection connection)
    {
        this.id = connection.id;
        this.innovationId = connection.innovationId;
        this.fromNodeId = connection.fromNodeId;
        this.toNodeId = connection.toNodeId;
        this.weight = connection.weight;
        this.enabled = connection.enabled;
        this.isRecurrent = connection.isRecurrent;
    }

    public Connection(int newConnId, int fromNodeId, int toNodeId, float weight)
    {
        this.id = newConnId;
        this.innovationId = -1;
        this.fromNodeId = fromNodeId;
        this.toNodeId = toNodeId;
        this.weight = weight;
        this.enabled = true;
        this.isRecurrent = false;
    }

    public Connection(int newConnId, Node fromNode, Node toNode)
    {
        this.id = newConnId;
        this.innovationId = -1;
        this.fromNodeId = fromNode.Id;
        this.toNodeId = toNode.Id;
        this.weight = Init();
        this.enabled = true;
        this.isRecurrent = false;
    }

    public Connection(int newConnId, int fromNodeId, int toNodeId)
    {
        this.id = newConnId;
        this.innovationId = -1;
        this.fromNodeId = fromNodeId;
        this.toNodeId = toNodeId;
        this.weight = Init();
        this.enabled = true;
        this.isRecurrent = false;
    }

    public Connection(int newConnId, int innovationId, int fromNodeId, int toNodeId, float weight)
    {
        this.id = newConnId;
        this.innovationId = innovationId;
        this.fromNodeId = fromNodeId;
        this.toNodeId = toNodeId;
        this.weight = weight;
        this.enabled = true;
        this.isRecurrent = false;
    }

    public Connection(int newConnId, int innovationId, int fromNodeId, int toNodeId)
    {
        this.id = newConnId;
        this.innovationId = innovationId;
        this.fromNodeId = fromNodeId;
        this.toNodeId = toNodeId;
        this.weight = Init();
        this.enabled = true;
        this.isRecurrent = false;
    }

    public Connection(int newConnId, int innovationId, int fromNodeId, int toNodeId, bool isConnRecurrent)
    {
        this.id = newConnId;
        this.innovationId = innovationId;
        this.fromNodeId = fromNodeId;
        this.toNodeId = toNodeId;
        this.weight = Init();
        this.enabled = true;
        this.isRecurrent = isConnRecurrent;
    }

    public override string ToString()
    {
        return "[" + id + "] = " + fromNodeId + " -> " + ToNodeId + " : " + weight;
    }

    public float Init()
    {
        return Weight = UnityEngine.Random.Range(-1f, 1f);
    }

    public void Mutate()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < 0.1f)
        {
            weight = UnityEngine.Random.Range(-1f, 1f);
        }
        else
        {
            weight += UnityEngine.Random.Range(-0.1f, 0.1f);
        }
    }

    internal void Copy(Connection connection)
    {
        // Create a copy of the connection
        this.innovationId = connection.innovationId;
        this.fromNodeId = connection.fromNodeId;
        this.toNodeId = connection.toNodeId;
        this.weight = connection.weight;
        this.enabled = connection.enabled;
        this.isRecurrent = connection.isRecurrent;
    }

    internal void Crossover(Connection otherConnection)
    {
        // Crossover the connection with the other connection
        // using the other connection's weight

        // If the other connection is disabled, disable this connection
        if (!otherConnection.enabled)
        {
            this.enabled = false;
        }

        // If the other connection is recurrent, make this connection recurrent
        if (otherConnection.isRecurrent)
        {
            this.isRecurrent = true;
        }

        // Crossover the weight
        this.weight = otherConnection.weight;

    }

    #endregion
}
