using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.TetraGraph
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

        public TetraGraph()
        {
            Nodes = new List<GraphNode>();
            Faces = new List<GraphFace>();
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
        /// <param name="node"></param>
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
        }

        /// <summary>
        /// Creates and adds a new face that connects the given nodes. The new face is returned
        /// </summary>
        public GraphFace AddFace(GraphNode node1, GraphNode node2, GraphNode node3)
        {
            // Create and add new face
            GraphFace face = new GraphFace(node1, node2, node3);
            Faces.Add(face);

            // Also add face to each node it is attached to
            node1.AddFace(face);
            node2.AddFace(face);
            node3.AddFace(face);

            return face;
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
    }
}
