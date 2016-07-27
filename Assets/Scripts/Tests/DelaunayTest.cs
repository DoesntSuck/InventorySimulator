using UnityEngine;
using System.Collections.Generic;
using UnityExtension;

namespace InventorySimulator
{
    public class DelaunayTest : MonoBehaviour
    {
        // Will be given a complex collider (complex collider has Contains() method)
        // Randomly generate points checking if they are contained within the complex collider
        // Do until have n points -- TODO: do i care if delaunay points are inside collider?

        public int nNodes = 10;

        private DelaunayTriangulation triangulation;

        void Start()
        {
            // New array of vectors
            Vector3[] insertionVectors = new Vector3[nNodes];

            // Generate random position and create empty object at position
            for (int i = 0; i < insertionVectors.Length; i++)
            {
                // Random position
                insertionVectors[i] = Random.insideUnitSphere;

                // New gameobj at position
                GameObject nodeObj = new GameObject("Node");
                nodeObj.transform.SetParent(transform);
                nodeObj.transform.position = insertionVectors[i];
            }

            // Give vectors to triangulation
            triangulation = new DelaunayTriangulation();
            triangulation.Insert(insertionVectors);
        }

        void OnDrawGizmos()
        {
            if (triangulation != null)
            {
                foreach (GraphEdge edge in triangulation.Graph.Edges)
                    Gizmos.DrawLine(edge.A.Vector, edge.B.Vector);
            }

            //foreach (GraphTriangle triangle in graph.Triangles)
            //{
            //    Gizmos.DrawSphere(triangle.Circumsphere.Centre, 0.05f);
            //}
        }

        private bool CheckDelaunay()
        {
            foreach (GraphTriangle triangle in triangulation.Graph.Triangles)
            {
                foreach (GraphNode node in triangulation.Graph.Nodes)
                {
                    if (!triangle.Contains(node))
                    {
                        if (triangle.InsideCircumsphere(node.Vector))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}