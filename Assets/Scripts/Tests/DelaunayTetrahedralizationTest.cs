using UnityEngine;
using System.Collections;
using Framework;
using Framework.TetraGraphs;


public class DelaunayTetrahedralizationTest : MonoBehaviour
{

    // Will be given a complex collider (complex collider has Contains() method)
    // Randomly generate points checking if they are contained within the complex collider
    // Do until have n points -- TODO: do i care if delaunay points are inside collider?

    public int nNodes = 10;

    private DelaunayTetrahedralization tetrahedralization;

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
        tetrahedralization = new DelaunayTetrahedralization();
        tetrahedralization.Insert(insertionVectors);
    }

    void OnDrawGizmos()
    {
        if (tetrahedralization != null)
        {
            foreach (GraphTetrahedron tetra in tetrahedralization.Graph.Tetrahedrons)
            {
                for (int i = 0; i < tetra.Nodes.Length - 1; i++)
                {
                    for (int j = i + 1; j < tetra.Nodes.Length; j++)
                    {
                        Gizmos.DrawLine(tetra.Nodes[i].Vector, tetra.Nodes[j].Vector);
                    }
                }
            }
        }
    }

    //private bool CheckDelaunay()
    //{
    //    foreach (GraphTriangle triangle in triangulation.Graph.Triangles)
    //    {
    //        foreach (GraphNode node in triangulation.Graph.Nodes)
    //        {
    //            if (!triangle.Contains(node))
    //            {
    //                if (triangle.InsideCircumsphere(node.Vector))
    //                    return false;
    //            }
    //        }
    //    }

    //    return true;
    //}
}
