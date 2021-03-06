﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCamera
{
    /// <summary>
    /// RTS style camera controls.
    /// </summary>
    public class RTSCameraController : MonoBehaviour
    {
        new public Camera camera;

        [Header("Movement speeds")]
        public float moveSpeed = 5f;

        public float rotationSpeed = 100f;

        public float zoomSpeed = 5f;

        [Header("Constraints")]
        public bool useConstraints = false;
        public Rect moveArea;

        public float panAngle = 360f;

        public float minTilt = 30f;
        public float maxTilt = 85f;

        public float minZoom = 5f;
        public float maxZoom = 20f;

        [Header("Zoom Tilt")]
        public bool enableZoomTilt = true;
        public float minZoomAngle = 45f;
        public float maxZoomAngle = 85f;

        Vector2 dragPos;

        Vector2 moveTarget;
        bool isMovingTowards = false;

        Vector2 Position => new Vector2(transform.position.x, transform.position.z);

        Vector2 Rotation => new Vector2(Vector3.SignedAngle(transform.forward, new Vector3(transform.forward.x, 0f, transform.forward.z), -transform.right), Vector3.SignedAngle(new Vector3(transform.forward.x, 0f, transform.forward.z), Vector3.forward, -Vector3.up));

        float ZoomLevel => (Mathf.Abs(camera.transform.localPosition.z) - minZoom) / (maxZoom - minZoom);

        #region Low Level

        /// <summary>
        /// Move the camera instantly.
        /// </summary>
        /// <param name="dir">The point relative to the cameras position in world space.</param>
        public void Move(Vector2 dir)
        {
            float x = dir.x;
            if (useConstraints && ((Position.x <= moveArea.x && x < 0) || (Position.x >= moveArea.xMax && x > 0)))
                x = 0;
            float y = dir.y;
            if (useConstraints && ((Position.y <= moveArea.y && y < 0) || (Position.y >= moveArea.yMax && y > 0)))
                y = 0;
            transform.Translate(new Vector3(x, 0f, y), Space.World);
        }

        /// <summary>
        /// Move the camera instantly.
        /// </summary>
        /// <param name="dir">The point relative to the cameras position in local space.</param>
        public void MoveLocal(Vector2 dir)
        {
            Vector2 horizontal = transform.right * dir.x;
            Vector2 vertical = new Vector2(transform.forward.x, transform.forward.z).normalized * dir.y;

            Move(horizontal + vertical);
        }

        /// <summary>
        /// Pan the camera around the pivot by an angle.
        /// </summary>
        /// <param name="angle"></param>
        public void PanAngle(float angle)
        {
            if ((Rotation.y > panAngle / 2f && angle > 0) || (Rotation.y < -panAngle / 2f && angle < 0))
                return;
            transform.Rotate(0f, angle, 0f, Space.World);
        }

        /// <summary>
        /// Tilt the camera around the pivot by an angle.
        /// </summary>
        /// <param name="angle"></param>
        public void TiltAngle(float angle)
        {
            if ((Rotation.x > maxTilt && angle > 0) || (Rotation.x < minTilt && angle < 0))
                return;
            transform.RotateAround(transform.position, transform.right, angle);
        }

        /// <summary>
        /// Zoom the camera by moving it farther / closer to the pivot.
        /// </summary>
        /// <param name="amount"></param>
        public void ZoomDistance(float amount)
        {
            if ((ZoomLevel >= 1f && amount < 0) || (ZoomLevel <= 0f && amount > 0))
                return;

            camera.transform.Translate(new Vector3(0f, 0f, amount));
        }

        /// <summary>
        /// Calculates the point where a ray from the cursor intersects with pivot.y.
        /// </summary>
        /// <returns>Point of intersection in world space.</returns>
        public Vector2 GetMouseIntersection()
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            float distToGround = Mathf.Abs((camera.transform.position.y - transform.position.y) / ray.direction.y);
            Vector3 vectorToGround = ray.direction * distToGround;
            Vector3 intersection = camera.transform.position + vectorToGround;
            return new Vector2(intersection.x, intersection.z);
        }

        #endregion

        #region High Level

        /// <summary>
        /// Move the camera along the Z axis.
        /// </summary>
        /// <param name="dir">Positive to move forwards, negative to move backwards.</param>
        public void Dolly(float dir)
        {
            if (dir != 0)
                isMovingTowards = false;
            MoveLocal(Vector2.up * dir * Time.deltaTime * moveSpeed);
        }

        /// <summary>
        /// Move the camera along the X axis.
        /// </summary>
        /// <param name="dir">Positive to move right, negative to move left.</param>
        public void Trucking(float dir)
        {
            if (dir != 0)
                isMovingTowards = false;
            MoveLocal(Vector2.right * dir * Time.deltaTime * moveSpeed);
        }

        /// <summary>
        /// Move the camera by dragging.
        /// </summary>
        /// <param name="start">Did the drag start this frame?</param>
        public void DragMove(bool start = false)
        {
            isMovingTowards = false;
            Vector2 newPos = GetMouseIntersection();
            if(!start)
                Move(new Vector2(dragPos.x - newPos.x, dragPos.y - newPos.y));
            dragPos = GetMouseIntersection();
        }

        /// <summary>
        /// Pan around the pivot.
        /// </summary>
        /// <param name="dir">Positive to turn clockwise, negative to turn anti-clockwise.</param>
        public void Pan(float dir)
        {
            PanAngle(dir * Time.deltaTime * rotationSpeed);
        }

        /// <summary>
        /// Tilt around the pivot.
        /// </summary>
        /// <param name="dir">Positive to turn down, negative to turn up.</param>
        public void Tilt(float dir)
        {
            TiltAngle(-dir * Time.deltaTime * rotationSpeed);
        }

        /// <summary>
        /// Zoom the camera by moving it farther / closer to the pivot
        /// </summary>
        /// <param name="dir">Positive to zoom in, negative to zoom out</param>
        public void Zoom(float dir)
        {
            float oldLevel = ZoomLevel;

            ZoomDistance(dir * Time.deltaTime * zoomSpeed);

            float levelDiff = Mathf.Abs(oldLevel - ZoomLevel);

            if(enableZoomTilt)
            {
                float currAngle = transform.eulerAngles.x;
                float angle = currAngle;

                //Zooming out
                if(dir < 0f)
                {
                    float dist = 1f - oldLevel;
                    float ratio = levelDiff / dist;
                    if (currAngle <= Mathf.Lerp(minZoomAngle, maxZoomAngle, oldLevel))
                        angle = Mathf.Lerp(currAngle, maxZoomAngle, ratio);
                }

                //Zooming in
                if(dir > 0f)
                {
                    float dist = oldLevel;
                    float ratio = levelDiff / dist;
                    if (currAngle >= Mathf.Lerp(minZoomAngle, maxZoomAngle, oldLevel))
                        angle = Mathf.Lerp(currAngle, minZoomAngle, ratio);
                }

                transform.rotation = Quaternion.Euler(angle, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }

        /// <summary>
        /// Start moving the camera towards the given target over time.
        /// </summary>
        /// <param name="target"></param>
        public void MoveTowards(Vector2 target)
        {
            moveTarget = target;
            isMovingTowards = true;
        }

        #endregion

        private void Update()
        {
            if(isMovingTowards)
            {
                Move((moveTarget - Position).normalized * Time.deltaTime * moveSpeed * 1.5f);
                if (Vector2.Distance(Position, moveTarget) < 0.1f)
                    isMovingTowards = false;
            }
        }
    }
}
