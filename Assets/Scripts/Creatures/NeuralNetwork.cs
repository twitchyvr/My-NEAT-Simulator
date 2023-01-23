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
    [SerializeField] public List<Node> Nodes = new();
    [SerializeField] public List<Connection> Connections = new();
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
            Nodes.Add(inNode);
            nodeId++;
        }

        // Add an additional input node for bias
        Node biasNode = new(nodeId, Node.NodeType.Bias);
        if (addBiasNode)
        {
            biasNode.Value = _defaultBias;
            biasNode.NodeLayer = 1;
            Nodes.Add(biasNode);
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
            Nodes.Add(outNode);
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
                        Connections.Add(connection);
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method adds a node to the network
    /// </summary>
    public int AddANode()
    {
        Debug.Log("Adding a node");
        // Select a random connection
        int connId = UnityEngine.Random.Range(1, Connections.Count);

        // If the connection is already disabled, return
        if (!Connections[connId].Enabled)
        {
            return -1;
        }

        // Disable the connection
        Connections[connId].Enabled = false;

        // Create a new node
        int nodeId = Nodes.Count + 1;
        Node newNode = new(nodeId, Node.NodeType.Hidden)
        {
            Value = 0,
            NodeLayer = Nodes[Connections[connId].FromNodeId].NodeLayer + 1
        };
        // Add the new node to the network
        Nodes.Add(newNode);

        // Create two new connections
        int connId1 = Connections[connId].FromNodeId * 100000 + nodeId;
        int connId2 = nodeId * 100000 + Connections[connId].ToNodeId;
        Connection newConn1 = new(connId1, Connections[connId].FromNodeId, nodeId, 1f, true, true);
        Connection newConn2 = new(connId2, nodeId, Connections[connId].ToNodeId, Connections[connId].Weight, true, true);
        // Add the new connections to the array, but first we need to add the new connections to the end of the array
        Connections.Add(newConn1);
        Connections.Add(newConn2);

        int _maxLayer = 0;
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].NodeLayer > _maxLayer)
            {
                _maxLayer = Nodes[i].NodeLayer;
            }
        }
        _maxLayer = _maxLayer + 1;

        // Update the layer property of all nodes, and move them to the correct layer.  Don't simply move hidden nodes to the highest layer.  Use logic to determine which layer each hidden node is in.
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].Type == Node.NodeType.Input || Nodes[i].Type == Node.NodeType.Bias)
            {
                Nodes[i].NodeLayer = 1;
            }
            else if (Nodes[i].Type == Node.NodeType.Output)
            {
                Nodes[i].NodeLayer = _maxLayer;
            }
            else
            {
                // This is a hidden node
                // Find the layer of the node that is connected to this hidden node
                int layer = 1;
                for (int j = 0; j < Connections.Count; j++)
                {
                    if (Connections[j].ToNodeId == Nodes[i].Id)
                    {
                        layer = Nodes[Connections[j].FromNodeId].NodeLayer;
                    }
                }
                Nodes[i].NodeLayer = layer + 1;
            }
        }

        return nodeId;

    }

    /// <summary>
    /// This method adds a connection to the network
    /// </summary>
    public int AddAConnection()
    {
        Debug.Log("Adding a connection");
        // Select two random nodes
        int node1 = UnityEngine.Random.Range(1, Nodes.Count);
        int node2 = UnityEngine.Random.Range(1, Nodes.Count);
        int newConnId = node1 * 100000 + node2;

        // If the connection already exists in the array, then return
        foreach (Connection conn in Connections)
        {
            if (conn.FromNodeId == node1 && conn.ToNodeId == node2)
            {
                return -1;
            }
        }

        // If the connection is from an input node to an output node, return
        if (Nodes[node1].Type == Node.NodeType.Input && Nodes[node2].Type == Node.NodeType.Output)
        {
            return -1;
        }

        // If the connection is from a bias node to an output node, return
        if (Nodes[node1].Type == Node.NodeType.Bias && Nodes[node2].Type == Node.NodeType.Output)
        {
            return -1;
        }

        // If the connection is from an input node to a hidden node, create the connection
        if (Nodes[node1].Type == Node.NodeType.Input && Nodes[node2].Type == Node.NodeType.Hidden)
        {

            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(newConnId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            Connections.Add(connection);
        }

        // If the connection is from a hidden node to an output node, create the connection
        if (Nodes[node1].Type == Node.NodeType.Hidden && Nodes[node2].Type == Node.NodeType.Output)
        {
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(newConnId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            Connections.Add(connection);
        }

        // If the connection is from a hidden node to a hidden node on a different layer, create the connection
        if (Nodes[node1].Type == Node.NodeType.Hidden && Nodes[node2].Type == Node.NodeType.Hidden && Nodes[node1].NodeLayer != Nodes[node2].NodeLayer)
        {
            bool isEnabled = true;
            if (UnityEngine.Random.Range(1, 100) < 5)
            {
                isEnabled = false;
            }
            Connection connection = new(newConnId, node1, node2, UnityEngine.Random.Range(-20f, 20f), isEnabled, false);
            Connections.Add(connection);
        }

        // If the connection is from a hidden node to a hidden node on the same layer, and the connection is already enabled, disable it
        if (Nodes[node1].Type == Node.NodeType.Hidden && Nodes[node2].Type == Node.NodeType.Hidden && Nodes[node1].NodeLayer == Nodes[node2].NodeLayer)
        {

            foreach (Connection conn in Connections)
            {
                if (conn.FromNodeId == newConnId)
                {
                    conn.Enabled = false;
                }
            }
        }

        // return the id of the connection
        return newConnId;
    }

    /// <summary>
    /// This method removes a connection from the network. It does not remove the nodes that the connection is connected to.
    /// </summary>
    /// <param name="connId">The id of the connection to be removed</param>
    public void RemoveAConnection(int connId)
    {
        Debug.Log("Removing a connection");
        // If the connection is not enabled, return
        if (!Connections[connId].Enabled)
        {
            return;
        }

        // If the connection is enabled, disable it
        Connections[connId].Enabled = false;
    }

    /// <summary>
    /// This method removes a specified node from the network. It also removes all connections to and from the node.
    /// </summary>
    /// <param name="nodeId">The id of the node to be removed</param>
    public void RemoveANode(int nodeId)
    {
        Debug.Log("Removing a node");
        // If the node is an input node, return
        if (Nodes[nodeId].Type == Node.NodeType.Input)
        {
            return;
        }

        // If the node is an output node, return
        if (Nodes[nodeId].Type == Node.NodeType.Output)
        {
            return;
        }

        // If the node is a bias node, return
        if (Nodes[nodeId].Type == Node.NodeType.Bias)
        {
            return;
        }

        // If the node is a hidden node, remove it
        if (Nodes[nodeId].Type == Node.NodeType.Hidden)
        {
            // Remove all connections to and from the node
            foreach (Connection conn in Connections)
            {
                if (conn.FromNodeId == nodeId || conn.ToNodeId == nodeId)
                {
                    conn.Enabled = false;
                }
            }

            // Remove the node from the array
            Nodes[nodeId] = null;
        }
    }

    /// <summary>
    /// This method will mutate the network
    /// </summary>
    public void Mutate()
    {
        Debug.Log("Mutating the network");
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
        for (int i = 0; i < Nodes.Count; i++)
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

        int _maxLayer = 1;
        // Find the maximum layer
        for (int i = 0; i < Nodes.Count; i++)
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

                            try
                            {
                                node.Value += Nodes[connection.FromNodeId].Value * connection.Weight;
                            }
                            catch (Exception e)
                            {
                                Debug.Log("--------------------");
                                Debug.Log("Exception: " + e.Message);
                                Debug.Log("Node ID: " + node.Id);
                                Debug.Log("Node Type: " + node.Type);
                                Debug.Log("Node Layer: " + node.NodeLayer);
                                Debug.Log("From Node ID: " + connection.FromNodeId);
                                Debug.Log("To Node ID: " + connection.ToNodeId);
                                Debug.Log("Connection Weight: " + connection.Weight);
                                Debug.Log("Connection Enabled: " + connection.Enabled);
                            }
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

    /// <summary>
    /// This method will compare the topologies of two neural networks on the basis of their topology distance metric and weight distance metric
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare to</param>
    /// <returns>The topology distance between the two neural networks. A value of zero means the neural networks are identical</returns>
    internal float CompareTopologies(NeuralNetwork neuralNetwork)
    {
        // Get the topology distance
        return GetTopologyDistance(neuralNetwork);

    }

    /// <summary>
    /// The GetTopologyDistance method will return the topology distance between two neural networks.
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare to</param>
    /// <returns>The topology distance between the two neural networks</returns>
    private float GetTopologyDistance(NeuralNetwork neuralNetwork)
    {
        // Get the number of excess and disjoint genes
        int[] excessDisjointGenes = GetExcessDisjointGenes(neuralNetwork);
        float excessDisjointResult = excessDisjointGenes[0] + excessDisjointGenes[1];
        float averageWeightDifference = GetAverageWeightDifference(neuralNetwork);
        float compatibilityDifference = excessDisjointResult + averageWeightDifference;
        return compatibilityDifference;
    }

    /// <summary>
    /// This method will get the average weight difference between the genomes of two neural networks
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare to</param>
    /// <returns>The average weight difference between the genomes of the two neural networks</returns>
    private float GetAverageWeightDifference(NeuralNetwork neuralNetwork)
    {
        // Get the number of matching genes
        int matchingGenes = GetMatchingGenes(neuralNetwork);

        // Get the total weight difference
        float totalWeightDifference = 0;
        foreach (Connection connection in Connections)
        {
            foreach (Connection connection2 in neuralNetwork.Connections)
            {
                if (connection.InnovationId == connection2.InnovationId)
                {
                    totalWeightDifference += Math.Abs(connection.Weight - connection2.Weight);
                }
            }
        }

        // Return the average weight difference
        return totalWeightDifference / matchingGenes;
    }

    /// <summary>
    /// This method will get the number of matching genes in the genome of this neural network
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare to</param>
    /// <returns>The number of matching genes</returns>
    private int GetMatchingGenes(NeuralNetwork neuralNetwork)
    {
        // Get the number of matching genes
        int matchingGenes = 0;
        foreach (Connection connection in Connections)
        {
            foreach (Connection connection2 in neuralNetwork.Connections)
            {
                if (connection.InnovationId == connection2.InnovationId)
                {
                    matchingGenes++;
                }
            }
        }

        // Return the number of matching genes
        return matchingGenes;
    }

    /// <summary>
    /// This method will get the number of excess and disjoint genes in the genome of this neural network
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare to</param>
    /// <returns>The number of excess and disjoint genes in an array of ints</returns>
    private int[] GetExcessDisjointGenes(NeuralNetwork neuralNetwork)
    {
        // Get the number of excess and disjoint genes
        int[] excessDisjointGenes = new int[2];

        // Get the number of excess genes
        excessDisjointGenes[0] = GetExcessGenes(neuralNetwork);

        // Get the number of disjoint genes
        excessDisjointGenes[1] = GetDisjointGenes(neuralNetwork);

        // Return the number of excess and disjoint genes
        return excessDisjointGenes;
    }

    /// <summary>
    /// This method will get the number of excess genes in the genome of this neural network
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare to</param>
    /// <returns>The number of excess genes</returns>
    private int GetExcessGenes(NeuralNetwork neuralNetwork)
    {
        // Get the number of excess genes
        int excessGenes = 0;

        // Get the number of genes in the shortest genome
        int shortestGenome = Math.Min(Connections.Count, neuralNetwork.Connections.Count);

        // Loop through all connections
        for (int i = 0; i < shortestGenome; i++)
        {
            // If the innovation number of the connection in this genome is greater than the innovation number of the connection in the other genome
            if (Connections[i].InnovationId > neuralNetwork.Connections[i].InnovationId)
            {
                // Increment the number of excess genes
                excessGenes++;
            }
        }

        // Return the number of excess genes
        return excessGenes;
    }

    /// <summary>
    /// This method will get the number of disjoint genes between two neural networks
    /// </summary>
    /// <param name="neuralNetwork">The neural network to compare this neural network to</param>
    /// <returns>The number of disjoint genes between this neural network and the other neural network</returns>
    private int GetDisjointGenes(NeuralNetwork neuralNetwork)
    {
        // Get the number of disjoint genes
        int disjointGenes = 0;

        // Get the number of genes in the shortest genome
        int shortestGenome = Math.Min(Connections.Count, neuralNetwork.Connections.Count);

        // Loop through all connections
        for (int i = 0; i < shortestGenome; i++)
        {
            // If the innovation number of the connection in this genome is less than the innovation number of the connection in the other genome
            if (Connections[i].InnovationId < neuralNetwork.Connections[i].InnovationId)
            {
                // Increment the number of disjoint genes
                disjointGenes++;
            }
        }

        // Return the number of disjoint genes
        return disjointGenes;
    }

    #endregion

    #region Activation Functions
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

    #endregion
}
