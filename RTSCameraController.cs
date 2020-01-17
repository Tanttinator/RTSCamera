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
        new public Camera camera;

        /// <summary>
        /// Move the camera instantly.
        /// </summary>
        /// <param name="dir">The point relative to the cameras position.</param>
        public void Move(Vector2 dir)
        {
            transform.Translate(new Vector3(dir.x, 0f, dir.y), Space.World);
        }

        /// <summary>
        /// Pan the camera around the pivot
        /// </summary>
        /// <param name="angle"></param>
        public void PanPivot(float angle)
        {
            transform.Rotate(0f, angle, 0f, Space.World);
        }

        /// <summary>
        /// Tilt the camera around the pivot
        /// </summary>
        /// <param name="angle"></param>
        public void TiltPivot(float angle)
        {
            transform.RotateAround(transform.position, transform.right, angle);
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
    }
}
