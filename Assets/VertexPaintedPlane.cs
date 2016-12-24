using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// VertexColor Change
public struct VertexColorInfo
{
    public Color baseColor;
    public float coloredAmount;
}
public class VertexPaintedPlane : MonoBehaviour
{    
    public Mesh mesh;
    public VertexColorInfo[] vertexColorInfo;
    void Start () {
        Init();
	}

    void Init()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        vertexColorInfo = new VertexColorInfo[mesh.vertices.Length];
        Color[] colors = new Color[vertexColorInfo.Length];
        for (int i = 0; i < vertexColorInfo.Length; i++)
        {
            vertexColorInfo[i].baseColor = new Color(1.0f, 1.0f, 1.0f);
            colors[i] = vertexColorInfo[i].baseColor;
        }
        mesh.colors = colors;
    }
}
