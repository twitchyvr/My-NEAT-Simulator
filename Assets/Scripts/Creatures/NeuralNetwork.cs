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
    [SerializeField] public Node[] Nodes = new Node[] { }; //{ get { return _nodes; } }
    [SerializeField] public Connection[] Connections = new Connection[] { }; // { get { return _connections; } }
    [SerializeField] public int SpeciesId;
    #endregion

    #region Private Variables
    private float _defaultBias = 0.25f;
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
    public NeuralNetwork(int[] netLayers)
    {
        if (netLayers != null)
        {
            // Init with the first and last layers
            Initialize(netLayers[0], netLayers[^1]);
        }
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
        // Loop through the input, bias, output, and hidden nodes and enters them into the array
        // The bias node is always the last node in the input layer
        // Hidden nodes get added later during the mutation phase
        // Layer property of all nodes (except the input and bias nodes) get checked and are updated if needed
        // Their location in the node array is always the same.

        int nodeId = 1;
        int connId = -1;

        // Create input nodes
        for (int i = 1; i <= inputNodesCount; i++)
        {
            Node inNode = new(nodeId, Node.NodeType.Input)
            {
                Value = UnityEngine.Random.Range(-20f, 20f),
                NodeLayer = 1
            };
            // Add the node to the array
            Node[] tempNodes = Nodes;
            Nodes = new Node[tempNodes.Length + 1];
            Array.Copy(tempNodes, Nodes, tempNodes.Length);
            Nodes[nodeId - 1] = inNode;
            nodeId++;
        }

        // Add an additional input node for bias
        Node biasNode = new(nodeId, Node.NodeType.Bias);
        if (addBiasNode)
        {
            biasNode.Value = _defaultBias;
            biasNode.NodeLayer = 1;
            Node[] tempNodes = Nodes;
            Nodes = new Node[tempNodes.Length + 1];
            Array.Copy(tempNodes, Nodes, tempNodes.Length);
            Nodes[nodeId - 1] = biasNode;
            nodeId++;
        }

        // Create output nodes
        for (int i = 1; i <= outputNodesCount; i++)
        {
            Node outNode = new(nodeId, Node.NodeType.Output)
            {
                Value = 0,
                NodeLayer = 2
            };
            Node[] tempNodes = Nodes;
            Nodes = new Node[tempNodes.Length + 1];
            Array.Copy(tempNodes, Nodes, tempNodes.Length);
            Nodes[nodeId - 1] = outNode;
            nodeId++;
        }

        // Loop through all nodes and create connections
        foreach (Node thisNode in Nodes)
        {
            // If the node is an input or bias node, connect it to all output nodes
            if (thisNode.Type == Node.NodeType.Input || thisNode.Type == Node.NodeType.Bias)
            {
                // Loop through all output nodes
                foreach (Node otherNode in Nodes)
                {
                    // If the node is an output node, create a connection
                    if (otherNode.Type == Node.NodeType.Output)
                    {
                        // Create a connection
                        connId = thisNode.Id * 100000 + otherNode.Id;
                        bool isEnabled = true;

                        // 5% chance of disabling the connection
                        if (UnityEngine.Random.Range(1, 100) < 5)
                        {
                            isEnabled = false;
                        }

                        Connection connection = new(connId, thisNode.Id, otherNode.Id, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
                        Connection[] tempConnections = Connections;
                        Connections = new Connection[connId + 1];
                        Array.Copy(tempConnections, Connections, tempConnections.Length);
                        Connections[connId] = connection;
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
        int connId = UnityEngine.Random.Range(1, Connections.Length);

        // If the connection is already disabled, return
        if (!Connections[connId].Enabled)
        {
            return;
        }

        // Disable the connection
        Connections[connId].Enabled = false;

        // Create a new node
        int nodeId = Nodes.Length + 1;
        Node newNode = new(nodeId, Node.NodeType.Hidden)
        {
            Value = 0,
            NodeLayer = Nodes[Connections[connId].FromNodeId].NodeLayer + 1
        };
        // Add the new node to the network
        Nodes[nodeId] = newNode;

        // Create two new connections
        int connId1 = Connections[connId].FromNodeId * 100000 + nodeId;
        int connId2 = nodeId * 100000 + Connections[connId].ToNodeId;
        Connection newConn1 = new(connId1, Connections[connId].FromNodeId, nodeId, 1f, true, true);
        Connection newConn2 = new(connId2, nodeId, Connections[connId].ToNodeId, Connections[connId].Weight, true, true);
        // Add the new connections to the array, but first we need to add the new connections to the end of the array
        Connection[] tempArray = new Connection[Connections.Length + 2];
        Connections.CopyTo(tempArray, 0);
        tempArray[Connections.Length] = newConn1;
        tempArray[Connections.Length + 1] = newConn2;
        Connections = tempArray;

        // Update the layer property of all nodes
        foreach (Node node in Nodes)
        {
            if (node.Id == Connections[connId].FromNodeId)
            {
                node.NodeLayer = Nodes[Connections[connId].FromNodeId].NodeLayer + 1;
            }
        }

    }

    /// <summary>
    /// This method adds a connection to the network
    /// </summary>
    public void AddAConnection()
    {
        // Select two random nodes
        int node1 = UnityEngine.Random.Range(1, Nodes.Length);
        int node2 = UnityEngine.Random.Range(1, Nodes.Length);

        // If the connection already exists in the array, then return
        foreach (Connection conn in Connections)
        {
            if (conn.FromNodeId == node1 && conn.ToNodeId == node2)
            {
                return;
            }
        }

        // If the connection is from an input node to an output node, return
        if (Nodes[node1].Type == Node.NodeType.Input && Nodes[node2].Type == Node.NodeType.Output)
        {
            return;
        }

        // If the connection is from a bias node to an output node, return
        if (Nodes[node1].Type == Node.NodeType.Bias && Nodes[node2].Type == Node.NodeType.Output)
        {
            return;
        }

        // If the connection is from an input node to a hidden node, create the connection
        if (Nodes[node1].Type == Node.NodeType.Input && Nodes[node2].Type == Node.NodeType.Hidden)
        {
            int connId = node1 * 100000 + node2;
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(connId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            Connections[connId] = connection;
        }

        // If the connection is from a hidden node to an output node, create the connection
        if (Nodes[node1].Type == Node.NodeType.Hidden && Nodes[node2].Type == Node.NodeType.Output)
        {
            int connId = node1 * 100000 + node2;
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(connId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            Connections[connId] = connection;
        }

        // If the connection is from a hidden node to a hidden node on a different layer, create the connection
        if (Nodes[node1].Type == Node.NodeType.Hidden && Nodes[node2].Type == Node.NodeType.Hidden && Nodes[node1].NodeLayer != Nodes[node2].NodeLayer)
        {
            int connId = node1 * 100000 + node2;
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(connId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            Connections[connId] = connection;
        }

        // If the connection is from a hidden node to a hidden node on the same layer, and the connection is already enabled, disable it
        if (Nodes[node1].Type == Node.NodeType.Hidden && Nodes[node2].Type == Node.NodeType.Hidden && Nodes[node1].NodeLayer == Nodes[node2].NodeLayer)
        {
            int connId = node1 * 100000 + node2;

            foreach (Connection conn in Connections)
            {
                if (conn.FromNodeId == connId)
                {
                    conn.Enabled = false;
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
        foreach (Connection conn in Connections)
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
        foreach (Node node in Nodes)
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
    public Node[] LoadInputs(Node[] inputs)
    {
        // Load the inputs into the input nodes
        foreach (Node inputNode in inputs)
        {
            // If the node exists, set the value
            foreach (Node thisNode in Nodes)
            {
                if (thisNode.Id == inputNode.Id && (Nodes[inputNode.Id].Type == Node.NodeType.Input || Nodes[inputNode.Id].Type == Node.NodeType.Bias))
                {
                    thisNode.Value = inputNode.Value;
                }
            }
        }

        Node[] inputNodes = new Node[] { };
        for (int i = 0; i < Nodes.Length; i++)
        {
            Node node = Nodes[i];
            // If the node is an input node, add it to the input node array
            if (node.Type == Node.NodeType.Input || node.Type == Node.NodeType.Bias)
            {
                // Add the node to the input node array and resize the array
                Array.Resize(ref inputNodes, inputNodes.Length + 1);
                inputNodes[^1] = node;
            }
        }

        // Return the output nodes
        return inputNodes;
    }

    /// <summary>
    /// This method will run the network
    /// </summary>
    public void RunTheNetwork()
    {
        // Propagate the values through the network to the output nodes
        // This works by starting at layer 2, then scanning through the node array
        // looking for nodes which have been placed in this layer.

        // When it finds one, it sets its input to zero, then it scans through the connection array
        // looking for connections that terminate at this node

        // When it finds one, it checks to see where that connection originated, and grabs the output value from that node,
        // multiplies it by the connection weight and adds the result to the input value of the target node

        // Then it continues scanning through the connection array looking for more connections that terminate at this node
        // and repeats the process until it gets to the end.

        // This approach captures all the connections going to that node, and once the input is known, we apply the activation function
        // and use the return value to populate the output value of the node.

        // Then goes back to scanning through the node array looking for nodes in the next layer, and repeats the process until it gets to the end.
        // This approach is a little more efficient than the approach used in the original NEAT paper, but it is functionally equivalent.

        int _maxLayer = 0;
        // Find the maximum layer
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (Nodes[i].NodeLayer > _maxLayer)
            {
                _maxLayer = Nodes[i].NodeLayer;
            }
        }

        // Loop through all layers starting at layer 2
        for (int layer = 2; layer <= _maxLayer; layer++)
        {
            // Loop through all nodes
            foreach (Node node in Nodes)
            {
                // If the node is in the current layer (starting at layer 2)
                // Set the node's value to zero
                if (node.NodeLayer == layer)
                {
                    node.Value = 0;

                    // Then, loop through all connections
                    foreach (Connection connection in Connections)
                    {
                        if (connection == null) return;

                        // If the connection is enabled, and the connection's input node is the node we are looking for, add the connection's weight to the node's value
                        if (connection.Enabled && connection.FromNodeId == node.Id)
                        {
                            node.Value += Nodes[connection.FromNodeId].Value * connection.Weight;
                        }
                    }

                    // Next, apply the activation function to the node's value
                    node.Value = ActivationFunction(node.Value);
                }
            }
        }

    }

    /// <summary>
    /// This method will give the output value from a specified node.
    /// </summary>
    /// <param name="nodeId">The node ID to get the output value from</param>
    /// <returns>The output value from the specified node</returns>
    public float GetOutput(int pathNodeId)
    {
        // Loop through all connections
        foreach (Connection connection in Connections)
        {
            // If the connection is enabled, and the connection's input node is the node we are looking for, add the connection's weight to the node's value
            if (connection.Enabled && connection.FromNodeId == pathNodeId)
            {
                Nodes[pathNodeId].Value += Nodes[connection.FromNodeId].Value * connection.Weight;
            }
        }

        // Return the node's value
        return Nodes[pathNodeId].Value;
    }

    /// <summary>
    /// The Sigmoid function
    /// </summary>
    /// <param name="x">The value to pass through the Sigmoid function</param>
    /// <returns>The result of the Sigmoid function (a float between 0 and 1)</returns>
    public float Sigmoid(float x)
    {
        // Sigmoid function with a range of 0 to 1
        return (float)(1 / (1 + Math.Exp(-4.9 * x)));
    }

    /// <summary>
    /// The Tanh function
    /// </summary>
    /// <param name="x">The value to pass through the Tanh function</param>
    /// <returns>The result of the Tanh function (a float between -1 and 1)</returns>
    public float Tanh(float x)
    {
        // Tanh function with a range of -1 to 1
        return (float)Math.Tanh(x);
    }

    /// <summary>
    /// The ActivationFunction method will return the result of the activation function
    /// </summary>
    /// <param name="x">The value to pass through the activation function</param>
    /// <param name="useSigmoid">If true, the Sigmoid function will be used. If false, the Tanh function will be used.</param>
    /// <returns>The result of the activation function</returns>
    public float ActivationFunction(float x, bool useSigmoid = false)
    {
        if (useSigmoid)
        {
            return Sigmoid(x);
        }
        else
        {
            return Tanh(x);
        }
    }

    public void Save(string path)
    {
        using StreamWriter writer = new(path);
        writer.Write(JsonUtility.ToJson(this, true));
        writer.Close();
    }

    public void Load(string path)
    {
        using StreamReader reader = new(path);
        JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), this);
        reader.Close();
    }

    #endregion
}
