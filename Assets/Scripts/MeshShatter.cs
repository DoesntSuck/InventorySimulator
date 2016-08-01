using UnityEngine;
using System.Collections;

public class MeshShatter : MonoBehaviour {

    // TODO: point of impact should be where a break forms (dont make it an insertion point, because they are the voronoi cell nuclei)

    // Get point of impact
    // Generate cloud of points
    // Give points to Delaunay function
    // Get dual graph
    // Cull points outside:
        // For every point that is not contained within the mesh:
            // For every face connected to the point
                // Find where each edge of face contacts mesh bounds
                // Create vectors at each intersection (two vectors)
                // Add two faces connecting the two new points to the two other points in original face
            // Delete node not contained in mesh

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
