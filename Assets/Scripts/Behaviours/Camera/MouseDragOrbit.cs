using UnityEngine;
using System;

namespace CameraControls
{
    /// <summary>
    /// basic behaviour to allow user input to manipulate the camera.
    /// On wheel down the position of the event is saved, as the mouse is dragged the camera is orbited by 
    /// </summary>
    public class MouseDragOrbit : MonoBehaviour
    {
        #region Fields Exposed To Unity Editor
        /// <summary>
        /// Target gameobject
        /// The camera will force itself to look at this object when <see cref="Update"/> is called.
        /// </summary>
        public GameObject Target;
        /// <summary>
        /// The speed coefficient.  Just a scalar, does not have units.
        /// The different 
        /// </summary>
        public float OrbitSpeed = 0.02f;
        #endregion

        #region Internal State Variables
        private Vector3? mouseClickedPosition = null;
        private Quaternion rotation;
        #endregion

        #region Unity Lifecycle Methods
        void Start()
        {
            rotation = transform.rotation;
            Reposition(0);
        }

        void Update()
        {
            if (DragDistanceX() is float distance)
            {
                Reposition(distance * OrbitSpeed);
            }
        }
        #endregion

        #region Logic Functions
        /// <summary>
        /// Stores the mouse wheel click location and computes the drag distance.
        /// </summary>
        /// <returns>The distance dragged along the x axis in screen pixels, or null if the mouse is not being dragged</returns>
        protected float? DragDistanceX()
        {
            if (Input.GetMouseButtonDown(2))
            {
                mouseClickedPosition = Input.mousePosition;
                rotation = transform.rotation;
                //originalCameraPosition.transform.position = transform.position; // move this into the update, because it would interfere with other camera behaviours.
                Debug.Log("Mouse down at " + mouseClickedPosition);
            }

            if (Input.GetMouseButtonUp(2))
            {
                mouseClickedPosition = null;
                Debug.Log("Mouse up at " + mouseClickedPosition);
            }

            if (mouseClickedPosition != null)
            {
                var theta = (Input.mousePosition.x - mouseClickedPosition.Value.x);
                Debug.Log("Mouse drag theta " + theta);
                return theta;
            }
            return null;
        }
        /// <summary>
        /// orbit the camera around the <see cref="ta"/>
        /// </summary>
        /// <param name="theta"></param>
        protected void Reposition(float theta)
        {
            transform.rotation = rotation;
            transform.LookAt(Target.transform.position);
            transform.RotateAround(Target.transform.position, Vector3.up, theta);
        }
        #endregion
    }
}
