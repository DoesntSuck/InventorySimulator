using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Framework.TetraGraphs;

namespace Framework
{
    public class DelaunayTetrahedralization
    {
        /// <summary>
        /// The extents of the triangle that contains all inerstion vectors
        /// </summary>
        public static Vector3 SuperTetraExtents = new Vector3(100, 100, 100);

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
                // Find inside faces of guilty tetrahedrons
                //

                List<GraphFace> insideFaces = new List<GraphFace>();
                for (int i = 0; i < guiltyTetras.Count - 1; i++)
                {
                    // Iterate through each face of each guilty tetra
                    foreach (GraphFace face in guiltyTetras[i].Faces)
                    {
                        for (int j = 0; j < guiltyTetras.Count; j++)
                        {
                            // Compare guilty tetra face to other guilty tetra face
                            if (guiltyTetras[j].Contains(face))
                            {
                                // If they share the face, the face is an inside face
                                insideFaces.Add(face);
                                break;                          // If face is common to two tetras it is on the inside: no need to keep checking
                            }
                        }
                    }
                }

                //
                // Remove inside faces leaving a hole in the triangulation
                //

                foreach (GraphFace insideFace in insideFaces)
                    Graph.RemoveFace(insideFace);

                //
                // Add given vector to graph
                //

                GraphNode newNode = Graph.AddNode(insertionVector);

                //
                // Triangulate the hole by connecting guilty tetra outside faces to new node
                //

                foreach (GraphTetrahedron guiltyTetra in guiltyTetras)
                {
                    // Outside faces are those belonging to guilty tetras but not to the insideFaces list
                    foreach (GraphFace outsideFace in guiltyTetra.Faces.Except(insideFaces))
                    {
                        Graph.AddFaces(outsideFace, newNode);
                    }   
                }

                //
                // Remove guilty tetrahedrons from graph
                //
                foreach (GraphTetrahedron tetra in guiltyTetras)
                    Graph.RemoveTetraherdron(tetra);
            }

            //
            // Remove super tetra
            //

            foreach (GraphNode node in superTetra)
                Graph.RemoveNode(node);
        }
    }
}
