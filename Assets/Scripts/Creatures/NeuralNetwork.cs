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

    }

    /// <summary>
    /// This method adds a connection to the network
    /// </summary>
    public void AddAConnection()
    {

    }

    /// <summary>
    /// This method will mutate the network
    /// </summary>
    public void Mutate()
    {

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
            _nodes[nodeId].Value = node.Value;
        }

        return _nodes;
    }

    /// <summary>
    /// This method will run the network
    /// </summary>
    public void RunTheNetwork()
    {

    }

    /// <summary>
    /// This method will get the outputs from the network
    /// </summary>
    public void GetOutputs()
    {

    }

    #endregion
}
