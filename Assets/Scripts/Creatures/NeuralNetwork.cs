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
using System.Collections.Generic;
using System.Linq;
//using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

public class NeuralNetwork
{

    #region Settable Variables
    public Dictionary<int, Node> Nodes { get { return _nodes; } }
    public Dictionary<int, Connection> Connections { get { return _connections; } }
    #endregion

    #region Private Variables
    private int[] _netLayers;
    private Dictionary<int, Node> _nodes = new();
    private Dictionary<int, Connection> _connections = new();
    #endregion

    #region Properties
    public int[] NetLayers { get { return _netLayers; } }
    #endregion

    #region Methods
    public void Init(int[] netLayers = null)
    {
        if (netLayers != null)
            _netLayers = netLayers;
        else
            _netLayers = new int[3] { 8, 0, 2 };

        int nodeId = 0;
        int connId = 0;

        // Create nodes for each layer
        for (int i = 0; i < _netLayers.Length; i++)
        {
            for (int j = 0; j < _netLayers[i]; j++)
            {
                float netlayerszero = _netLayers[0] - 1;
                Node.NodeType type = Node.NodeType.Hidden;
                if (i == 0 && j != netlayerszero)
                    type = Node.NodeType.Input;
                else if (i > 0 && i < _netLayers.Length - 1)
                    type = Node.NodeType.Hidden;
                if (i == 0 && j == netlayerszero)
                    // Make the last node in the input layer a bias node by default
                    type = Node.NodeType.Bias;
                else if (i == _netLayers.Length - 1)
                    type = Node.NodeType.Output;

                Node node = new(nodeId, type, i);
                node.Init();
                foreach ((int connectionId, Connection connectionItem) in node.Connections)
                {
                    connectionItem.Init();
                }
                if (type != Node.NodeType.Input) node.Init();
                _nodes.Add(nodeId, node);
                nodeId++;
            }
        }

        foreach ((int thisNodeId, Node thisNode) in _nodes)
        {
            foreach ((int thisConnId, Connection thisConnection) in thisNode.Connections)
            {
                _connections.Add(thisConnId, thisConnection);
            }
        }

        int tick = 0;

        // Create connections from input nodes to output nodes only
        for (int i = 0; i < _netLayers[0]; i++)
        {
            for (int j = 0; j < _netLayers[^1]; j++)
            {
                Connection conn = new(connId, _nodes[i], _nodes[_nodes.Count - _netLayers[^1] + j])
                {
                    Weight = (UnityEngine.Random.Range(-1f, 1f) * _nodes[_nodes.Count - _netLayers[0]].Value),
                    Enabled = true,
                    IsRecurrent = false,
                    InnovationId = connId
                };
                _nodes[i].AddConnection(conn);
                _nodes[_nodes.Count - _netLayers[^1] + j].AddConnection(conn); // Add connection to output node
                connId++;
            }
        }
        Tick(tick++);
    }

    public int Tick(int tick)
    {
        return tick;
    }

    public NeuralNetwork(int[] netLayers = null)
    {
        Init(netLayers);
    }

    public void SetInputValues(float[] inputValues)
    {
        for (int i = 0; i < _netLayers[0]; i++)
        {
            _nodes[i].Value = inputValues[i];
        }
    }

    public float[] GetOutputValues()
    {
        float[] outputValues = new float[_netLayers[^1]];
        for (int i = 0; i < _netLayers[^1]; i++)
        {
            outputValues[i] = _nodes[_nodes.Count - _netLayers[^1] + i].Value;
        }
        return outputValues;
    }

    public void EvaluateInput(Dictionary<int, Node> inputValues, Dictionary<int, Node> desiredOutputValues)
    {
        // Set input values
        for (int i = 0; i < _netLayers[0]; i++)
        {
            _nodes[i].Value = inputValues[i].Value;
        }

        // Set desired output values
        for (int i = 0; i < _netLayers[^1]; i++)
        {
            _nodes[_nodes.Count - _netLayers[^1] + i].DesiredValue = desiredOutputValues[i].Value;
        }

        // Evaluate network
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].Evaluate();
        }
    }

    public Dictionary<int, Node> FeedForward(Dictionary<int, Node> inputValues)
    {
        // Set input values
        for (int i = 0; i < _netLayers[0]; i++)
        {
            _nodes[i].Value = inputValues[i].Value;
        }

        // Evaluate network
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].Evaluate();
        }

        // Return output values
        Dictionary<int, Node> outputValues = new();
        for (int i = 0; i < _netLayers[^1]; i++)
        {
            outputValues.Add(i, _nodes[_nodes.Count - _netLayers[^1] + i]);
        }
        return outputValues;
    }

    public override string ToString()
    {
        string s = "";
        for (int i = 0; i < _nodes.Count; i++)
        {
            s += _nodes[i].ToString() + "\n";
        }
        return s;
    }

    #endregion
}
