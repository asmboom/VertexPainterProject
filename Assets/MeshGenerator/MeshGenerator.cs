using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour {
    public int width;
    public int height;
    public float distance;
    GameObject genratedMeshObject;
    Mesh generatedMesh;
    public MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    public MeshCollider meshCollider;
    public void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
    public void GenerateMesh()
    {
        generatedMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        int indexCount = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            { 
                verts.Add(new Vector3(x * distance, 0, y * distance));
                Vector2 uvMap = new Vector2((float)x / (width - 1), (float)y / (height - 1));
                uvs.Add(uvMap);
                indexCount++;
            }
        }
        for (int i=0;i<width*height-width;i++)
        {
            if ((i + 1) % width == 0)
                continue;
            tris.Add(i);
            tris.Add(i + 1 + width);
            tris.Add(i+1);

            tris.Add(i);
            tris.Add(i + width);
            tris.Add(i + 1 + width);
        }
        // A X B 행렬에서 나올 수 있는 폴리곤 수 = (height-1)*(2*width-2))
        generatedMesh.vertices = verts.ToArray();
        generatedMesh.uv = uvs.ToArray();
        generatedMesh.triangles = tris.ToArray();
        generatedMesh.RecalculateBounds();
        generatedMesh.RecalculateNormals();


        meshFilter.mesh = generatedMesh;
        meshCollider.sharedMesh = generatedMesh;

    }
}
