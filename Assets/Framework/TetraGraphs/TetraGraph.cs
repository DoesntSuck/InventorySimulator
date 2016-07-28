using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityExtension;

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
            for (int i = 0; i < 2; i++)
            {
                GraphTetrahedron tetra = CreateTetrahedron(newFace);
                if (tetra != null)
                    Tetrahedrons.Add(tetra);
            }

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
                    // Only make a new face if there isn't already one connecting these nodes
                    if (!ContainsFace(face.Nodes[i], face.Nodes[j], node))
                    {
                        GraphFace newFace = AddFace(face.Nodes[i], face.Nodes[j], node);
                        faces[index++] = newFace;   // Index is incremented AFTER adding face to array
                    }
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

        public bool ContainsTetra(ICollection<GraphNode> nodes)
        {
            foreach (GraphTetrahedron tetra in Tetrahedrons)
            {
                if (tetra.Contains(nodes))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Finds and removes the given tetrahedron from this graph
        /// </summary>
        public void RemoveTetraherdron(GraphTetrahedron tetrahedron)
        {
            Tetrahedrons.Remove(tetrahedron);
        }

        // TODO: Inserting a face can create many tetrahdra
        
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

                    // Check that there is not already a tetra made from the given nodes
                    if (!ContainsTetra(tetraNodes))
                    {
                        // Create list to store faces of tetra, list starts with the current face
                        HashSet<GraphFace> tetraFacesSet = new HashSet<GraphFace>() { face };
                        foreach (GraphFace otherFace in Faces)
                        {
                            // Check that otherFace contains no unique nodes
                            if (tetraNodes.ContainsAll(otherFace.Nodes))
                            {
                                tetraFacesSet.Add(otherFace);

                                // Found enough connecting faces to make a tetrahedron
                                if (tetraFacesSet.Count == 3)
                                {
                                    // Convert to array so can access specific elements
                                    GraphFace[] tetraFaces = tetraFacesSet.ToArray();

                                    GraphTetrahedron tetra = new GraphTetrahedron(newFace, tetraFaces[0], tetraFaces[1], tetraFaces[2]);

                                    // TODO: dont stop here!
                                    return tetra;
                                }
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
