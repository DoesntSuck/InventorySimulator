using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.TetraGraphs
{
    public class GraphTetrahedron
    {
        /// <summary>
        /// The 4 nodes that this tetrahedron is connected to
        /// </summary>
        public GraphNode[] Nodes { get; private set; }

        /// <summary>
        /// The four faces that this tetrahedron is connected to
        /// </summary>
        public GraphFace[] Faces { get; private set; }

        /// <summary>
        /// The minimum bounding sphere that touches each of the nodes in this tetrahedron
        /// </summary>
        public Sphere Circumsphere
        {
            get
            {
                // Lazy initialization of circumsphere
                if (circumsphere == null)
                    circumsphere = Sphere.Circumsphere(Nodes[0].Vector, Nodes[1].Vector, Nodes[2].Vector, Nodes[3].Vector);

                return circumsphere;
            }
        }
        private Sphere circumsphere;

        public GraphTetrahedron(GraphFace face1, GraphFace face2, GraphFace face3, GraphFace face4)
        {
            // Add faces to face collection
            Faces = new GraphFace[] { face1, face2, face3, face4 };

            // Find set of unique nodes
            HashSet<GraphNode> nodeSet = new HashSet<GraphNode>();
            foreach (GraphFace face in Faces)
            {
                foreach (GraphNode node in face.Nodes)
                {
                    nodeSet.Add(node);
                }
            }

            // Store as an array
            Nodes = nodeSet.ToArray();
        }

        /// <summary>
        /// Checks if the given point is inside the circumsphere of this tetrahedron
        /// </summary>
        public bool InsideCircumsphere(Vector3 point)
        {
            return Circumsphere.Contains(point);
        }

        /// <summary>
        /// Checks if this tetrahedron contains the given node
        /// </summary>
        public bool Contains(GraphNode node)
        {
            return Nodes.Contains(node);
        }

        public bool Contains(ICollection<GraphNode> nodes)
        {
            foreach (GraphNode node in nodes)
            {
                if (!Nodes.Contains(node))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if this tetrahedron contains the given face
        /// </summary>
        public bool Contains(GraphFace face)
        {
            return Faces.Contains(face);
        }

        public bool SharesFace(GraphTetrahedron other)
        {
            foreach (GraphFace face in Faces)
            {
                if (other.Contains(face))
                    return true;
            }

            return false;
        }

        public bool Equals(GraphTetrahedron other)  
        {
            // Check each node for inclusion in other tetrahedron's nodes
            foreach (GraphNode node in Nodes)
            {
                // If other doesn't contain any one of these nodes then the tetrahedrons are not equal
                if (!other.Contains(node))
                    return false;
            }

            // Tetrahedrons are equal
            return true;
        }

        public override string ToString()
        {
            return "Tetra: " + Nodes[0].Vector.ToString() + ", "
                             + Nodes[1].Vector.ToString() + ", "
                             + Nodes[2].Vector.ToString() + ", "
                             + Nodes[3].Vector.ToString();
        }
    }
}
