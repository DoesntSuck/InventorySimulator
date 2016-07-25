using System;
using UnityEngine;
using UnityExtension;

namespace InventorySimulator
{
    /// <summary>
    /// A sphere composed of a central vertex in 3-dimensional space and a radius
    /// </summary>
    public class Sphere
    {
        public Vector3 Centre { get; set; }
        public float Radius { get; set; }

        public Sphere(Vector3 centre, float radius)
        {
            Centre = centre;
            Radius = radius;
        }

        /// <summary>
        /// Creates a new sphere that touches each of the three vertices in the given triangle.
        /// </summary>
        /// <param name="triangle">The triangle to create a circumsphere for</param>
        /// <returns>A new sphere that touches each of the triangles three vertices</returns>
        public static Sphere Circumsphere(Vector3 a, Vector3 b, Vector3 c)
        {
            // Calculate centre of sphere
            Vector3 circumcentre = Math3D.Circumcentre(a, b, c);

            // Calcuate radius of sphere
            float circumradius = Math3D.Circumradius(a, b, c);

            // New sphere
            return new Sphere(circumcentre, circumradius);
        }

        /// <summary>
        /// Checks if the given point is contained within this sphere
        /// </summary>
        /// <param name="point">The point to check for inclusion within this sphere</param>
        /// <returns>True if the point is inside this sphere</returns>
        public bool Contains(Vector3 point)
        {
            // True if point is less than radius distance away from the centre
            return Vector3.Distance(Centre, point) < Radius;
        }
    }
}
