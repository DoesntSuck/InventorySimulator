﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySimulator
{
    /// <summary>
    /// A triangle composed of three vertices and edges in 3-dimensional space
    /// </summary>
    public class GraphTriangle
    {
        public GraphNode a { get; private set; }
        public GraphNode b { get; private set; }
        public GraphNode c { get; private set; }

        public GraphEdge AB { get; private set; }
        public GraphEdge AC { get; private set; }
        public GraphEdge BC { get; private set; }

        private Sphere circumsphere;

        public GraphTriangle(GraphNode a, GraphNode b, GraphNode c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            AB = a.GetEdge(b);
            AC = a.GetEdge(c);
            BC = b.GetEdge(c);

            // Check that nodes form a triangle
            if (!(a.ConnectsTo(b) && 
                  b.ConnectsTo(c) && 
                  c.ConnectsTo(a)))
                throw new ArgumentException("Nodes do not form a triangle");
        }

        /// <summary>
        /// Checks if the given point is inside the circumsphere of this triangle. The circumsphere is a sphere that touches all three of the
        /// triangle's verties.
        /// </summary>
        /// <param name="point">The point to test for inclusion within this triangle's circumsphere</param>
        /// <returns>True if the point is inside this triangles circumsphere</returns>
        public bool InsideCircumsphere(Vector3 point)
        {
            // Lazy initialization of circumsphere
            if (circumsphere == null)
                circumsphere = Sphere.Circumsphere(a.Vector, b.Vector, c.Vector);

            // Check for inclusion
            return circumsphere.Contains(point);
        }

        /// <summary>
        /// Checks if this triangle contains the given node
        /// </summary>
        public bool Contains(GraphNode node)
        {
            return node == a || 
                   node == b || 
                   node == c;
        }

        /// <summary>
        /// Checks if this triangle contains the given edge
        /// </summary>
        public bool Contains(GraphEdge edge)
        {
            return edge == AB ||
                   edge == AC ||
                   edge == BC;
        }
        
        /// <summary>
        /// Iterate through the nodes that make up this triangle
        /// </summary>
        public IEnumerable<GraphNode> GetNodes()
        {
            yield return a;
            yield return b;
            yield return c;
        }

        /// <summary>
        /// Iterate through the edges that make up this triangle
        /// </summary>
        public IEnumerable GetEdges()
        {
            yield return AB;
            yield return AC;
            yield return BC;

        }

        /// <summary>
        /// Checks if the given triangle contains the same nodes as this triangle
        /// </summary>
        public bool Equals(GraphTriangle other)
        {
            // Check each node for inclusion in other triangles nodes
            foreach (GraphNode node in GetNodes())
            {
                // If other doesn't contain any one of these nodes then the triangles are not equal
                if (!other.Contains(node))
                    return false;
            }

            // Triangles are equal
            return true;
        }

        public override string ToString()
        {
            return "a: " + a.Vector.ToString() + ", b: " + b.Vector.ToString() + ", c: " + c.Vector.ToString();
        }
    }
}