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
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color(1.0f, 1.0f, 1.0f);
        mesh.colors = colors;
    }
	/*void Update () {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity))
            {
                int triangle = hitInfo.triangleIndex;
                print(triangle);
            }
        }
	}*/
}
