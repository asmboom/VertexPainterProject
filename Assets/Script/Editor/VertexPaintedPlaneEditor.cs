using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(VertexPaintedPlane))]
public class VertexPaintedPlaneEditor : Editor
{
    public override void OnInspectorGUI()   //OnInspectorGUI 에 오버라이드 해 줍니다.
    {
        base.OnInspectorGUI();
        VertexPaintedPlane controller = target as VertexPaintedPlane;
        if (GUILayout.Button("Generate VertexInfo"))
        {
            if (controller)
            {
                controller.Init();
                Terrain t;
            }
        }
    }
}