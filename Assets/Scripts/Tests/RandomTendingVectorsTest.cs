using UnityEngine;
using UnityExtension;

namespace InventorySimulator
{
    /// <summary>
    /// Script tests the ability to generate random vectors from a triangular distribution. When the attached collider is clicked 'VectorCount' number
    /// of gizmo spheres should appear in the scene. The distribution of these spheres should tend towards the clicked point, becoming sparser the
    /// further away from the click up to 'MaxDistance' away from the click.
    /// 
    /// Note: gizmos must be enabled in order to see the effect of this script.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RandomTendingVectorsTest : MonoBehaviour
    {
        [Tooltip("The number of vectors to generate")]
        public int VectorCount = 100;

        [Tooltip("The maximum distance of vectors from the clicked point")]
        public float MaxDistance = 2.0f;

        [Tooltip("The size of the gizmo spheres that represent each vector")]
        public float GizmoSphereRadius = 0.05f;

        private Vector3[] points;
        private new Collider collider;

        void Awake()
        {
            collider = GetComponent<Collider>();
        }

        // Draw the points as gizmos to the screen
        public void OnDrawGizmos()
        {
            if (points != null)
            {
                foreach (Vector3 point in points)
                    Gizmos.DrawSphere(point, GizmoSphereRadius);
            }
        }

        // When mouse is clicked on the attached collider, Get the clicked point and generate points around it
        public void OnMouseDown()
        {
            // Raycast to attached collider
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (collider.Raycast(ray, out hit, VectorCount))
            {
                // Generate a bunch of points
                points = new Vector3[VectorCount];
                for (int i = 0; i < points.Length; i++)
                    points[i] = Math3D.RandomVectorFromTriangularDistribution(hit.point, MaxDistance);
            }
        }
    }
}