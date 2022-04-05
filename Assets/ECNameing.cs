using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ECNaming : EditorWindow
{
    private bool _bEditOn = true;

    [SerializeField]
    public List<GameObject> ins_GameObjects = new List<GameObject>();

    [MenuItem("Tools/ECNaming")]
    private static void Open()
    {
        ECNaming win = GetWindow<ECNaming>();
        win.titleContent = new GUIContent("Naming Tool");
        win.Show();
    }
    private void OnEnable()
    {
        _bEditOn = true;
    }
    private void OnDisable()
    {
        _bEditOn = false;
        _nCount = 0;
    }

    private Editor _editor;
    private string _strName = null;
    private int _nCount = 0;

    private void OnGUI()
    {
        if (!_editor) { _editor = Editor.CreateEditor(this); }
        if (_editor) { _editor.OnInspectorGUI(); }

        GUILayout.BeginVertical("BOX");

        GUILayout.Space(10);
        GUILayout.Label("변경 할 이름", GUILayout.Width(75));
        _strName = EditorGUILayout.TextField(_strName, GUILayout.ExpandWidth(true));
        GUILayout.Label("시작 인덱스(-1이면 인덱스를 안붙임)", GUILayout.ExpandWidth(true));
        _nCount = EditorGUILayout.IntField(_nCount, GUILayout.ExpandWidth(true));
        GUILayout.Space(10);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("변경"))
        {
            int nIdx = _nCount;
            string strName = _strName;
            for (int i = 0; i < ins_GameObjects.Count; i++)
            {
                strName = _strName;
                if (_nCount != -1)
                {
                    strName += nIdx;
                    nIdx++;
                }
                ins_GameObjects[i].name = strName;
            }

            if (ins_GameObjects.Count == 0)
            {
                Debug.LogError("리스트가 비어있음");
            }
            else
            {
                Debug.LogError("변경 완료");
            }
        }
    }
}

[CustomEditor(typeof(ECNaming), true)]
public class ECNamingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var list = serializedObject.FindProperty("ins_GameObjects");
        EditorGUILayout.PropertyField(list, new GUIContent("오브젝트 리스트"), true);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("초기화"))
        {
            list.arraySize = 0;
        }
    }
}