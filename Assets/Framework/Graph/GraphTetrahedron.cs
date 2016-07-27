using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InventorySimulator
{
    public class GraphTetrahedron
    {
        public GraphNode a { get { return nodes[0]; } }
        public GraphNode b { get { return nodes[1]; } }
        public GraphNode c { get { return nodes[2]; } }
        public GraphNode d { get { return nodes[3]; } }

        private GraphNode[] nodes;
        private GraphTriangle[] triangles;

        public Sphere Circumsphere
        {
            get
            {
                // Lazy initialization of circumsphere
                if (circumsphere == null)
                    circumsphere = Sphere.Circumsphere(a.Vector, b.Vector, c.Vector, d.Vector);

                return circumsphere;
            }
        }
        private Sphere circumsphere;

        public GraphTetrahedron(GraphNode a, GraphNode b, GraphNode c, GraphNode d)
        {
            nodes = new GraphNode[] { a, b, c, d };
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
            return Contains(triangle.a) &&
                   Contains(triangle.b) &&
                   Contains(triangle.c);
        }

        public IEnumerable<GraphNode> GetNodes()
        {
            return nodes.AsEnumerable();
        }

        public IEnumerable<GraphEdge> GetEdges()
        {
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                for (int j = i + 1; j < nodes.Length; j++)
                {
                    yield return nodes[i].GetEdge(nodes[j]);
                }
            }
        }

        public IEnumerable<GraphTriangle> GetTriangles(TriGraph graph)
        {
            for (int i = 0; i < nodes.Length - 2; i++)
            {
                for (int j = i + 1; j < nodes.Length - 1; j++)
                {
                    for (int k = j + 1; k < nodes.Length; k++)
                    {
                        yield return new GraphTriangle(nodes[i], nodes[j], nodes[k]);
                    }
                }
            }
        }

        public bool Equals(GraphTetrahedron other)  
        {
            // Check each node for inclusion in other tetrahedron's nodes
            foreach (GraphNode node in GetNodes())
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
