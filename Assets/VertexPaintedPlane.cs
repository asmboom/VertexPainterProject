using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// VertexColor Change
[System.Serializable]
public struct VertexColorInfo
{
    public Color baseColor;
    public float coloredAmount;
}
[RequireComponent(typeof(MeshCollider))]
public class VertexPaintedPlane : MonoBehaviour
{
    public Mesh mesh;
    public VertexColorInfo[] vertexColorInfo;
    public List<int>[] nearVertices;
    public Dictionary<int,float> coloredVertices;
    void Awake () {
        Init();
	}

    void Init()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertexColorInfo = new VertexColorInfo[mesh.vertices.Length];
        Color[] colors = new Color[vertexColorInfo.Length];
        coloredVertices = new Dictionary<int, float>();
        nearVertices = new List<int>[mesh.vertices.Length];
        for (int i = 0; i < vertexColorInfo.Length; i++)
        {
            nearVertices[i] = new List<int>();
            vertexColorInfo[i].baseColor = new Color(1.0f, 1.0f, 1.0f);
            colors[i] = vertexColorInfo[i].baseColor;
        }
        mesh.colors = colors;
        for (int i=0; i < mesh.triangles.Length/3;i++)
        {
            int a = mesh.triangles[i * 3];
            int b = mesh.triangles[i * 3 + 1];
            int c = mesh.triangles[i * 3 + 2];
            nearVertices[a].Add(b);
            nearVertices[a].Add(c);
            nearVertices[b].Add(b);
            nearVertices[b].Add(c);
            nearVertices[c].Add(a);
            nearVertices[c].Add(b);
        }
    }
    public void SetColoredInfo(int index,float _coloredAmount,float _coloredTime)
    {
        vertexColorInfo[index].coloredAmount = _coloredAmount;
        if (coloredVertices.ContainsKey(index))
            coloredVertices[index] = _coloredTime;
        else
            coloredVertices.Add(index, _coloredTime);
    }
    void Update()
    {
        if(coloredVertices.Count>0)
        {
            bool changed = false;
            Color[] colors = mesh.colors;
            foreach(var key in coloredVertices.Keys.ToList())
            {
                coloredVertices[key] = coloredVertices[key] - Time.deltaTime;
                if (coloredVertices[key] <= 0)
                {
                    Color lerpedColor = Color.LerpUnclamped(colors[key], vertexColorInfo[key].baseColor, Time.deltaTime*5);

                    float distance = Vector3.Distance(new Vector3(lerpedColor.r, lerpedColor.g, lerpedColor.b), new Vector3(vertexColorInfo[key].baseColor.r, vertexColorInfo[key].baseColor.g, vertexColorInfo[key].baseColor.b));
                    if(distance<0.1f)
                    //if (lerpedColor == vertexColorInfo[key].baseColor)
                    {
                        colors[key] = vertexColorInfo[key].baseColor;
                        vertexColorInfo[key].coloredAmount = 0.0f;
                        coloredVertices.Remove(key);
                    }
                    else colors[key] = lerpedColor;
                    changed = true;
                }
            }
            if (changed)
                mesh.colors = colors;
        }
    }
}
