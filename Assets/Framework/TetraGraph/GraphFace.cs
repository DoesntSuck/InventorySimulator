using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;

namespace Framework.TetraGraph
{
    public class GraphFace
    {
        /// <summary>
        /// The three nodes that this face is connected to
        /// </summary>
        public GraphNode[] Nodes { get; private set; }

        public GraphFace(GraphNode node1, GraphNode node2, GraphNode node3)
        {
            Nodes = new GraphNode[] { node1, node2, node3 };
        }

        /// <summary>
        /// Checks if this face is connected to the given node
        /// </summary>
        public bool Contains(GraphNode node)
        {
            return Nodes.Contains(node);
        }

        /// <summary>
        /// Checks if this face is connected to the two given nodes
        /// </summary>
        public bool Contains(GraphNode node1, GraphNode node2)
        {
            return Nodes.Contains(node1) && Nodes.Contains(node2);
        }

        /// <summary>
        /// Gets the third node in this faces set of nodes that is unique from the two given nodes
        /// </summary>
        public GraphNode GetOther(GraphNode node1, GraphNode node2)
        {
            foreach (GraphNode node in Nodes)
            {
                // If not node 1 or 2 it must be the third unique node
                if (node != node1 && 
                    node != node2)
                    return node;
            }

            // If node wasn't found, something has gone wrong
            throw new ArgumentException("Third unique node could not be found.");
        }

        /// <summary>
        /// Finds the two other nodes that this face is connected to
        /// </summary>
        public GraphNode[] GetOthers(GraphNode node)
        {
            GraphNode[] others = new GraphNode[2];
            int i = 0;
            foreach (GraphNode other in Nodes)
            {
                if (other != node)
                    others[i++] = other;                    // Method will throw index out of range exception if node is not contained in Nodes
            }

            return others;
        }

        /// <summary>
        /// Checks if this face and the given face contain the same nodes
        /// </summary>
        public bool Equals(GraphFace other)
        {
            foreach (GraphNode node in Nodes)
            {
                // Atleast one node is different
                if (!other.Contains(node))
                    return false;
            }

            // All nodes are the same
            return true;
        }
    }
}
