using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
public class JHVertexPainter : EditorWindow
{
    bool isPainting;
    Vector3 hitPoint;
    Color gizmoColor=new Color(0,160,255, 122);
    Color gizmoOutlineColor = new Color(0, 0, 255, 255);
    int channel=0;
    float brushSize=0.25f;
    float brushStrength = 0.5f;
    bool isClicked;
    Vector2 scrollPos;
    [MenuItem("Window/JH Dev/Vertex Painter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(JHVertexPainter));
    }
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
    void OnGUI()
    {
        scrollPos =
               EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("브러쉬 셋팅", GUI.skin.box, GUILayout.Width(150.0f));
        GUILayout.Space(20);
        brushSize = EditorGUILayout.Slider("브러쉬 사이즈", brushSize, 0, 10);
        brushStrength = EditorGUILayout.Slider("브러쉬 Strength", brushStrength, 0, 1);


        GUILayout.Space(20);
        GUILayout.Label("드로우 채널", GUI.skin.box, GUILayout.Width(150.0f));
        GUILayout.Space(20);
        string channelStr = (channel == 0) ? "R" : (channel == 1) ? "G" : (channel == 2) ? "B" : "A";
        GUILayout.Label("선택된 채널 : "+ channelStr);
        GUILayout.BeginHorizontal();


        GUI.color = new Color(1, 0, 0);
        if (GUILayout.Button("R채널"))
            channel = 0;
        GUI.color = new Color(0, 1, 0);
        if (GUILayout.Button("G채널"))
            channel = 1;
        GUI.color = new Color(0, 0, 1);
        if (GUILayout.Button("B채널"))
            channel = 2;
        GUILayout.EndHorizontal();
        GUI.color = new Color(1, 1, 1);

        //기지모

        GUILayout.Space(20);
        GUILayout.Label("Gizmos",GUI.skin.box,GUILayout.Width(150.0f));
        GUILayout.Space(20);
        gizmoColor = EditorGUILayout.ColorField("컬러",gizmoColor);
        gizmoOutlineColor = EditorGUILayout.ColorField("아웃라인 컬러", gizmoOutlineColor);

        // 페인트버튼 
        GUILayout.Space(20);
        string paintingButtonStr = (isPainting) ? "페인팅 멈춤" : "페인팅 시작";
        if (GUILayout.Button(paintingButtonStr))
        {
            if (isPainting)
            {
                isClicked = false;
            }
            else
            {

                GUI.FocusControl("");

                if (new Rect(GUILayoutUtility.GetLastRect().x - 1, GUILayoutUtility.GetLastRect().y - 5, position.width, 5).Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    GUI.FocusControl("");
                }
            }
            isPainting = !isPainting;
        }

        EditorGUILayout.EndScrollView();
        Repaint();
    }
    void BrushPainting(RaycastHit hit)
    {
        Handles.color = gizmoColor;
        Handles.DrawSolidDisc(hit.point, hit.normal, brushSize);
        Handles.color = gizmoOutlineColor;
        Handles.DrawWireDisc(hit.point, hit.normal,brushSize);
        if (Event.current.button == 0)
        {
            isClicked = true;
        }
        else isClicked = false;
        //if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        //    isClicked = true;
        //else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        //{
        //    isClicked = false;
        //}
        if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            VertexPainting(hit);
        }
        HandleUtility.Repaint();
    }
    void VertexPainting(RaycastHit hit)
    {
        Mesh mesh = hit.transform.GetComponent<MeshFilter>().sharedMesh;
        Color[] colors = mesh.colors;
        Vector3[] vertices = mesh.vertices;
        if (colors.Length < vertices.Length)
            colors = new Color[vertices.Length];
        Vector3 unscaledPos = hit.transform.TransformPoint(hit.point);
        Vector3 scale = hit.transform.localScale;
        unscaledPos = new Vector3(unscaledPos.x / scale.x, unscaledPos.y / scale.y, unscaledPos.z / scale.z);
        /*int closestVertex = GetClosestVertex(mesh, unscaledPos, hit.triangleIndex);
        Vector3 scaledPos*/
        for (int i=0;i<vertices.Length;i++)
        {
            Vector3 scaledPos = hit.transform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(scaledPos, hit.point);
            float colorAmount = (1 - distance / brushSize)*brushStrength;
            if (distance<=brushSize)
            //if((vertices[i]-vertices[closestVertex]).magnitude<=brushSize)
            {
                Color targetColor=(channel == 0) ? Color.red : (channel == 1) ? Color.green : (channel == 2) ? Color.blue : Color.black;

                Color lerpedColor = Color.Lerp(colors[i], targetColor, colorAmount);
                colors[i] = lerpedColor;
            }
        }
        mesh.colors = colors;
        EditorUtility.SetDirty(mesh);
    }
    int GetClosestVertex(Mesh targetMesh, Vector3 localPosition, int triangleIndex)
    {
        int closestVertex = -1;
        float dist=-1;
        for (int i = 0; i < 3; i++)
        {
            float dis = Vector3.Distance(localPosition, targetMesh.vertices[targetMesh.triangles[triangleIndex * 3 + i]]);
            if (dist==-1||dis < dist)
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
    void OnSceneGUI(SceneView sceneView)
    {
        if (!isPainting)
            return;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        //Debug.Log("te");
        Vector2 guiPosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,1000f))
        {
            BrushPainting(hit);
        }

        //OnDrawGizmos();
    }

}