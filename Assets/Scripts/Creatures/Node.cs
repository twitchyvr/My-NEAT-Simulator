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
//using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

public class Node
{

    #region Private Variables
    private int _id;
    private float _value;
    private List<Connection> _connections;

    private NodeType _type;
    private float _inputSum;
    private float _outputSum;
    private int _nodeLayer;

    #endregion

    #region Properties
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public float Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public List<Connection> Connections
    {
        get { return _connections; }
        set { _connections = value; }
    }

    public NodeType Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public float InputSum
    {
        get { return _inputSum; }
        set { _inputSum = value; }
    }

    public float OutputSum
    {
        get { return _outputSum; }
        set { _outputSum = value; }
    }

    public int NodeLayer
    {
        get { return _nodeLayer; }
        set { _nodeLayer = value; }
    }

    public enum NodeType
    {
        Input,
        Hidden,
        Output
    }

    #endregion

    #region Methods

    public Node(int id, NodeType type, int nodeLayer)
    {
        _id = id;
        _type = type;
        _nodeLayer = nodeLayer;
        _connections = new List<Connection>();
    }

    public void AddConnection(Connection connection)
    {
        _connections.Add(connection);
    }

    public void RemoveConnection(Connection connection)
    {
        _connections.Remove(connection);
    }

    public void RemoveAllConnections()
    {
        _connections.Clear();
    }

    public void CalculateValue()
    {
        if (_type == NodeType.Input)
        {
            return;
        }

        _value = Sigmoid(_inputSum);
    }

    public float Sigmoid(float x)
    {
        return 1 / (1 + MathF.Exp(-x));
    }
    #endregion
}
