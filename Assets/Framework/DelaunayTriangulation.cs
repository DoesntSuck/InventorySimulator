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
        public static Vector3 superTriangleExtents = new Vector3(100, 100, 100);

        public TriGraph Graph { get { return graph; } }
        private TriGraph graph;

        private GraphNode[] superTriangleNodes;

        public DelaunayTriangulation()
        {
            graph = new TriGraph();
        }

        /// <summary>
        /// Incremental insertion of vectors to a Delaunay triangulation according to the Bowyer-Watson algorithm.
        /// </summary>
        public void Insert(Vector3[] insertionVectors)
        {
            //
            // Add super triangle large enough to encorporate every vector in input set
            //

            superTriangleNodes = new GraphNode[4];

            // Create super triangle -> 4 trianlges that make a triangle-based pyramid
            // Super triangle is large enough that it encorporates all points in input set
            superTriangleNodes[0] = graph.AddNode(new Vector3(0, superTriangleExtents.y, 0));                                               // Top
            superTriangleNodes[1] = graph.AddNode(new Vector3(-superTriangleExtents.x, -superTriangleExtents.y, -superTriangleExtents.z));  // BottomLeft
            superTriangleNodes[2] = graph.AddNode(new Vector3(superTriangleExtents.x, -superTriangleExtents.y, -superTriangleExtents.z));   // BottomRight
            superTriangleNodes[3] = graph.AddNode(new Vector3(0, -superTriangleExtents.y, superTriangleExtents.z));                         // BottomBack

            // Add edges between nodes
            for (int i = 0; i < superTriangleNodes.Length - 1; i++)
            {
                for (int j = i + 1; j < superTriangleNodes.Length; j++)
                {
                    graph.AddEdge(superTriangleNodes[i], superTriangleNodes[j]);
                }
            }

            //
            // Add vectors to triangulation
            //

            foreach (Vector3 insertionVector in insertionVectors)
                Insert(insertionVector);
            //
            // Remove super triangle
            //

            foreach (GraphNode superTriangleNode in superTriangleNodes)
                graph.RemoveNode(superTriangleNode);
        }

        /// <summary>
        /// Bowyer-Watson incremental insertion algorithm. 
        /// </summary>
        private void Insert(Vector3 vector)
        {
            //
            // Find triangles that the new vector violates the Delaunay property of.
            //

            // List for storing triangles that violate Delaunay property
            List<GraphTriangle> guiltyTriangles = new List<GraphTriangle>();

            // Find triangles that the point violates the Delaunay-ness of
            foreach (GraphTriangle triangle in graph.Triangles)
            {
                // Check if point is inside the circumsphere of the current triangle
                if (triangle.InsideCircumsphere(vector))
                    guiltyTriangles.Add(triangle);          // If so, Delaunay-ness has been violated and the triangle is marked as guilty
            }

            //
            // Find inside edges of guilty triangles - will be removed to create a hole
            //

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

            //
            // Remove inside edges from guilty triangles, leaving a hole in the triangulation
            //

            // Remove all inside edges from graph
            foreach (GraphEdge insideEdge in insideEdges)
                graph.RemoveEdge(insideEdge);

            //
            // Add given vector to graph
            //

            GraphNode node = graph.AddNode(vector);

            //
            // Triangulate the hole: created triangles are guaranteed to be Delaunay
            //

            // Add edge between every node in a guilty triangle to the newly inserted node
            foreach (GraphTriangle guiltyTriangle in guiltyTriangles)
            {
                foreach (GraphNode guiltyNode in guiltyTriangle.GetNodes())
                    graph.AddEdge(node, guiltyNode);
            }
        }
    }
}
