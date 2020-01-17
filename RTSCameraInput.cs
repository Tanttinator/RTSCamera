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

        //Speed of camera movement
        public float speed = 5f;

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
                Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;
                camera.Move(dir);
            }
        }
    }
}
