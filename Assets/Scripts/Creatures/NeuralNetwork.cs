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

[Serializable]
public class NeuralNetwork : IComparable<NeuralNetwork>
{

    #region Settable Variables
    public Dictionary<int, Node> Nodes { get { return _nodes; } }
    public Dictionary<int, Connection> Connections { get { return _connections; } }
    #endregion

    #region Private Variables
    //    private int[] _netLayers;
    private Dictionary<int, Node> _nodes = new();
    private Dictionary<int, Connection> _connections = new();
    private float _defaultBias = 0.25f;
    private float _fitness;
    #endregion

    #region Properties
    //    public int[] NetLayers { get { return _netLayers; } }
    public float Fitness { get; internal set; }
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;
        if (Fitness > other.Fitness) return 1;
        if (Fitness < other.Fitness) return -1;
        return 0;
    }

    #endregion

    #region Constructors
    public NeuralNetwork(int[] netLayers = null)
    {
        // Init with the first and last layers
        Initialize(netLayers[0], netLayers[^1]);
    }

    public NeuralNetwork(int inputNodesCount, int outputNodesCount)
    {
        Initialize(inputNodesCount, outputNodesCount);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Populate the arrays holding the node and connections for the network with startup values
    /// </summary>
    /// <param name="inputNodesCount">The number of input nodes</param>
    /// <param name="outputNodesCount">The number of output nodes</param>
    /// <param name="addBiasNode">Whether to add a bias node (defaults to true)</param>
    public void Initialize(int inputNodesCount, int outputNodesCount, bool addBiasNode = true)
    {
        int nodeId = 1;
        int connId = -1;

        // Create input nodes
        for (int i = 1; i <= inputNodesCount; i++)
        {
            Node inNode = new(nodeId, Node.NodeType.Input);
            inNode.Value = 0f;
            inNode.NodeLayer = 1;
            _nodes.Add(nodeId, inNode);
            nodeId++;
        }

        // Add an additional input node for bias
        Node biasNode = new(nodeId, Node.NodeType.Bias);
        if (addBiasNode)
        {
            biasNode.Value = _defaultBias;
            biasNode.NodeLayer = 1;
            _nodes.Add(nodeId, biasNode);
            nodeId++;
        }

        // Create output nodes
        for (int i = 1; i <= outputNodesCount; i++)
        {
            Node outNode = new(nodeId, Node.NodeType.Output);
            outNode.Value = 0;
            outNode.NodeLayer = 2;
            _nodes.Add(nodeId, outNode);
            nodeId++;
        }

        // Loop through all nodes and create connections
        foreach ((int thisNodeId, Node thisNode) in _nodes)
        {
            // If the node is an input or bias node, connect it to all output nodes
            if (thisNode.Type == Node.NodeType.Input || thisNode.Type == Node.NodeType.Bias)
            {
                foreach ((int otherNodeId, Node otherNode) in _nodes)
                {
                    if (otherNode.Type == Node.NodeType.Output)
                    {
                        connId = thisNodeId * 100000 + otherNodeId;
                        bool isEnabled = true;
                        if (UnityEngine.Random.Range(1, 100) < 5)
                        {
                            isEnabled = false;
                        }
                        Connection connection = new(connId, thisNodeId, otherNodeId, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
                        _connections.Add(connection.InnovationId, connection);
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method adds a node to the network
    /// </summary>
    public void AddANode()
    {
        // Select a random connection
        int connId = UnityEngine.Random.Range(1, _connections.Count);

        // If the connection is already disabled, return
        if (!_connections[connId].Enabled)
        {
            return;
        }

        // Disable the connection
        _connections[connId].Enabled = false;

        // Create a new node
        int nodeId = _nodes.Count + 1;
        Node newNode = new(nodeId, Node.NodeType.Hidden);
        newNode.Value = 0;
        newNode.NodeLayer = _nodes[_connections[connId].FromNodeId].NodeLayer + 1;
        // Add the new node to the network
        _nodes.Add(nodeId, newNode);

        // Create two new connections
        int connId1 = _connections[connId].FromNodeId * 100000 + nodeId;
        int connId2 = nodeId * 100000 + _connections[connId].ToNodeId;
        Connection newConn1 = new(connId1, _connections[connId].FromNodeId, nodeId, 1f, true, true);
        Connection newConn2 = new(connId2, nodeId, _connections[connId].ToNodeId, _connections[connId].Weight, true, true);
        // Add the new connections to the network
        _connections.Add(connId1, newConn1);
        _connections.Add(connId2, newConn2);
    }

    /// <summary>
    /// This method adds a connection to the network
    /// </summary>
    public void AddAConnection()
    {
        // Select two random nodes
        int node1 = UnityEngine.Random.Range(1, _nodes.Count);
        int node2 = UnityEngine.Random.Range(1, _nodes.Count);

        // If the connection already exists, return
        if (_connections.ContainsKey(node1 * 100000 + node2))
        {
            return;
        }

        // If the connection is from an input node to an output node, return
        if (_nodes[node1].Type == Node.NodeType.Input && _nodes[node2].Type == Node.NodeType.Output)
        {
            return;
        }

        // If the connection is from a bias node to an output node, return
        if (_nodes[node1].Type == Node.NodeType.Bias && _nodes[node2].Type == Node.NodeType.Output)
        {
            return;
        }

        // If the connection is from an input node to a hidden node, create the connection
        if (_nodes[node1].Type == Node.NodeType.Input && _nodes[node2].Type == Node.NodeType.Hidden)
        {
            int connId = node1 * 100000 + node2;
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(connId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            _connections.Add(connection.InnovationId, connection);
        }

        // If the connection is from a hidden node to an output node, create the connection
        if (_nodes[node1].Type == Node.NodeType.Hidden && _nodes[node2].Type == Node.NodeType.Output)
        {
            int connId = node1 * 100000 + node2;
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(connId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            _connections.Add(connection.InnovationId, connection);
        }

        // If the connection is from a hidden node to a hidden node on a different layer, create the connection
        if (_nodes[node1].Type == Node.NodeType.Hidden && _nodes[node2].Type == Node.NodeType.Hidden && _nodes[node1].NodeLayer != _nodes[node2].NodeLayer)
        {
            int connId = node1 * 100000 + node2;
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(connId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            _connections.Add(connection.InnovationId, connection);
        }

        // If the connection is from a hidden node to a hidden node on the same layer, and the connection is already enabled, disable it
        if (_nodes[node1].Type == Node.NodeType.Hidden && _nodes[node2].Type == Node.NodeType.Hidden && _nodes[node1].NodeLayer == _nodes[node2].NodeLayer)
        {
            int connId = node1 * 100000 + node2;
            if (_connections.ContainsKey(connId))
            {
                if (_connections[connId].Enabled)
                {
                    _connections[connId].Enabled = false;
                }
            }
        }




    }

    /// <summary>
    /// This method will mutate the network
    /// </summary>
    public void Mutate()
    {
        // Loop through all connections
        foreach ((int connId, Connection conn) in _connections)
        {
            // If the connection is enabled, randomly disable it
            if (conn.Enabled)
            {
                if (UnityEngine.Random.Range(1, 100) < 5)
                {
                    conn.Enabled = false;
                }
            }

            // If the connection is disabled, randomly enable it
            if (!conn.Enabled)
            {
                if (UnityEngine.Random.Range(1, 100) < 5)
                {
                    conn.Enabled = true;
                }
            }

            // If the connection is enabled, randomly change the weight
            if (conn.Enabled)
            {
                if (UnityEngine.Random.Range(1, 100) < 5)
                {
                    conn.Weight = UnityEngine.Random.Range(-20f, 20f);
                }
            }

            // If the connection is disabled, randomly change the weight
            if (!conn.Enabled)
            {
                if (UnityEngine.Random.Range(1, 100) < 5)
                {
                    conn.Weight = UnityEngine.Random.Range(-20f, 20f);
                }
            }
        }

        // Loop through all nodes
        foreach ((int nodeId, Node node) in _nodes)
        {
            // If the node is a bias node, randomly change the bias value
            if (node.Type == Node.NodeType.Bias)
            {
                if (UnityEngine.Random.Range(1, 100) < 5)
                {
                    node.Value = UnityEngine.Random.Range(-20f, 20f);
                }
            }
        }

    }

    /// <summary>
    /// This method will load the inputs into the network
    /// </summary>
    /// <param name="inputs">The inputs to load into the network </param>
    /// <returns>The inputs loaded into the network</returns>
    public Dictionary<int, Node> LoadInputs(Dictionary<int, Node> inputs)
    {
        // Load the inputs into the input nodes
        foreach ((int nodeId, Node node) in inputs)
        {
            if (_nodes.ContainsKey(nodeId))
                _nodes[nodeId].Value = node.Value;
        }

        RunTheNetwork();

        return _nodes;
    }

    /// <summary>
    /// This method will run the network
    /// </summary>
    public void RunTheNetwork()
    {
        if (_nodes.Count == 0)
            return;

        // Loop through all nodes
        foreach ((int nodeId, Node node) in _nodes)
        {
            // If the node is an input node, do nothing
            if (node.Type == Node.NodeType.Input)
                continue;

            // If the node is a bias node, do nothing
            if (node.Type == Node.NodeType.Bias)
                continue;

            // If the node is an output node, calculate the output value
            if (node.Type == Node.NodeType.Output)
            {
                node.Value = GetOutputs(nodeId);
            }

            // If the node is a hidden node, calculate the output value
            if (node.Type == Node.NodeType.Hidden)
            {
                node.Value = GetOutputs(nodeId);
            }
        }
    }

    /// <summary>
    /// This method will get the outputs from the network
    /// </summary>
    public float GetOutputs(int nodeId)
    {
        // Loop through all connections
        foreach ((int connectionId, Connection connection) in _connections)
        {
            // If the connection is enabled, and the connection's input node is the node we are looking for, add the connection's weight to the node's value
            if (connection.Enabled && connection.FromNodeId == nodeId)
            {
                _nodes[nodeId].Value += _nodes[connection.FromNodeId].Value * connection.Weight;
            }
        }

        // Return the node's value
        return _nodes[nodeId].Value;
    }

    public void DisplayNetworkOnGui()
    {
        // 1. Create a list of nodes, sorted by their layer
        // 2. Position squares that represent the nodes on the GUI, based on their layer
        // 3. Display the nodes on the GUI as small squares
        // 4. Display the connections on the GUI as lines
        // 5. Display the node values on the GUI as small text
        // 6. Display the connection weights on the GUI as small text

        // Create a list of nodes, sorted by their layer
        List<Node> nodes = _nodes.Values.OrderBy(x => x.NodeLayer).ToList();

        // Loop through all nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            // Display the nodes on the GUI as small squares
            GUI.Box(new Rect(100 + (i * 100), 100 + (nodes[i].NodeLayer * 100), 50, 50), nodes[i].Value.ToString());

            // Display the node values on the GUI as small text
            GUI.Label(new Rect(100 + (i * 100), 100 + (nodes[i].NodeLayer * 100), 50, 50), nodes[i].Value.ToString());
        }

        // Loop through all connections
        foreach ((int connectionId, Connection connection) in _connections)
        {
            // If the connection is enabled, display the connection on the GUI as a line
            if (connection.Enabled)
            {
                // Get the node that the connection is coming from
                Node fromNode = _nodes[connection.FromNodeId];

                // Get the node that the connection is going to
                Node toNode = _nodes[connection.ToNodeId];

                // Display the connection on the GUI as a line
                GUI.Box(new Rect(100 + (fromNode.NodeLayer * 100), 100 + (fromNode.NodeLayer * 100), 50, 50), "");
                GUI.Box(new Rect(100 + (toNode.NodeLayer * 100), 100 + (toNode.NodeLayer * 100), 50, 50), "");

                // Display the connection weights on the GUI as small text
                GUI.Label(new Rect(100 + (fromNode.NodeLayer * 100), 100 + (fromNode.NodeLayer * 100), 50, 50), connection.Weight.ToString());
            }
        }


    }
    #endregion
}
