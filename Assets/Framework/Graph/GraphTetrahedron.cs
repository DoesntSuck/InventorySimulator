using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InventorySimulator
{
    public class GraphTetrahedron
    {
        public GraphNode[] Nodes { get { return nodes; } }
        private GraphNode[] nodes;

        public GraphEdge[] Edges { get { return edges; } }
        private GraphEdge[] edges;

        public GraphTriangle[] Triangles { get { return triangles;  } }
        private GraphTriangle[] triangles;

        public Sphere Circumsphere
        {
            get
            {
                // Lazy initialization of circumsphere
                if (circumsphere == null)
                    circumsphere = Sphere.Circumsphere(nodes[0].Vector, nodes[1].Vector, nodes[2].Vector, nodes[3].Vector);

                return circumsphere;
            }
        }
        private Sphere circumsphere;

        public GraphTetrahedron(GraphTriangle a, GraphTriangle b, GraphTriangle c, GraphTriangle d)
        {
            triangles = new GraphTriangle[] { a, b, c, d };
            edges = new GraphEdge[6];
            nodes = new GraphNode[4];

            // Add each unique edge and node to collections
            int edgeIndex = 0;
            int nodeIndex = 0;
            foreach (GraphTriangle triangle in triangles)
            {
                foreach (GraphEdge edge in triangle.GetEdges())
                {
                    // If not already in collection, add to collection
                    if (!edges.Contains(edge))
                        edges[edgeIndex++] = edge;      // Index is incremented AFTER edge is added to collection
                }

                foreach (GraphNode node in triangle.GetNodes())
                {
                    // If not already in collection, add to collection
                    if (!nodes.Contains(node))
                        nodes[nodeIndex++] = node;
                }
            }
        }

        public bool InsideCircumsphere(Vector3 point)
        {
            return circumsphere.Contains(point);
        }

        public bool Contains(GraphNode node)
        {
            return nodes.Contains(node);
        }

        public bool Contains(GraphEdge edge)
        {
            return Contains(edge.A) && Contains(edge.B);
        }

        public bool Contains(GraphTriangle triangle)
        {
            return triangles.Contains(triangle);
        }

        public bool SharesEdge(GraphTetrahedron other)
        {
            int sharedNodes = 0;
            foreach (GraphNode node in nodes)
            {
                if (other.Contains(node))
                    sharedNodes++;
            }

            return sharedNodes == 2;
        }

        public bool Equals(GraphTetrahedron other)  
        {
            // Check each node for inclusion in other tetrahedron's nodes
            foreach (GraphNode node in nodes)
            {
                // If other doesn't contain any one of these nodes then the tetrahedrons are not equal
                if (!other.Contains(node))
                    return false;
            }

            // Tetrahedrons are equal
            return true;
        }
    }
}
