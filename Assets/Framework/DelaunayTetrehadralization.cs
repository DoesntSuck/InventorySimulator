using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.TetraGraphs;

namespace Framework
{
    public class DelaunayTetrahedralization
    {
        /// <summary>
        /// The extents of the triangle that contains all inerstion vectors
        /// </summary>
        public static Vector3 SuperTetraExtents = new Vector3(10, 10, 10);

        /// <summary>
        /// The tetraGraph storing all nodes, faces, and tetrahedrons in this Delaunay-Tetrahedralization
        /// </summary>
        public TetraGraph Graph { get; private set; }

        private GraphNode[] superTetra;

        public DelaunayTetrahedralization()
        {
            Graph = new TetraGraph();

            // Add super nodes
            GraphNode top = Graph.AddNode(new Vector3(0, SuperTetraExtents.y, 0));                                                  // Top
            GraphNode leftBottom = Graph.AddNode(new Vector3(-SuperTetraExtents.x, -SuperTetraExtents.y, -SuperTetraExtents.z));    // LeftBottom
            GraphNode rightBottom = Graph.AddNode(new Vector3(SuperTetraExtents.x, -SuperTetraExtents.y, -SuperTetraExtents.z));    // RightBottom
            GraphNode backBottom = Graph.AddNode(new Vector3(0, -SuperTetraExtents.y, SuperTetraExtents.z));                        // BackBottom

            // Add super faces
            Graph.AddFaces(top, leftBottom, rightBottom, backBottom);

            // Remember which nodes form the superTetra so they can be removed at the end
            superTetra = new GraphNode[] { top, leftBottom, rightBottom, backBottom };
        }

        /// <summary>
        /// Inserts the given vectors into this Delaunay-Tetrahedralization according to the Bowyer-Watson incremental insertion algorithm
        /// </summary>
        public void Insert(Vector3[] insertionVectors)
        {
            foreach (Vector3 insertionVector in insertionVectors)
            {
                //
                // Create list of tetrahedrons that have had their Delaunay-ness violated by new vector
                //

                List<GraphTetrahedron> guiltyTetras = new List<GraphTetrahedron>();
                foreach (GraphTetrahedron tetra in Graph.Tetrahedrons)
                {
                    if (tetra.InsideCircumsphere(insertionVector))
                        guiltyTetras.Add(tetra);
                }

                //
                // Find inside / ouside faces of guilty tetrahedrons
                //

                List<GraphFace> insideFaces;
                List<GraphFace> outsideFaces;
                TetraGraph.InsideOutsideFaces(out insideFaces, out outsideFaces, guiltyTetras);

                //
                // Remove guilty tetrahedrons from graph
                //
                foreach (GraphTetrahedron tetra in guiltyTetras)
                    Graph.RemoveTetraherdron(tetra);

                //
                // Remove inside faces leaving a hole in the triangulation
                //

                foreach (GraphFace insideFace in insideFaces)
                    Graph.RemoveFace(insideFace);

                //
                // Triangulate the hole by connecting outside faces to new node
                //

                GraphNode newNode = Graph.AddNode(insertionVector);
                foreach (GraphFace outsideFace in outsideFaces)
                    Graph.AddFaces(outsideFace, newNode);
            }

            //
            // Remove super tetra
            //

            foreach (GraphNode node in superTetra)
                Graph.RemoveNode(node);
        }

        public TetraGraph DualGraph()
        {
            // each tetrahedron gets converted into a vector -> vector is located at tetra circumsphere centre

            // For each tetra circumcenter, check for tetras that share a face, add edge between tetra circumcenters

            // need to add faces not edges: foreach tetra find a tetra that shares an edge, then another that shares an edge with both, connect their circumcenters


            // New graph
            TetraGraph dualGraph = new TetraGraph();

            // Create dict of tetra - nodes
            Dictionary<GraphTetrahedron, GraphNode> dualGraphNodes = new Dictionary<GraphTetrahedron, GraphNode>();
            foreach (GraphTetrahedron tetra in Graph.Tetrahedrons)
            {
                // Node position is circumsphere centre
                Vector3 nodeVector = tetra.Circumsphere.Centre;

                // New node
                GraphNode node = dualGraph.AddNode(nodeVector);
                
                // Add to dict
                dualGraphNodes.Add(tetra, node);
            }

            // Find tetras that form a dual graph triangle
            foreach (GraphTetrahedron tetra1 in Graph.Tetrahedrons)
            {
                foreach (GraphTetrahedron tetra2 in Graph.Tetrahedrons)
                {
                    if (tetra1.SharesFace(tetra2))
                    {
                        foreach (GraphTetrahedron tetra3 in Graph.Tetrahedrons)
                        {
                            if (tetra3.SharesFace(tetra2) &&
                                tetra3.SharesFace(tetra1))
                            {
                                // Get nodes from dict (tetra is key); insert face between the three
                                dualGraph.AddFace(dualGraphNodes[tetra1], dualGraphNodes[tetra2], dualGraphNodes[tetra3]);
                            }
                        }
                    }
                }
            }

            return dualGraph;
        }
    }
}
