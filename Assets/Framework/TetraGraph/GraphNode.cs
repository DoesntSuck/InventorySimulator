using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.TetraGraph
{
    public class GraphNode
    {
        /// <summary>
        /// The collection of faces connected to this node
        /// </summary>
        public List<GraphFace> Faces { get; private set; }

        /// <summary>
        /// The vector data that this node stores
        /// </summary>
        public Vector3 Vector { get; private set; }

        public GraphNode(Vector3 vector)
        {
            Vector = vector;
            Faces = new List<GraphFace>();
        }

        /// <summary>
        /// Adds the given face to this nodes collection of faces
        /// </summary>
        public void AddFace(GraphFace face)
        {
            Faces.Add(face);
        }

        /// <summary>
        /// Finds and removes the given face from this nodes collection of faces
        /// </summary>
        public void RemoveFace(GraphFace face)
        {
            Faces.Remove(face);
        }

        /// <summary>
        /// Finds and removes the face the connects this node to the given two nodes
        /// </summary>
        public void RemoveFace(GraphNode connectedNode1, GraphNode connectedNode2)
        {
            // Iterate through all faces connected to this node
            for (int i = 0; i < Faces.Count; i++)
            {
                // If face contains THIS node and the two given nodes...
                if (Faces[i].Contains(connectedNode1) &&
                    Faces[i].Contains(connectedNode2))
                {
                    Faces.RemoveAt(i);                      // Remove the face
                    break;
                }
            }
        }

        /// <summary>
        /// Finds and removes all faces that are connected to both this node and the given node
        /// </summary>
        public void RemoveFaces(GraphNode connectedNode)
        {
            // Iterate through all faces connected to this node
            for (int i = 0; i < Faces.Count; i++)
            {
                // If face contains THIS node and the given node...
                if (Faces[i].Contains(connectedNode))
                    Faces.RemoveAt(i--);                    // Remove the face
            }
        }

        /// <summary>
        /// Finds the face that connects this node with both of the given nodes
        /// </summary>
        public GraphFace GetFace(GraphNode connectedNode1, GraphNode connectedNode2)
        {
            foreach (GraphFace face in Faces)
            {
                // If face contains THIS node and the two given nodes
                if (face.Contains(connectedNode1) &&
                    face.Contains(connectedNode2))
                    return face;
            }

            // No face in collection contains all three nodes
            return null;
        }

        /// <summary>
        /// Iterates through the collection of faces that share this node and the given node
        /// </summary>
        public IEnumerable<GraphFace> GetFaces(GraphNode connectedNode)
        {
            foreach (GraphFace face in Faces)
            {
                // If face contains THIS node and the given node
                if (face.Contains(connectedNode))
                    yield return face;
            }
        }

        /// <summary>
        /// Checks if this node contains a face connecting it to the given node
        /// </summary>
        public bool ConnectsTo(GraphNode other)
        {
            foreach (GraphFace face in Faces)
            {
                // If face contains THIS node and the given node
                if (face.Contains(other))
                    return true;
            }

            // No face connects this node to the given node
            return false;
        }

        /// <summary>
        /// Checks if there is a face connecting this node to the two given nods
        /// </summary>
        public bool ConnectsTo(GraphNode node1, GraphNode node2)
        {
            foreach (GraphFace face in Faces)
            {
                // If face connects THIS node to the two given nodes
                if (face.Contains(node1) &&
                    face.Contains(node2))
                    return true;
            }

            // No face connects this node to the two given nodes
            return false;
        }
    }
}
