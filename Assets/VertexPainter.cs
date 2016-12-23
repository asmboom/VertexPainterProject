using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPainter : MonoBehaviour {

    SphereCollider col;
    void Start()
    {
        col = GetComponent<SphereCollider>();
    }
    void SetVertexColor(int triangle, Transform _target,Vector3 point)
    {

        Mesh mesh = _target.GetComponent<MeshFilter>().mesh;
        Color[] colors = mesh.colors;
        int closestVertex = -1;
        float dist = 1000f;
        for(int i=0;i<3;i++)
        {
            float dis = Vector3.Distance(point, _target.TransformPoint(mesh.vertices[mesh.triangles[triangle * 3 + i]]));
            if (dis < dist)
            {
                dist = dis;
                closestVertex = i;
            }
        }
        if (closestVertex >= 0)
        {
            colors[mesh.triangles[triangle * 3+closestVertex]] = new Color(0.0f, 0.0f, 0.0f);
            mesh.colors = colors;
        }
    }
    void OnCollisionStay(Collision col)
    {
        int triangle;

        if (col.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            foreach (ContactPoint P in col.contacts)
            {
                RaycastHit hit;
                Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
                if (Physics.Raycast(ray, out hit, 0.1f, LayerMask.GetMask("ground")))
                {
                    triangle = hit.triangleIndex;
                    SetVertexColor(triangle, hit.transform,P.point);
                    break;
                }
            }
        }
    }
}
