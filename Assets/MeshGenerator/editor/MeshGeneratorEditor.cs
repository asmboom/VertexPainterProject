using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()   //OnInspectorGUI 에 오버라이드 해 줍니다.
    {
        base.OnInspectorGUI();
        MeshGenerator controller = target as MeshGenerator;
        if (GUILayout.Button("Generate Mesh From Terrain"))
        {
            if (controller)
            {
                controller.GenerateMesh();
            }
        }
    }
}