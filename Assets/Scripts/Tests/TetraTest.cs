using UnityEngine;
using System.Collections;

public class TetraTest : MonoBehaviour
{
    Transform[] transforms;

    void Awake()
    {
        transforms = GetComponentsInChildren<Transform>();
    }

    public void OnDrawGizmos()
    {
        for (int i = 1; i < transforms.Length - 1; i++)
        {
            for (int j = i + 1; j < transforms.Length; j++)
            {
                Gizmos.DrawLine(transforms[i].position, transforms[j].position);
            }
        }

        for (int i = 1; i < transforms.Length - 2; i++)
        {
            for (int j = i + 1; j < transforms.Length - 1; j++)
            {
                for (int k = j + 1; k < transforms.Length; k++)
                {
                    Vector3 facePoint;
                    Vector3 faceVector;

                    FacePlane(out facePoint, out faceVector, transforms[i].position, transforms[j].position, transforms[k].position);

                    Gizmos.DrawRay(facePoint, faceVector);
                }
            }
        }
    }

    private void FacePlane(out Vector3 facePoint, out Vector3 faceVector, Vector3 a, Vector3 b, Vector3 c)
    {
        facePoint = (a + b + c) / 3;

        Vector3 ab = b - a;
        Vector3 ac = c - a;
        faceVector = Vector3.Cross(ab, ac);
    }
}
