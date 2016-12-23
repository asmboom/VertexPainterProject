using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// VertexColor Change
public class VertexPaintedPlane : MonoBehaviour {
    
    public MeshFilter meshFilter;

	void Start () {
        meshFilter = GetComponent<MeshFilter>();
        Init();
	}

    void Init()
    {
        Mesh mesh = meshFilter.mesh;
        Color[] colors = new Color[mesh.vertices.Length];
        Debug.Log(mesh.vertices.Length);
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color(1.0f, 1.0f, 1.0f);
        mesh.colors = colors;
        Debug.Log(mesh.colors.Length);

    }
}
