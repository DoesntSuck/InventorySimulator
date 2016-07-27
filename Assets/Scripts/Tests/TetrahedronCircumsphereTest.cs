using UnityEngine;
using System.Collections.Generic;
using UnityExtension;
using Framework;

namespace InventorySimulator
{
    public class TetrahedronCircumsphereTest : MonoBehaviour
    {
        private Vector3[] vectors;

        // Use this for initialization
        void Start()
        {
            vectors = new Vector3[4];
            for (int i = 0; i < 4; i++)
                vectors[i] = transform.GetChild(i).position;

            // Calculate circumsphere in memory
            Sphere circumsphere = Sphere.Circumsphere(vectors[0], vectors[1], vectors[2], vectors[3]);

            // Instantiate sphere game object, position at circumcentre, size to circumradius
            GameObject sphereobj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereobj.transform.position = circumsphere.Centre;
            sphereobj.transform.localScale = Vector3.one * (circumsphere.Radius * 2);
        }

        void OnDrawGizmos()
        {
            // Draw lines between each position in vectors array
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length - 1; i++)
                {
                    for (int j = i + 1; j < vectors.Length; j++)
                    {
                        Gizmos.DrawLine(vectors[i], vectors[j]);
                    }
                }
            }
        }
    }
}