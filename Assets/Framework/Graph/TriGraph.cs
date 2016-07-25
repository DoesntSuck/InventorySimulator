using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace InventorySimulator
{
    public class TriGraph : Graph
    {
        public List<GraphTriangle> Triangles { get; private set; }

        private Dictionary<GraphEdge, List<GraphTriangle>> edgeTriangles;

        public TriGraph()
        {
            edgeTriangles = new Dictionary<GraphEdge, List<GraphTriangle>>();
            Triangles = new List<GraphTriangle>();
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
        /// Removes the edge from all collections
        /// </summary>
        public override void RemoveEdge(GraphEdge edge)
        {
            base.RemoveEdge(edge);

            edgeTriangles.Remove(edge);
        }

        /// <summary>
        /// Does nothing... yet.
        /// </summary>
        /// <param name="node"></param>
        public override void RemoveNode(GraphNode node)
        {
            base.RemoveNode(node);

            // Feels bad man
            throw new NotImplementedException("Please don't remove nodes");
        }

        private void AddTriangle(GraphNode x, GraphNode y, GraphNode z)
        {
            GraphTriangle triangle = new GraphTriangle(x, y, z);

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
            if (edgeTriangles[toRemove.AB].Count == 0) RemoveEdge(toRemove.AB);
            if (edgeTriangles[toRemove.AC].Count == 0) RemoveEdge(toRemove.AC);
            if (edgeTriangles[toRemove.BC].Count == 0) RemoveEdge(toRemove.BC);
        }

        public bool ContainsTriangle(GraphNode a, GraphNode b, GraphNode c)
        {
            foreach (GraphTriangle triangle in Triangles)
            {
                if (triangle.Contains(a) && triangle.Contains(b) && triangle.Contains(c))
                    return true;
            }

            return false;
        }

        public void PrintTriangles()
        {
            foreach (GraphTriangle triangle in Triangles)
                Debug.Log(triangle.ToString());
        }
    }
}
