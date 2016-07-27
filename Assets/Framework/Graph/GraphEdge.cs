using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Graph
{
    public class GraphEdge
    {
        public GraphNode A { get; private set; }
        public GraphNode B { get; private set; }

        public GraphEdge(GraphNode a, GraphNode b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Checks if this edge contains a reference to the given node
        /// </summary>
        public bool Contains(GraphNode node)
        {
            // True if A OR B are the given node
            if (node == A || node == B)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the other node is this edge if this edge contains the given node
        /// </summary>
        public GraphNode GetOther(GraphNode node)
        {
            if (node == A) return B;
            else if (node == B) return A;
            else throw new ArgumentException("Edge doesn't contain the given node");
        }

        /// <summary>
        /// Checks if this edge and the given edge contain the same nodes
        /// </summary>
        public bool Equals(GraphEdge edge)
        {
            // True if the edges contain the same nodes
            if (edge.Contains(A) && edge.Contains(B))
                return true;

            return false;
        }
    }
}
