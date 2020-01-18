using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCamera
{
    /// <summary>
    /// Optional input class for the controller.
    /// </summary>
    [RequireComponent(typeof(RTSCameraController))]
    public class RTSCameraInput : MonoBehaviour
    {
        [Header("Types of movement")]
        //Keyboard movement
        public bool wasd = true;

        //Drag movement
        public bool dragging = true;

        //Pivot rotation
        public bool panPivot = true;
        public bool tiltPivot = true;

        //Camera rotation
        public bool panCamera = true;
        public bool tiltCamera = true;

        //Zooming
        public bool zooming = true;

        //Moving to target
        public bool moveToTarget = true;

        new RTSCameraController camera;

        private void Awake()
        {
            camera = GetComponent<RTSCameraController>();
        }

        void Update()
        {
            //Keyboard movement
            if(wasd)
            {
                camera.Dolly(Input.GetAxis("Vertical"));
                camera.Trucking(Input.GetAxis("Horizontal"));
            }

            //Drag movement
            if(dragging)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    camera.DragMove(true);
                }
                if(Input.GetMouseButton(0))
                {
                    camera.DragMove();
                }
            }

            //Pivot turning
            if (Input.GetMouseButton(1))
            {
                if (panPivot)
                    camera.Pan(Input.GetAxis("Mouse X"));

                if (tiltPivot)
                    camera.Tilt(Input.GetAxis("Mouse Y"));
            }

            //Zooming
            if (zooming && Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.001f)
                camera.Zoom(Input.GetAxis("Mouse ScrollWheel"));

            //Moving to target
            if (moveToTarget && Input.GetMouseButtonDown(2))
                camera.MoveTowards(camera.GetMouseIntersection());
        }
    }
}
