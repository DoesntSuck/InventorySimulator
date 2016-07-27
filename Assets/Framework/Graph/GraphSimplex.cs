using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InventorySimulator
{
    /// <summary>
    /// A simplex is a triangle with n-dimensions.  3-simplex = tetrahedron, 2-simplex = triangle, 1-simplex = line, 0-simplex = point
    /// </summary>
    public class GraphSimplex
    {
        private GraphNode[] nodes;
        private int dimensions;

        public Sphere Circumsphere
        {
            get
            {
                // Lazy initialization of circumcircle
                if (circumsphere == null)
                    circumsphere = Sphere.Circumsphere(nodes);

                return circumsphere;
            }
        }
        private Sphere circumsphere;

        public GraphSimplex(params GraphNode[] nodes)
        {
            this.nodes = nodes;
            dimensions = nodes.Length;
        }

        public bool InsideCircumsphere(Vector3 point)
        {
            return circumsphere.Contains(point);
        }

        public bool Contains(GraphNode node)
        {
            return nodes.Contains(node);
        }
    }
}
