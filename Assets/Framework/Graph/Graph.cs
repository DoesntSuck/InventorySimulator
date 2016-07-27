using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graph
{
    public class Graph
    {
        /// <summary>
        /// Collection of nodes in this graph
        /// </summary>
        public List<GraphNode> Nodes { get; protected set; }

        /// <summary>
        /// Collection of edges in this graph
        /// </summary>
        public HashSet<GraphEdge> Edges { get; protected set; } // Collection is a set to quicken 'Remove' operations

        public Graph()
        {
            Nodes = new List<GraphNode>();
            Edges = new HashSet<GraphEdge>();
        }
        
        /// <summary>
        /// Gets the closest nodes to the given vector. Nodes are sort baed on distance to the given vector
        /// </summary>
        public List<GraphNode> Closest(Vector3 point, int count)
        {
            // Sort Nodes list based on each nodes' vector's distance to the given point
            Nodes.Sort((node1, node2) => Vector3.Distance(point, node1.Vector).CompareTo(Vector3.Distance(point, node2.Vector)));

            // Return subset of elements starting at first element and containing 'count' number of elements
            return Nodes.GetRange(0, count);
        }

        /// <summary>
        /// Creates and adds a new node containing the given Vector3 data. Returns the newly created node
        /// </summary>
        public virtual GraphNode AddNode(Vector3 data)
        {
            GraphNode newNode = new GraphNode(data);
            Nodes.Add(newNode);
            return newNode;
        }

        /// <summary>
        /// Removes the given node from this graph. Any edges connected to the node are also removed.
        /// </summary>
        public virtual void RemoveNode(GraphNode toBeDeleted)
        {
            // Remove all edges connecting to the 'toBeDeleted' node
            while (toBeDeleted.Edges.Count > 0)
                RemoveEdge(toBeDeleted.Edges[0]);

            // Remove node
            Nodes.Remove(toBeDeleted);
        }

        /// <summary>
        /// Creates and adds a new edge between the given nodes. Returns the newly created edge.
        /// </summary>
        public virtual GraphEdge AddEdge(GraphNode x,GraphNode y)
        {
            // Create and add edge to set of edges
            GraphEdge edge = new GraphEdge(x, y);
            Edges.Add(edge);

            x.AddEdge(edge);
            y.AddEdge(edge);

            return edge;
        }

        /// <summary>
        /// Finds and removes the edge from node x to node y.
        /// </summary>
        public virtual void RemoveEdge(GraphNode x, GraphNode y)
        {
            // Get edge between x and y, remove from list of edges
            GraphEdge edge = x.GetEdge(y);
            Edges.Remove(edge);

            // Both nodes in edge remove the edge from their list of edges
            x.RemoveEdge(y);
            y.RemoveEdge(x);
        }

        /// <summary>
        /// Removes the given edge from this graph.
        /// </summary>
        public virtual void RemoveEdge(GraphEdge edge)
        {
            RemoveEdge(edge.A, edge.B);
        }
    }
}
