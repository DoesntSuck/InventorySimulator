using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

namespace InventorySimulator
{
    // TODO: Don't connect original mesh vectors to delaunay triangulation: these vectors are part of the voronoi dual graph NOT the delaunay triangulation

    // TODO: Maybe edges from outside voronoi points to inside ones are needed to find the point on the side of the mesh that will be used

    public class DelaunayTriangulation
    {
        private TriGraph graph;

        public DelaunayTriangulation()
        {
            graph = new TriGraph();
        }

        // TODO: USE BOWER-WATSON ALGORITHM INSTEAD (wikipedia ya'll)
        public void Insert(Vector3 vector)
        {
            // Special case: first node to be inserted - no checks required - no edges to be made
            if (graph.Nodes.Count == 0)
            {
                graph.AddNode(vector);
            }

            // Special case: second or third node to be inserted - no checked required
            else if (graph.Nodes.Count == 1 || graph.Nodes.Count == 2)
            {
                // Add node
                GraphNode newNode = graph.AddNode(vector);

                // Add edges between all nodes
                foreach (GraphNode node in graph.Nodes)
                    graph.AddEdge(newNode, node);
            }

            else
            {
                // List for storing triangles that violate Delaunay property
                List<GraphTriangle> guiltyTriangles = new List<GraphTriangle>();

                // Find triangles that the point violates the Delaunay-ness of
                for (int j = 0; j < graph.Triangles.Count; j++)
                {
                    // Check if point is inside the circumsphere of the current triangle
                    if (graph.Triangles[j].InsideCircumsphere(vector))
                        guiltyTriangles.Add(graph.Triangles[j]);    // If so, Delaunay-ness has been violated and the triangle is marked as guilty
                }

                // If there are guilty triangles
                if (guiltyTriangles.Count > 0)
                {
                    // Generate list of inside edges of the polygonal hole left by removing the guilty triangles
                    List<GraphEdge> insideEdges = new List<GraphEdge>();
                    for (int i = 0; i < guiltyTriangles.Count - 1; i++)
                    {
                        // Check each edge of each guilty triangle
                        foreach (GraphEdge edge in guiltyTriangles[i].GetEdges())
                        {
                            // Iterate through remaining guiltyTriangles checking if any contain the current edge
                            for (int j = i + 1; j < guiltyTriangles.Count; j++)
                            {
                                // If another guilty triangle contains the same edge, the edge is an inner edge
                                if (guiltyTriangles[j].Contains(edge))
                                {
                                    insideEdges.Add(edge);
                                    break;                                      // Break - no need to keep checking other triangles
                                }
                            }
                        }
                    }

                    // Remove all inside edges from graph
                    foreach (GraphEdge insideEdge in insideEdges)
                        graph.RemoveEdge(insideEdge);

                    // Add vector to graph
                    GraphNode node = graph.AddNode(vector);

                    // Triangulate the hole
                    foreach (GraphTriangle guiltyTriangle in guiltyTriangles)
                    {
                        foreach (GraphNode guiltyNode in guiltyTriangle.GetNodes())
                            graph.AddEdge(node, guiltyNode);
                    }

                    //List<GraphNode> affectedNodes = RemoveTriangles(graph, guiltyTriangles);    // Remove triangles
                    //TriangulateHole(graph, vector, affectedNodes);                              // Triangulate the hole
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
        }

        public void Insert(Vector3[] insertionVectors)
        {
            for (int i = 0; i < insertionVectors.Length; i++)
            {
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
