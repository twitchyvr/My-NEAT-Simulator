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

    #endregion

    #region Private Variables
    private int[] _netLayers;
    private Dictionary<int, Node> _nodes = new();
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
            _netLayers = new int[2] { 8, 2 };

        int nodeId = 0;
        int connId = 0;

        // Create nodes for each layer
        for (int i = 0; i < _netLayers.Length; i++)
        {
            for (int j = 0; j < _netLayers[i]; j++)
            {
                Node.NodeType type = Node.NodeType.Hidden;
                if (i == 0)
                    type = Node.NodeType.Input;
                else if (i == _netLayers.Length - 1)
                    type = Node.NodeType.Output;

                Node node = new Node(nodeId, type, i);
                _nodes.Add(nodeId, node);
                nodeId++;
            }
        }


    }

    public NeuralNetwork(int[] netLayers = null)
    {
        Init(netLayers);
    }

    #endregion
}
