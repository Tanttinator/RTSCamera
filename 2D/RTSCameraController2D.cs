using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCamera
{
    /// <summary>
    /// RTS camera controls for 2D.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class RTSCameraController2D : MonoBehaviour
    {
        new Camera camera;

        [Header("Movement speeds")]
        [SerializeField] float moveSpeed = 5f;

        [SerializeField] float zoomSpeed = 5f;

        [Header("Constraints")]
        [SerializeField] Rect moveArea;
        [SerializeField] float minZoom = 2f;
        [SerializeField] float maxZoom = 10f;

        Vector2 dragPos;

        Vector2 moveTarget;
        public bool isMovingToTarget { get; protected set; } = false;

        /// <summary>
        /// Try to move the camera in the given direction.
        /// </summary>
        /// <param name="dir">The point relative to the camera.</param>
        public void Translate(Vector2 dir)
        {
            float x = dir.x;
            if ((transform.position.x < moveArea.x && x < 0) || (transform.position.x > moveArea.xMax && x > 0))
                x = 0;
            float y = dir.y;
            if ((transform.position.y < moveArea.y && y < 0) || (transform.position.y > moveArea.yMax && y > 0))
                y = 0;
            transform.Translate(new Vector3(x, y, 0f));
        }

        /// <summary>
        /// Move the camera using the keyboard.
        /// </summary>
        /// <param name="dir"></param>
        public void Move(Vector2 dir)
        {
            if(dir.magnitude > 0)
                isMovingToTarget = false;
            Translate(dir * moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Drag the camera with the mouse.
        /// </summary>
        /// <param name="start">Did the drag start this frame?</param>
        public void Drag(bool start = false)
        {
            isMovingToTarget = false;
            Vector2 newPos = GetMousePos();
            if (!start)
                Move(dragPos - newPos);
            dragPos = GetMousePos();
        }

        /// <summary>
        /// Change the camera zoom by an amount.
        /// </summary>
        /// <param name="amount">Positive to zoom out, negative to zoom in.</param>
        public void Zoom(float amount)
        {
            if ((camera.orthographicSize < minZoom && amount > 0) || (camera.orthographicSize > maxZoom && amount < 0))
                return;
            camera.orthographicSize -= amount * Time.deltaTime * zoomSpeed;
        }

        /// <summary>
        /// Move the camera towards the position over time.
        /// </summary>
        /// <param name="pos"></param>
        public void MoveTowards(Vector2 pos)
        {
            moveTarget = pos;
            isMovingToTarget = true;
        }

        Vector2 GetMousePos()
        {
            return camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (isMovingToTarget)
            {
                Translate((moveTarget - (Vector2)transform.position).normalized * Time.deltaTime * moveSpeed * 1.5f);
                if (Vector2.Distance(transform.position, moveTarget) < 0.1f)
                    isMovingToTarget = false;
            }
        }
    }
}
