using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VertexPainter : MonoBehaviour {

    SphereCollider col;
    [SerializeField]
    Color vertexColor;
    [SerializeField]
    float vertexHeight;
    [SerializeField]
    float radius;
    Vector3 lastPosition;
    [SerializeField]
    float vertexColoredTime = 10.0f;

    void Start()
    {
        col = GetComponent<SphereCollider>();
    }
    void SetVertexColor(Transform _target,int triangle,Vector3 worldPosition)
    {

        Mesh mesh = _target.GetComponent<MeshFilter>().mesh;
        Color[] colors = mesh.colors;
        int closestVertex = GetClosestVertex(mesh, _target.TransformPoint(worldPosition), triangle);
        if (closestVertex >= 0)
        {
            colors[closestVertex] = vertexColor;
            mesh.colors = colors;
        }
    }
    void SetVertexColorInRadius(Transform _target, int triangle, Vector3 worldPosition, float _radius)
    {
        Mesh mesh = _target.GetComponent<MeshFilter>().mesh;
        VertexPaintedPlane plane = _target.GetComponent<VertexPaintedPlane>();
        Color[] colors = mesh.colors;
        Vector3[] vertices = mesh.vertices;
        int closestVertex = GetClosestVertex(mesh, _target.TransformPoint(worldPosition), triangle);
        if (closestVertex >= 0)
        {
            int[] verticesIndexes;
            GetVertexInRadius(plane, mesh, out verticesIndexes, closestVertex, _radius);
            Vector3 bPos = mesh.vertices[closestVertex];
            foreach (int index in verticesIndexes)
            {
                float distance = Vector3.Distance(bPos, mesh.vertices[index]);
                float colorAmount = 1 - distance / _radius;

                if (colorAmount >= plane.vertexColorInfo[index].coloredAmount)
                {
                    plane.SetColoredInfo(index, colorAmount, vertexColoredTime);
                    SetVertexHeight(mesh, plane,index, vertexHeight*colorAmount, ref vertices,false);
                    //plane.vertexColorInfo[index].coloredAmount = colorAmount;
                    Color lerpedColor = Color.Lerp(plane.vertexColorInfo[index].baseColor, vertexColor, colorAmount);
                    colors[index] = lerpedColor;
                }
                //else colorAmount = plane.vertexColorInfo[index].coloredAmount;
            }
            mesh.vertices = vertices;
            mesh.colors = colors;
        }
    }
    void SetVertexHeight(Mesh _target, VertexPaintedPlane plane,int vertex, float height,ref Vector3[] vertices,bool update=true)
    {
        Mesh mesh = _target;
        vertices[vertex].y = plane.vertexColorInfo[vertex].height+height;
        if(update)
            mesh.vertices = vertices;
    }
    void SetVertexHeight(Transform _target, int triangle, Vector3 worldPosition, float height)
    {
        Mesh mesh = _target.GetComponent<MeshFilter>().mesh;
        int closestVertex = GetClosestVertex(mesh, _target.TransformPoint(worldPosition), triangle);
        if (closestVertex >= 0)
        {
            Vector3[] vertices = mesh.vertices;
            vertices[closestVertex].y += height;
            mesh.vertices = vertices;
        }
    }
    void OnCollisionStay(Collision col)
    {
        return;
        int triangle;
        if (col.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            if (Vector3.Distance(transform.position,lastPosition)<0.2f)
            {
                return;
            }
            lastPosition = transform.position;
            foreach (ContactPoint P in col.contacts)
            {
                RaycastHit hit;
                Ray ray = new Ray(P.point + P.normal * 0.05f, -P.normal);
                if (Physics.Raycast(ray, out hit, 0.1f, LayerMask.GetMask("ground")))
                {
                    triangle = hit.triangleIndex;
                    SetVertexColorInRadius(hit.transform, triangle, hit.point, radius);
                    break;
                }
            }
        }
    }
    int GetClosestVertex(Mesh targetMesh, Vector3 localPosition,int triangleIndex)
    {
        int closestVertex = -1;
        float dist = 1000f;
        for (int i = 0; i < 3; i++)
        {
            float dis = Vector3.Distance(localPosition, targetMesh.vertices[targetMesh.triangles[triangleIndex * 3 + i]]);
            if (dis < dist)
            {
                dist = dis;
                closestVertex = targetMesh.triangles[triangleIndex * 3 + i];
            }
        }
        if (closestVertex >= 0)
        {
            return closestVertex;
        }
        return -1;
    }
    void SearchNearVertex(VertexPaintedPlane plane, Mesh targetMesh, ref List<int> findedVertices, int baseVertex, int nextVertex, float _radius, ref bool[] visited)
    {
        if (visited[nextVertex])
            return;
        visited[nextVertex] = true;
        Vector3 basePosition = targetMesh.vertices[baseVertex];
        float distance = Vector3.Distance(basePosition, targetMesh.vertices[nextVertex]);
        if (distance > _radius)
        {
            return;
        }
        findedVertices.Add(nextVertex);
        foreach (int v in plane.nearVertices[nextVertex])
        {
            SearchNearVertex(plane, targetMesh, ref findedVertices, baseVertex, v, radius,ref visited);
        }
    }
    void GetVertexInRadius(VertexPaintedPlane plane,Mesh targetMesh, out int[] findedVertices,int baseVertex,float _radius)
    {
        float time = Time.deltaTime;
        List<int> vIndexes = new List<int>();
        Vector3 basePosition = targetMesh.vertices[baseVertex];
        bool[] visited = new bool[targetMesh.vertices.Length];
        SearchNearVertex(plane, targetMesh, ref vIndexes, baseVertex, baseVertex, radius,ref visited);
        findedVertices = vIndexes.ToArray();
    }
    void Update()
    {
        RaycastHit hit;
        if (true/*transform.position != lastPosition*/)
        {
            if (Physics.SphereCast(transform.position + Vector3.up * 1, col.radius, Vector3.up * -1, out hit, Mathf.Infinity, LayerMask.GetMask("ground")))
            {
                SetVertexColorInRadius(hit.transform, hit.triangleIndex, hit.point, radius);
                //SetVertexColor(hit.transform, hit.triangleIndex, hit.point);
                //SetVertexHeight(hit.transform,hit.triangleIndex, hit.point, vertexHeight);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("ground")))
            {
                SetVertexColorInRadius(hit.transform, hit.triangleIndex, hit.point,radius);
                //SetVertexHeight(hit.triangleIndex, hit.transform, hit.point,vertexHeight);
            }

        }
    }
    void LateUpdate()
    {

        lastPosition = transform.position;
    }
}
