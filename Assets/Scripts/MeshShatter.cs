using UnityEngine;
using System.Collections;
using UnityExtension;
using Framework.TetraGraphs;

namespace Framework
{
    [RequireComponent(typeof(Collider))]
    public class MeshShatter : MonoBehaviour
    {

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

        // TODO: Properties
        // need a property that determines how many voronoi nuclei to generate
        // need a prop that determines the area that is affected by impact


        [Tooltip("The amount of stress this object can withstand without plastically deforming.")]
        public float YieldStrength;

        [Tooltip("The amount of stress this object can withstand before breaking. TensileStrength is always greater than or equal to the yield strength")]
        public float TensileStrength;

        [Tooltip("The amount of stress this object can withstand over and over again a million times without breaking")]
        public float FatigueStrength;

        [Range(-1, 1)]
        [Tooltip("This objects propensity for shattering or deforming under stress. A positive number indicates the object is more likely to bend than " +
            "break under pressure. A negative number indicates the object is more likely to break. The closer this number is towards its limits (-1, 1), " +
            "the larger the area that will be affected by the bend / break.")]
        public float Plasticity;

        [Tooltip("This objects ability to return from a deformed state to a normal state after stress has been removed")]
        public float Elasticity;

        public float Brittleness;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnMouseDown()
        {
            // TODO: Get impact point from collider collision

            Collider collider = GetComponent<Collider>();
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hitInfo;

            if (collider.Raycast(camRay, out hitInfo, 100))
            {
                // TODO: replace brittleness for equation that encorporates the force of impact (mass of other object etc) AND brittleness

                // Generate point cloud
                Vector3[] cloud = new Vector3[10];
                for (int i = 0; i < cloud.Length; i++)
                    cloud[i] = Math3D.RandomVectorFromTriangularDistribution(hitInfo.point, Brittleness);

                DelaunayTetrahedralization tetrahedralization = new DelaunayTetrahedralization();
                tetrahedralization.Insert(cloud);
                TetraGraph graph = tetrahedralization.Graph.DualGraph();

                // 10 points generated means 10 chunks to instantiate

                // TODO: figure out how to connect dual graph vectors to current mesh

            
            }
        }

        private void Combine(TetraGraph voronoiGraph)
        {
            // Find all verts outside mesh bounds using collider.contains

            // For each vert outside
                // Follow each edge to the point where it intersects with the edge of the mesh
                // A vert is added at this location

            // Add points of original mesh with Delaunay Triangulation?
            // Get rid of oold triangle data
            // Normals for each vertex are generated drawing the line that passess through the voronoi cell nuclei and the vertex in question


            foreach (GraphNode node in voronoiGraph.Nodes)
            {
            }
        }
    }
}