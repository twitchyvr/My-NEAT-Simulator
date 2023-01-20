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

[Serializable]
public class Node
{

    #region Private Variables
    private int _id = 0;
    private float _value = -1;  // The value of this node

    private NodeType _type;
    private float _inputSum = 0;
    private float _outputSum = 0;
    private int _nodeLayer = 0;
    private float _desiredValue = 0;

    private bool _enabled = false;

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

    public float DesiredValue
    {
        get { return _desiredValue; }
        set { _desiredValue = value; }
    }
    #endregion
    #region Enums
    public enum NodeType
    {
        Input,
        Bias,
        Hidden,
        Output
    }

    #endregion

    #region Methods
    public Node(int id, bool isEnabled = true)
    {
        _id = id;
        _type = NodeType.Input;
        _value = 0;
        _inputSum = 0;
        _outputSum = 0;
        _nodeLayer = 0;
        _enabled = isEnabled;
    }

    public Node(int id, NodeType type, bool isEnabled = true)
    {
        _id = id;
        _type = type;
        _value = 0;
        _inputSum = 0;
        _outputSum = 0;
        _nodeLayer = 0;
        _enabled = isEnabled;
    }

    public Node(int id, int layer, bool isEnabled = true)
    {
        _id = id;
        _type = NodeType.Input;
        _value = 0;
        _inputSum = 0;
        _outputSum = 0;
        _nodeLayer = layer;
        _enabled = isEnabled;
    }

    public Node(int id, NodeType type, int nodeLayer, bool isEnabled = true)
    {
        _id = id;
        _type = type;
        _nodeLayer = nodeLayer;
        _inputSum = 0;
        _outputSum = 0;
        _value = 0;
        _enabled = isEnabled;
    }

    public Node(int id, NodeType type, int nodeLayer, float value, bool isEnabled = true)
    {
        _id = id;
        _type = type;
        _nodeLayer = nodeLayer;
        _value = value;
        _enabled = isEnabled;
    }

    public void CalculateValue()
    {
        if (_type == NodeType.Input)
        {
            return;
        }

        _value = Tanh(_inputSum);
    }

    public float Sigmoid(float x)
    {
        return 1 / (1 + MathF.Exp(-x));
    }

    public float Tanh(float x)
    {
        return (MathF.Exp(x) - MathF.Exp(-x)) / (MathF.Exp(x) + MathF.Exp(-x)); // This is faster than MathF.Tanh(x)
        //return MathF.Tanh(x);
    }

    public static float[] Softmax(float[] x)
    {
        float[] expX = new float[x.Length];
        float sumExpX = 0;

        for (int i = 0; i < x.Length; i++)
        {
            expX[i] = MathF.Exp(x[i]);
            sumExpX += expX[i];
        }

        for (int i = 0; i < x.Length; i++)
        {
            expX[i] /= sumExpX;
        }

        return expX;
    }

    /// <summary>
    /// Evaluates the node
    /// </summary>
    /// <param name="inputs">The inputs to the network</param>
    public void Evaluate(float[] inputs)
    {
        if (_type == NodeType.Input)
        {
            _value = inputs[_id];
        }
        else if (_type == NodeType.Bias)
        {
            if (inputs[_id] != 0)
                _value = inputs[_id];
            else
                _value = 0.25f;
        }
    }

    /// <summary>
    /// Evaluates the node
    /// </summary>
    /// <param name="inputs">The inputs to the network</param>
    public void Init(float value = -1)
    {
        if (value == -1)
        {
            _value = UnityEngine.Random.Range(-1f, 1f);
        }
        else
        {
            _value = value;
        }
    }

    public override string ToString()
    {
        return "[" + _id + "] = " + _value + " (" + _type + ")";
    }

    /// <summary>
    /// This method mutates the node by generating a new random value between -1 and 1
    /// </summary>
    internal void Mutate()
    {
        // Mutate the node values and connections weights
        _value = UnityEngine.Random.Range(-1f, 1f);
    }

    /// <summary>
    /// This method resets the node to the default value. This is used when resetting the network to its initial state.
    /// </summary>
    /// <param name="defaultValue">The default value to reset the node to. If no value is supplied, it will default to zero.</param>
    internal void Reset(int defaultValue = 0)
    {
        // Reset the node values
        _value = defaultValue;
    }

    /// <summary>
    /// This method copies the node
    /// </summary>
    /// <param name="node">The node to copy</param>
    internal void Copy(Node node)
    {
        node.SetEnabled(false);
        // Create a copy of the node
        _value = node.Value;
        _type = node.Type;
        _inputSum = node.InputSum;
        _outputSum = node.OutputSum;
        _nodeLayer = node.NodeLayer;
        _enabled = true;
    }

    internal float CalculateNodeFitness()
    {
        // Calculate the fitness of the node
        float fitness = 0;
        return fitness;
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
    }

    public bool IsEnabled()
    {
        return _enabled;
    }
    #endregion
}
