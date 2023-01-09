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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

public class NeuralNetwork : IComparable<NeuralNetwork>
{

    #region Settable Variables
    public Dictionary<int, Node> Nodes { get { return _nodes; } }
    public Dictionary<int, Connection> Connections { get { return _connections; } }
    #endregion

    #region Private Variables
    private int[] _netLayers;
    private Dictionary<int, Node> _nodes = new();
    private Dictionary<int, Connection> _connections = new();
    private float _fitness;
    #endregion

    #region Properties
    public int[] NetLayers { get { return _netLayers; } }
    public float Fitness { get; internal set; }
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;
        if (Fitness > other.Fitness) return 1;
        if (Fitness < other.Fitness) return -1;
        return 0;
    }

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
                if (type != Node.NodeType.Input && type != Node.NodeType.Bias) node.Init();
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
            EvaluateNode(_nodes[i]);
        }
    }

    public void EvaluateNode(Node node)
    {
        foreach ((int connectionId, Connection connectionItem) in node.Connections)
        {
            if (connectionItem.Enabled)
            {
                _nodes[connectionItem.ToNodeId].Value += connectionItem.Weight * node.Value;
            }
            _nodes[connectionItem.ToNodeId].Value = System.MathF.Tanh(_nodes[connectionItem.ToNodeId].Value);
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
            EvaluateNode(_nodes[i]);
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

    public void Mutate()
    {
        // Mutate weights
        foreach ((int connectionId, Connection connectionItem) in _connections)
        {
            connectionItem.Mutate();
        }

        // Mutate nodes
        foreach ((int nodeId, Node nodeItem) in _nodes)
        {
            nodeItem.Mutate();
        }
    }

    public NeuralNetwork Crossover(NeuralNetwork otherParent)
    {
        // Crossover weights
        foreach ((int connectionId, Connection connectionItem) in _connections)
        {
            connectionItem.Crossover(otherParent._connections[connectionId]);
        }

        // Crossover nodes
        foreach ((int nodeId, Node nodeItem) in _nodes)
        {
            nodeItem.Crossover(otherParent._nodes[nodeId]);
        }

        return this;
    }

    public void Copy(NeuralNetwork otherParent)
    {
        // Copy weights
        foreach ((int connectionId, Connection connectionItem) in _connections)
        {
            int copyId = _connections.Count + 1;
            connectionItem.Copy(otherParent._connections[connectionId]);
            connectionItem.InnovationId = copyId;
        }

        // Copy nodes
        foreach ((int nodeId, Node nodeItem) in _nodes)
        {
            int copyId = _nodes.Count + 1;
            nodeItem.Copy(otherParent._nodes[nodeId]);
            nodeItem.Id = copyId;
        }
    }

    public void Reset()
    {
        foreach ((int nodeId, Node nodeItem) in _nodes)
        {
            nodeItem.Reset();
        }
    }

    public void Save(string path)
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

    public static NeuralNetwork Load(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<NeuralNetwork>(json);
    }

    public void SaveAsAsset(string path)
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

    public static NeuralNetwork LoadFromAsset(string path)
    {
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<NeuralNetwork>(json);
    }

    public void CalculateNetworkFitness()
    {
        float fitness = 0;
        foreach ((int nodeId, Node nodeItem) in _nodes)
        {
            fitness += nodeItem.CalculateNodeFitness(); // Calculate node fitness by comparing node value to desired value
        }
        Fitness = fitness;
    }

    public void AddNode(int connectionId)
    {
        // Create new node
        int newNodeId = _nodes.Count + 1;
        Node newNode = new(newNodeId, Node.NodeType.Hidden);

        // Create new connections
        int newConnectionId1 = _connections.Count + 1;
        Connection newConnection1 = new(newConnectionId1, _connections[connectionId].FromNodeId, newNodeId, _connections[connectionId].Weight);
        int newConnectionId2 = _connections.Count + 2;
        Connection newConnection2 = new(newConnectionId2, newNodeId, _connections[connectionId].ToNodeId, 1);

        // Add new connections to nodes
        _nodes[_connections[connectionId].FromNodeId].AddConnection(newConnection1);
        newNode.AddConnection(newConnection1);
        newNode.AddConnection(newConnection2);
        _nodes[_connections[connectionId].ToNodeId].AddConnection(newConnection2);

        // Add new node to network
        _nodes.Add(newNodeId, newNode);

        // Disable old connection
        _connections[connectionId].Enabled = false;
    }
    #endregion
}
