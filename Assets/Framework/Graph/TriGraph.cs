using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graph
{
    public class TriGraph : Graph
    {
        /// <summary>
        /// Collection of triangles formed by the addition of edges to this graph
        /// </summary>
        public List<GraphTriangle> Triangles { get; private set; }

        private Dictionary<GraphEdge, List<GraphTriangle>> edgeTriangles;   // Triangles indexed by edge

        public TriGraph()
        {
            edgeTriangles = new Dictionary<GraphEdge, List<GraphTriangle>>();
            Triangles = new List<GraphTriangle>();
        }

        public override void RemoveNode(GraphNode toBeDeleted)
        {
            base.RemoveNode(toBeDeleted);

            // Check for triangles that contain the given node
            for (int i = 0; i < Triangles.Count; i++)
            {
                // If a triangle contains the given node, remove the triangle
                if (Triangles[i].Contains(toBeDeleted))
                    Triangles.RemoveAt(i);
            }

            // Also remove triangle from edge-tri dict
            foreach (List<GraphTriangle> edgeTris in edgeTriangles.Values)
            {
                for (int i = 0; i < edgeTris.Count; i++)
                {
                    // If a triangle contains the given node, remove the triangle
                    if (edgeTris[i].Contains(toBeDeleted))
                        edgeTris.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Creates and adds a new edge between the given nodes; also stores any new triangles created with the addition of the new edge. Returns the
        /// newly created edge
        /// </summary>
        public override GraphEdge AddEdge(GraphNode x, GraphNode y)
        {
            GraphEdge edge = base.AddEdge(x, y);

            // Add empty triangle list to edge-tri dictionary
            edgeTriangles.Add(edge, new List<GraphTriangle>());

            // Find triangles created by adding new edge
            foreach (GraphNode node in y.ConnectedNodes())
            {
                // Check if triangle is created by adding new edge, but only add it if an equal triangle is not already present
                if (node.ConnectsTo(x) && !ContainsTriangle(x, y, node))
                    AddTriangle(x, y, node);
            }

            return edge;
        }

        /// <summary>
        /// Removes the edge from this graph. Any triangles associated with the edge will also be removed
        /// </summary>
        public override void RemoveEdge(GraphEdge edge)
        {
            base.RemoveEdge(edge);

            // Remove all triangles associated with the given edge
            foreach (GraphTriangle triangle in edgeTriangles[edge])
                Triangles.Remove(triangle);

            // Remove edge key from edge-tri dict
            edgeTriangles.Remove(edge);
        }

        /// <summary>
        /// Creates and stores a triangle that is associated with the given three nodes
        /// </summary>
        private void AddTriangle(GraphNode x, GraphNode y, GraphNode z)
        {
            GraphTriangle triangle = new GraphTriangle(x, y, z);

            // Add to triangle list and edge-indexed triangle dict
            Triangles.Add(triangle);
            edgeTriangles[triangle.AB].Add(triangle);
            edgeTriangles[triangle.AC].Add(triangle);
            edgeTriangles[triangle.BC].Add(triangle);
        }


        /// <summary>
        /// Remove triangle from all collections. Where an edge in 'toRemove' is no longer used by any triangle after 'toRemove's removal, that edge
        /// is deleted.
        /// </summary>
        public void RemoveTriangle(GraphTriangle toRemove)
        {
            // Remove triangle from triangle list
            Triangles.Remove(toRemove);

            // Remove triangle from edge-tri dictionary
            edgeTriangles[toRemove.AB].Remove(toRemove);
            edgeTriangles[toRemove.AC].Remove(toRemove);
            edgeTriangles[toRemove.BC].Remove(toRemove);

            // Check if edges of 'toRemove' are in use by any other triangle; if not, remove the edge
            if (edgeTriangles[toRemove.AB].Count == 0) edgeTriangles.Remove(toRemove.AB);
            if (edgeTriangles[toRemove.AC].Count == 0) edgeTriangles.Remove(toRemove.AC);
            if (edgeTriangles[toRemove.BC].Count == 0) edgeTriangles.Remove(toRemove.BC);
        }

        //public List<GraphTriangle> GetTriangle(GraphNode a, GraphNode b, GraphNode c)
        //{
        //    //return edgeTriangles[edge];
        //}

        /// <summary>
        /// Checks if this tri-graph contains a triangle connecting the given nodes
        /// </summary>
        public bool ContainsTriangle(GraphNode a, GraphNode b, GraphNode c)
        {
            // Iterate through all triangles
            foreach (GraphTriangle triangle in Triangles)
            {
                // If triangle contains all nodes...
                if (triangle.Contains(a) && triangle.Contains(b) && triangle.Contains(c))
                    return true;
            }

            // No triangle contained all nodes
            return false;
        }

        public void PrintTriangles()
        {
            foreach (GraphTriangle triangle in Triangles)
                Debug.Log(triangle.ToString());
        }
    }
}
