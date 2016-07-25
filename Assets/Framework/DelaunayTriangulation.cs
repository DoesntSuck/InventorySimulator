using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityExtension;

namespace InventorySimulator
{
    public class DelaunayTriangulation
    {
        private TriGraph graph;
        private Bounds bounds;

        public DelaunayTriangulation(Mesh mesh, Bounds bounds)
        {
            this.bounds = bounds;
            graph = GraphFromMesh(mesh);
        }

        private TriGraph GraphFromMesh(Mesh mesh)
        {
            // New Graph
            TriGraph graph = new TriGraph();

            // Add all mesh verts to graph
            foreach (Vector3 vert in mesh.vertices)
                graph.AddNode(vert);

            // Add sides of triangles as edges to graph
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                GraphNode a = graph.Nodes[mesh.triangles[i]];
                GraphNode b = graph.Nodes[mesh.triangles[i + 1]];
                GraphNode c = graph.Nodes[mesh.triangles[i + 2]];

                graph.AddEdge(a, b);
                graph.AddEdge(a, c);
                graph.AddEdge(b, c);
            }

            return graph;
        }

        public void Insert(Vector3 vector)
        {
            // TODO: Check if vector is contained in bounds

            // List for storing triangles that violate Delaunay property
            List<GraphTriangle> guiltyTriangles = new List<GraphTriangle>();

            // Find triangles that the point violates the Delaunay-ness of
            int triangleCount = graph.Triangles.Count;  // Remember size of list NOW so that added triangles are not iterated over.
            for (int j = 0; j < triangleCount; j++)
            {
                // Check if point is inside the circumsphere of the current triangle
                if (graph.Triangles[j].InsideCircumsphere(vector))
                    guiltyTriangles.Add(graph.Triangles[j]);    // If so, Delaunay-ness has been violated and the triangle is marked as guilty
            }

            // If there are guilty triangles
            if (guiltyTriangles.Count > 0)
            {
                List<GraphNode> affectedNodes = RemoveTriangles(graph, guiltyTriangles);    // Remove triangles
                TriangulateHole(graph, vector, affectedNodes);                          // Triangulate the hole
            }

            else // No guilty triangles
            {
                // Get two closest nodes to the current vector
                List<GraphNode> closest = graph.Closest(vector, 2);

                // Add vector as node and connect an edge from the two closest nodes to this node
                GraphNode node = graph.AddNode(vector);
                foreach (GraphNode closeNode in closest)
                    graph.AddEdge(node, closeNode);
            }
        }

        public void Insert(Vector3[] insertionVectors)
        {
            for (int i = 0; i < insertionVectors.Length; i++)
            {
                if (bounds.Contains(insertionVectors[i]))
                    Insert(insertionVectors[i]);
            }
        }

        private List<GraphNode> RemoveTriangles(TriGraph graph, List<GraphTriangle> triangles)
        {
            List<GraphNode> affectedNodes = new List<GraphNode>();
            while (triangles.Count > 0)
            {
                // Get and remove first triangle from list
                GraphTriangle triangle = triangles.PopAt(0);

                // Remember its nodes, remove the triangle from the graph
                affectedNodes.AddRange(triangle.GetNodes());
                graph.RemoveTriangle(triangle);
            }

            return affectedNodes;
        }

        private void TriangulateHole(TriGraph graph, Vector3 point, List<GraphNode> affectedNodes)
        {
            // Convert point to a node
            GraphNode node = graph.AddNode(point);

            // Connect each affected node to the new node
            foreach (GraphNode affectedNode in affectedNodes)
                graph.AddEdge(node, affectedNode);
        }
    }
}
