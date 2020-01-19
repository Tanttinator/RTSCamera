using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCamera
{
    /// <summary>
    /// Optional input handler for the 2D camera controller.
    /// </summary>
    [RequireComponent(typeof(RTSCameraController2D))]
    public class RTSCameraInput2D : MonoBehaviour
    {
        [Header("Movement Types")]
        public bool wasd = true;

        public bool drag = true;

        public bool zoom = true;

        public bool moveToTarget = true;

        new RTSCameraController2D camera;

        private void Awake()
        {
            camera = GetComponent<RTSCameraController2D>();
        }

        private void Update()
        {
            if(wasd)
            {
                camera.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            }

            if(drag)
            {
                if (Input.GetMouseButtonDown(0))
                    camera.Drag(true);
                if (Input.GetMouseButton(0))
                    camera.Drag();
            }

            if(zoom)
            {
                camera.Zoom(Input.GetAxis("Mouse ScrollWheel"));
            }

            if (moveToTarget && Input.GetMouseButtonDown(2))
                camera.MoveTowards(camera.GetMousePos());
        }
    }
}
