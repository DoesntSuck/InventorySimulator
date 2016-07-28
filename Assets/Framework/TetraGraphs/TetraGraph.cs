using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.TetraGraphs
{
    public class TetraGraph
    {
        /// <summary>
        /// Collection of nodes in this graph
        /// </summary>
        public List<GraphNode> Nodes { get; private set; }

        /// <summary>
        /// Collection of faces in this graph
        /// </summary>
        public List<GraphFace> Faces { get; private set; }

        /// <summary>
        /// Collection of tetrahedrons in this graph
        /// </summary>
        public List<GraphTetrahedron> Tetrahedrons { get; private set; }

        public TetraGraph()
        {
            Nodes = new List<GraphNode>();
            Faces = new List<GraphFace>();
            Tetrahedrons = new List<GraphTetrahedron>();
        }

        /// <summary>
        /// Creates and adds a new node storing the given vector. New node is returned.
        /// </summary>
        public GraphNode AddNode(Vector3 vector)
        {
            GraphNode node = new GraphNode(vector);
            Nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Finds and removes the given node; also, removes faces attached to the given node
        /// </summary>
        public void RemoveNode(GraphNode node)
        {
            // Find and remove node
            Nodes.Remove(node);

            // Find faces attached to the given node
            for (int i = 0; i < Faces.Count; i++)
            {
                // Remove attached faces
                if(Faces[i].Contains(node))
                    Faces.RemoveAt(i--);
            }

            // Find tetrahedrons attached to the given node
            for (int i = 0; i < Tetrahedrons.Count; i++)
            {
                // Remove attached tetrahedrons
                if (Tetrahedrons[i].Contains(node))
                    Tetrahedrons.RemoveAt(i--);
            }
        }

        /// <summary>
        /// Creates and adds a new face that connects the given nodes. The new face is returned
        /// </summary>
        public GraphFace AddFace(GraphNode node1, GraphNode node2, GraphNode node3)
        {
            // Create new face
            GraphFace newFace = new GraphFace(node1, node2, node3);

            // Check if the new face can form a tetrahedron with three other connected faces
            GraphTetrahedron tetra = CreateTetrahedron(newFace);
            if (tetra != null)
                Tetrahedrons.Add(tetra);

            // Also add face to each node it is attached to
            node1.AddFace(newFace);
            node2.AddFace(newFace);
            node3.AddFace(newFace);

            // Add face to collection AFTER looking for tetrahedrons (so the new face is not iterated over)
            Faces.Add(newFace);

            return newFace;
        }

        /// <summary>
        /// Connects each unique pair of nodes in the given face to the given node, creating 3 new faces. The new faces are returned
        /// </summary>
        public GraphFace[] AddFaces(GraphFace face, GraphNode node)
        {
            // Array to store new faces
            GraphFace[] faces = new GraphFace[3];
            int index = 0;

            // Each unique pair of nodes in the given face is connected to the given node
            for (int i = 0; i < face.Nodes.Length - 1; i++)
            {
                for (int j = i + 1; j < face.Nodes.Length; j++)
                {
                    // Create new face
                    GraphFace newFace = AddFace(face.Nodes[i], face.Nodes[j], node);
                    faces[index++] = newFace;   // Index is incremented AFTER adding face to array
                }
            }

            // Return newly created faces
            return faces;
        }

        /// <summary>
        /// Creates and adds new faces connected each unique triplet of nodes in the given nodes, creating 4 new faces. The new faces are returned
        /// </summary>
        public GraphFace[] AddFaces(GraphNode node1, GraphNode node2, GraphNode node3, GraphNode node4)
        {
            // New array of faces created by connected each unqiue triplet of nodes
            GraphFace[] faces = new GraphFace[]
            {
                AddFace(node1, node2, node3),
                AddFace(node1, node2, node4),
                AddFace(node1, node3, node4),
                AddFace(node2, node3, node4)
            };

            return faces;
        }

        /// <summary>
        /// Removes the given face from this graph. Tetrahedrons that are connected to the face are also removed.
        /// </summary>
        /// <param name="face"></param>
        public void RemoveFace(GraphFace face)
        {
            // Find and remove face from faces collection
            Faces.Remove(face);

            // Find and remove tetrahedrons that connect to the face
            for (int i = 0; i < Tetrahedrons.Count; i++)
            {
                if (Tetrahedrons[i].Contains(face))
                    Tetrahedrons.RemoveAt(i--);
            }
        }

        /// <summary>
        /// Checks if this graph contains a face connecting the given nodes
        /// </summary>
        public bool ContainsFace(GraphNode node1, GraphNode node2, GraphNode node3)
        {
            foreach (GraphFace face in Faces)
            {
                // If face contains all of the given nodes
                if (face.Contains(node1) && 
                    face.Contains(node2) && 
                    face.Contains(node3))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a tetrahedron has been formed with the addition of the given edge. If so, the tetrahedron is created and returned.
        /// </summary>
        private GraphTetrahedron CreateTetrahedron(GraphFace newFace)
        {
            // Find a face that shares two points with the new face
            //     Save four unique points to set
            //     Search other faces for face whose points are ALL contained in set
            //          On finding two other faces that wholly share points with initial two faces, job done!

            foreach (GraphFace face in Faces)
            {
                // If face shares an edge with the newFace, check if we can make a tetrahedron with them
                if (face.SharesEdge(newFace))
                {
                    // Get the four points the two edges use
                    HashSet<GraphNode> tetraNodes = new HashSet<GraphNode>(newFace.Nodes);
                    tetraNodes.UnionWith(face.Nodes);

                    // Create list to store faces of tetra, list starts with the current face
                    List<GraphFace> tetraFaces = new List<GraphFace>() { face };
                    foreach (GraphFace otherFace in Faces)
                    {
                        // Checks that there aren't nodes in otherFace that tetra nodes doesn't contain
                        if (!otherFace.Nodes.Except(tetraNodes).Any())
                        {
                            tetraFaces.Add(otherFace);

                            // Found enough connecting faces to make a tetrahedron
                            if (tetraFaces.Count == 3)
                            {
                                GraphTetrahedron tetra = new GraphTetrahedron(newFace, tetraFaces[0], tetraFaces[1], tetraFaces[2]);
                                return tetra;
                            }
                        }
                    }
                }
            }

            // No tetrahedrons could be created with the new face
            return null;
        }
    }
}
