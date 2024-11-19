using UnityEngine;

namespace SF
{
    public class MouseWorldPosition : MonoBehaviour
    {
        public static MouseWorldPosition Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Returns the point from a raycast from the camera to see what world position we are holding the mouse on.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // We use the plane method so we don't have to use the physics version. 
            // Below is the physics equivalent.
            // if(Physics.Raycast(mouseCameraRay, out RaycastHit it))

            Plane plane = new Plane(Vector3.up,Vector3.zero);
            if(plane.Raycast(mouseCameraRay, out float distance))
            {
                return mouseCameraRay.GetPoint(distance);
            }
            else
                return Vector3.zero;
        }
    }
}
