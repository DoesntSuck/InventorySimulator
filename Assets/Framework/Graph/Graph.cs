using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySimulator
{
    public class Graph
    {
        public List<GraphNode> Nodes { get; protected set; }
        public HashSet<GraphEdge> Edges { get; protected set; }

        // TODO: Store list of edges to allow drawing gizmos easily

        public Graph()
        {
            Nodes = new List<GraphNode>();
            Edges = new HashSet<GraphEdge>();
        }

        /// <summary>
        /// Checks if there is an edge from node x to node y
        /// </summary>
        public bool Adjacent(GraphNode x, GraphNode y)
        {
            return x.ConnectsTo(y);
        }

        /// <summary>
        /// Iterates through all nodes connected by an edge to x
        /// </summary>
        public IEnumerable Neighbours(GraphNode x)
        {
            return x.ConnectedNodes();
        }

        public List<GraphNode> Closest(Vector3 point, int count)
        {
            // Sort Nodes list based on each nodes' vector's distance to the given point
            Nodes.Sort((node1, node2) => Vector3.Distance(point, node1.Vector).CompareTo(Vector3.Distance(point, node2.Vector)));

            // Return subset of elements starting at first element and containing 'count' number of elements
            return Nodes.GetRange(0, count);

            // SOrt list of nodes based on distance to the given point
            // Return array containing the first 'count' nodes in list.
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
            foreach (GraphEdge edge in toBeDeleted.Edges)
                RemoveEdge(edge);

            // Remove node
            Nodes.Remove(toBeDeleted);
        }

        /// <summary>
        /// Creates and adds a new edge between the given nodes. Returns the newly created edge.
        /// </summary>
        public virtual GraphEdge AddEdge(GraphNode x,GraphNode y)
        {
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
            GraphEdge edge = x.GetEdge(y);
            Edges.Remove(edge);

            x.RemoveEdge(y);
            y.RemoveEdge(x);
        }

        public virtual void RemoveEdge(GraphEdge edge)
        {
            RemoveEdge(edge.A, edge.B);
        }
    }
}
