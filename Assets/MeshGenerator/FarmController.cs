using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class FarmController : MonoBehaviour {
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
        List<Vector3> verts = new List<Vector3>();     // 오브젝트의 형태를 만들게 될 정점(이하 버택스)들입니다.
        List<Vector2> uvs = new List<Vector2>();      // 오브젝트의 uv입니다. uv에 대한 설명은 추후 하겠습니다.
        List<int> tris = new List<int>();                  // 버택스 3개가 모이면 삼각형 1개가 됩니다.(너무나도 당연한..)
                                                           // 폴리곤 오브젝트들은 3각형들의 집합인데.... 문제는 그 점들의 연결 순서입니다. 자세한 설명은 까먹지 않으면 추후 하도록 하겠습니다.(참고로 DirectX의 기초공부를 하셨으면 이해 하실 것입니다.)
        Vector3 start = new Vector3((width * distance) / 2, 0, (height * distance) / 2);
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                CreateFace(new Vector3(i * distance, 0, j * distance)-start, Vector3.forward*distance, Vector3.right*distance,true, verts, uvs, tris);
        
        generatedMesh.vertices = verts.ToArray();         // 전장에서 개발자가 만든 ChunkMesh라는 Mesh의 버택스 정보를 우리가 만든 정보로 대입합니다.
        generatedMesh.uv = uvs.ToArray();                 // uv역시 마찬가지고요
        generatedMesh.triangles = tris.ToArray();          // 삼각형 정보 역시 마찬가지입니다.
        generatedMesh.RecalculateBounds();             // 정리된 버택스들을 확립(?)합니다.
                                                       // 해당 함수에 대해 쉬운 설명이 가능하신분은 덧글로 부탁드립니다.
        generatedMesh.RecalculateNormals();             // 면의 노말(방향)을 확립(?)합니다.


        meshFilter.mesh = generatedMesh;
        meshCollider.sharedMesh = generatedMesh;

    }
    public void CreateFace(Vector3 corner,Vector3 up,Vector3 right,bool reversed,List<Vector3>verts,List<Vector2> uvs,List<int> tris)
    {
        int index = verts.Count;              // verts.Count이기 때문에 인자로 들어온 정점의 카운트 번호가 인댁스 번호가 되겠죠?

        verts.Add(corner);
        verts.Add(corner + up);
        verts.Add(corner + up + right);
        verts.Add(corner + right);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        if (reversed)
        {
            // 1st Triangle
            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);

            // 2rd Triangle
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 0);
        }
        else
        {
            // 1st Triangle
            tris.Add(index + 1);
            tris.Add(index + 0);
            tris.Add(index + 2);

            // 2rd Triangle
            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 0);
        }
    }
}
