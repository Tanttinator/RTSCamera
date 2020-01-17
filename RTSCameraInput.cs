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

        //Speed of camera movement
        public float moveSpeed = 5f;

        //Speed of pivot rotation
        public float pivotSpeed = 5f;

        new RTSCameraController camera;

        Vector2 dragPos;

        Vector2 pivotPos;

        private void Awake()
        {
            camera = GetComponent<RTSCameraController>();
        }

        void Update()
        {
            //Keyboard movement
            if(wasd)
            {
                Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
                camera.Move(dir);
            }

            //Drag movement
            if(dragging)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    dragPos = camera.GetMouseIntersection();
                }
                if(Input.GetMouseButton(0))
                {
                    Vector2 newPos = camera.GetMouseIntersection();
                    camera.Move(dragPos - newPos);
                    dragPos = camera.GetMouseIntersection();
                }
            }

            if(panPivot || tiltPivot)
            {
                if (Input.GetMouseButtonDown(1))
                    pivotPos = Input.mousePosition;
                if(Input.GetMouseButton(1))
                {
                    Vector2 newPos = Input.mousePosition;
                    if(panPivot)
                        camera.PanPivot(-(pivotPos - newPos).x * Time.deltaTime * pivotSpeed);
                    if (tiltPivot)
                        camera.TiltPivot((pivotPos - newPos).y * Time.deltaTime * pivotSpeed);
                    pivotPos = newPos;
                }
            }
        }
    }
}
