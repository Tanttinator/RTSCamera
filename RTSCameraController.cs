using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCamera
{
    /// <summary>
    /// RTS style camera controls.
    /// </summary>
    public class RTSCameraController : MonoBehaviour
    {
        /// <summary>
        /// Move the camera instantly.
        /// </summary>
        /// <param name="dir">The point relative to the cameras position.</param>
        public void Move(Vector2 dir)
        {
            transform.Translate(new Vector3(dir.x, 0f, dir.y), Space.World);
        }
    }
}
