using UnityEngine;
using UnityEditor;
using System.Collections;
[CustomEditor(typeof(FarmController))]
public class FarmControllerEditor : Editor
{
    public override void OnInspectorGUI()   //OnInspectorGUI 에 오버라이드 해 줍니다.
    {
        base.OnInspectorGUI();
        FarmController controller = target as FarmController;
        if (GUILayout.Button("Generate Tilemap"))
        {
            if (controller)
            {
                controller.GenerateMesh();
            }
        }
    }
}