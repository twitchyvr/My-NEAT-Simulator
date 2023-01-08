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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.UI;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.Serialization;
#endregion

public class CameraManager : MonoBehaviour
{

    #region Settable Variables
    [SerializeField] Camera _cam;

    #endregion
    #region Private Variables
    private Vector3 _lastMousePos;
    #endregion
    #region Init
    protected void Awake()
    {
        _cam = _cam != null ? _cam : Camera.main;

    }

    #endregion
    #region Loop
    protected void LateUpdate()
    {
        // Zoom if the user scrolls the mouse wheel
        if (Input.mouseScrollDelta.y != 0)
        {
            _cam.orthographicSize -= Input.mouseScrollDelta.y;
        }

        // Use the right mouse button to drag the orthographic camera around the x and z positions, while y stays at 10.85
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = _lastMousePos - Input.mousePosition;
            delta = new Vector3(delta.x, 0, delta.y);
            delta *= 0.25f;
            delta = new Vector3(delta.x, 0, delta.z);
            _cam.transform.position += delta;
        }

        _lastMousePos = Input.mousePosition;
    }

    #endregion
    #region Methods

    #endregion
}
