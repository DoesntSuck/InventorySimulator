using UnityEngine;
using System.Collections;
using Framework;

namespace InventorySimulator
{
    /// <summary>
    /// Script tests the ability to calculate the circumsphere of a triangle in 3-dimensional space. The three input vectors are used to created a
    /// a triangle; a sphere primitive is instantiated to represent the circumsphere. Set shading mode to wireframe in the scene iew in order to
    /// properly see the triangle and sphere.
    /// </summary>
    public class CircumsphereTest : MonoBehaviour
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;

        public void Start()
        {
            // Create in-memory circumsphere
            Sphere circumsphere = Sphere.Circumsphere(A, B, C);

            // Create sphere game object to be drawn tos creen
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            // Position and size sphere primitive according to circumsphere
            sphere.transform.position = circumsphere.Centre;
            sphere.transform.localScale = Vector3.one * (circumsphere.Radius * 2);
        }

        public void OnDrawGizmos()
        {
            // Draw triangle
            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(A, C);
            Gizmos.DrawLine(B, C);
        }
    }
}