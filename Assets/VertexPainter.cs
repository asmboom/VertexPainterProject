using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPainter : MonoBehaviour {

    void Start()
    {

    }
    void Update()
    {

    }
    void SetVertexColor(List<int> triangles,Mesh _targetMesh)
    {
        if (triangles == null || triangles.Count <= 0)
            return;
        Mesh mesh = _targetMesh;
        Color[] colors = mesh.colors;
        foreach (int triangle in triangles.ToArray())
        {
            colors[mesh.triangles[triangle * 3]] = new Color(0.0f, 0.0f, 0.0f);
            colors[mesh.triangles[triangle * 3 + 1]] = new Color(0.0f, 0.0f, 0.0f);
            colors[mesh.triangles[triangle * 3 + 2]] = new Color(0.0f, 0.0f, 0.0f);
        }
        mesh.colors = colors;
    }
    void OnCollisionStay(Collision col)
    {
        List<int> triangles = new List<int>();

        if (col.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            foreach (ContactPoint P in col.contacts)
            {
                RaycastHit hit;
                Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
                if (Physics.Raycast(ray, out hit, 0.1f, LayerMask.GetMask("ground")))
                {
                    Debug.Log(hit.triangleIndex);
                    triangles.Add(hit.triangleIndex);
                }
            }
            if (triangles.Count > 0)
                SetVertexColor(triangles, col.gameObject.GetComponent<MeshFilter>().mesh);
        }
    }
}
