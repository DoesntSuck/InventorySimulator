using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySimulator
{
    public class GraphNode
    {
        public Vector3 Vector { get; set; }
        public List<GraphEdge> Edges { get; private set; }

        public GraphNode(Vector3 data)
        {
            Vector = data;
            Edges = new List<GraphEdge>();
        }

        /// <summary>
        /// Adds the given edge to this nodes list of edges. NB: does not check if the given edge contains a reference to this node
        /// </summary>
        public void AddEdge(GraphEdge edge)
        {
            Edges.Add(edge);
        }

        /// <summary>
        /// Removes the given edge from this nodes list of edges
        /// </summary>
        public void RemoveEdge(GraphEdge edge)
        {
            Edges.Remove(edge);
        }

        /// <summary>
        /// Finds the edge connected to 'node' and deletes this nodes reference to the edge
        /// </summary>
        public void RemoveEdge(GraphNode node)
        {
            // Search through edge list for edge connected to 'node'
            for (int i = 0; i < Edges.Count; i++)
            {
                // If edge if connected to 'node'
                if (Edges[i].GetOther(this) == node)
                    RemoveEdge(Edges[i]);           // Remove edge from this nodes edge list
            }
        }

        /// <summary>
        /// Gets the edge connecting this node to the given node
        /// </summary>
        public GraphEdge GetEdge(GraphNode node)
        {
            // Search though edge list for edge connected to 'node'
            foreach (GraphEdge edge in Edges)
            {
                // Return edge if found
                if (edge.Contains(node))
                    return edge;
            }

            // Return null if edge not found
            return null;
        }

        /// <summary>
        /// Checks if this node is connect by an edge to the given node
        /// </summary>
        public bool ConnectsTo(GraphNode node)
        {
            if (node != this)
            {
                foreach (GraphEdge edge in Edges)
                {
                    if (edge.Contains(node))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Iterates through all nodes that this node is connected to by an edge
        /// </summary>
        public IEnumerable ConnectedNodes()
        {
            foreach (GraphEdge edge in Edges)
                yield return edge.GetOther(this);
        }

        //// Get all the triangles that use this node
        //public List<GraphTriangle> GetTriangles()
        //{
        //    List<GraphTriangle> triangles = new List<GraphTriangle>();

        //    // Search connected nodes (second point of triangle)
        //    foreach (GraphNode b in ConnectedNodes)
        //    {
        //        // Search nodes connected to the connected node (third point of triangle)
        //        foreach (GraphNode c in b.ConnectedNodes)
        //        {
        //            // A triangle is only created if the third connected node reconnects to THIS node
        //            if (c.ConnectedNodes.Contains(this))
        //                triangles.Add(new GraphTriangle(this, b, c));
        //        }
        //    }

        //    return triangles;
        //}
    }
}
