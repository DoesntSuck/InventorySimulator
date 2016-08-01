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
    private TetraGraph dualGraph;

    public bool dual = false;

    void Start()
    {
        // New array of vectors
        Vector3[] insertionVectors = new Vector3[nNodes];

        // Generate random position and create empty object at position
        for (int i = 0; i < insertionVectors.Length; i++)
            insertionVectors[i] = Random.insideUnitSphere;

        // Give vectors to triangulation
        tetrahedralization = new DelaunayTetrahedralization();
        tetrahedralization.Insert(insertionVectors);

        dualGraph = tetrahedralization.Graph.DualGraph();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            dual = !dual;
    }

    void OnDrawGizmos()
    {
        if (dual)
        {
            if (dualGraph != null)
                DrawTetraWithGizmos(dualGraph);
        }

        else
        {
            if (tetrahedralization != null)
                DrawTetraWithGizmos(tetrahedralization.Graph);
        }
    }

    private void DrawTetraWithGizmos(TetraGraph graph)
    {
        foreach (GraphTetrahedron tetra in graph.Tetrahedrons)
        {
            for (int i = 0; i < tetra.Nodes.Length - 1; i++)
            {
                for (int j = i + 1; j < tetra.Nodes.Length; j++)
                {
                    Gizmos.DrawLine(tetra.Nodes[i].Vector, tetra.Nodes[j].Vector);
                }
            }
        }

        foreach (GraphNode node in graph.Nodes)
        {
            Gizmos.DrawSphere(node.Vector, 0.01f);
        }
    }
}
