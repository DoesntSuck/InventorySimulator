using UnityEngine;
using System.Collections;

public class CentreOfTriangleTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Transform[] childTrans = GetComponentsInChildren<Transform>();
        Vector3 centre = (childTrans[1].position + childTrans[2].position + childTrans[3].position) / 3;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = centre;
        sphere.transform.localScale *= 0.25f;

    }

    public void OnDrawGizmos()
    {
        Transform[] childTrans = GetComponentsInChildren<Transform>();
        for (int i = 1; i < childTrans.Length - 1; i++)
        {
            for (int j = i + 1; j < childTrans.Length; j++)
            {
                Gizmos.DrawLine(childTrans[i].position, childTrans[j].position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
