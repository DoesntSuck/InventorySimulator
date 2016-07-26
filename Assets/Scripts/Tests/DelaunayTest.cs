using UnityEngine;
using System.Collections.Generic;
using UnityExtension;

namespace InventorySimulator
{
    [RequireComponent(typeof(MeshFilter))]
    public class DelaunayTest : MonoBehaviour
    {
        // Will be given a complex collider (complex collider has Contains() method)
        // Randomly generate points checking if they are contained within the complex collider
        // Do until have n points -- TODO: do i care if delaunay points are inside collider?

        private MeshFilter meshFilter;
        private TriGraph graph;

        private GraphTriangle failTri;
        private Vector3 penetratingPoint;

        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();

            Vector3[] insertionVectors = new Vector3[transform.childCount];
            for (int i = 0; i < insertionVectors.Length; i++)
                insertionVectors[i] = transform.GetChild(i).position;

            Triangulate(meshFilter.mesh, insertionVectors);
        }

        void OnDrawGizmos()
        {
            if (graph != null)
            {
                foreach (GraphEdge edge in graph.Edges)
                    Gizmos.DrawLine(edge.A.Vector, edge.B.Vector);
            }

            if (failTri != null)
            {
                Gizmos.color = Color.red;
                foreach (GraphEdge edge in failTri.GetEdges())
                    Gizmos.DrawLine(edge.A.Vector, edge.B.Vector);

                Gizmos.color = Color.green;

                Sphere circumsphere = Sphere.Circumsphere(failTri.a.Vector, failTri.b.Vector, failTri.c.Vector);
                Gizmos.DrawWireSphere(circumsphere.Centre, circumsphere.Radius);

                Gizmos.DrawSphere(penetratingPoint, 0.05f);

                Gizmos.color = Color.white;
            }

            //foreach (GraphTriangle triangle in graph.Triangles)
            //{
            //    Gizmos.DrawSphere(triangle.Circumsphere.Centre, 0.05f);
            //}
        }

        public void Triangulate(Mesh mesh, Vector3[] insertionVectors)
        {
            graph = CreateTriGraph(mesh);

            //graph.PrintTriangles();
            print("Delaunay Check: [Mesh] " + CheckDelaunay());

            for (int i = 0; i < insertionVectors.Length; i++)
            {
                Insert(insertionVectors[i]);
            }

            print("Delaunay Check: [Mesh + insertion] " + CheckDelaunay());
        }

        public void Triangulate(Vector3[] vectors)
        {
            graph = new TriGraph();

            // Add first 3 vectors, no checks required
            GraphNode a = graph.AddNode(vectors[0]);
            GraphNode b = graph.AddNode(vectors[1]);
            GraphNode c = graph.AddNode(vectors[2]);

            // Add first edges, third edge should cause a triangle to be created
            graph.AddEdge(a, b);
            graph.AddEdge(a, c);
            graph.AddEdge(b, c);

            // Iterate through all given vectors, starting at the fourth (first three are connected to make first triangle - no checking required)
            for (int i = 3; i < vectors.Length; i++)
            {
                // List for storing triangles that violate Delaunay property
                List<GraphTriangle> guiltyTriangles = new List<GraphTriangle>();

                // Find triangles that the point violates the Delaunay-ness of
                int triangleCount = graph.Triangles.Count;  // Remember size of list NOW so that added triangles are not iterated over.
                for (int j = 0; j < triangleCount; j++)
                {
                    // Check if point is inside the circumsphere of the current triangle
                    if (graph.Triangles[j].InsideCircumsphere(vectors[i]))
                        guiltyTriangles.Add(graph.Triangles[j]);    // If so, Delaunay-ness has been violated and the triangle is marked as guilty
                }

                // If there are guilty triangles
                if (guiltyTriangles.Count > 0)
                {
                    List<GraphNode> affectedNodes = RemoveTriangles(graph, guiltyTriangles);    // Remove triangles
                    TriangulateHole(graph, vectors[i], affectedNodes);                          // Triangulate the hole
                }

                else // No guilty triangles
                {
                    // Get two closest nodes to the current vector
                    List<GraphNode> closest = graph.Closest(vectors[i], 2);

                    // Add vector as node and connect an edge from the two closest nodes to this node
                    GraphNode node = graph.AddNode(vectors[i]);
                    foreach (GraphNode closeNode in closest)
                        graph.AddEdge(node, closeNode);
                }
            }
        }

        private bool CheckDelaunay()
        {
            foreach (GraphTriangle triangle in graph.Triangles)
            {
                foreach (GraphNode node in graph.Nodes)
                {
                    if (!triangle.Contains(node))
                    {
                        if (triangle.InsideCircumsphere(node.Vector))
                        {
                            failTri = triangle;
                            penetratingPoint = node.Vector;

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void Insert(Vector3 vector)
        {
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

        /// <summary>
        /// Create a tri-Graph from a mesh
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        private TriGraph CreateTriGraph(Mesh mesh)
        {
            TriGraph graph = new TriGraph();

            foreach (Vector3 vert in mesh.vertices)
                graph.AddNode(vert);

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