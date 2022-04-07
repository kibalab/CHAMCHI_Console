using System;
using UnityEditor;
using UnityEngine;

namespace CHAMCHI.BehaviourEditor
{
    public static class EditorUtil
    {

        public static void DrawTitle(string Title)
        {
            EditorGUILayout.LabelField("", GUI.skin.window, GUILayout.Height(25));
            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), $"   <size=15><b>{Title}</b></size>", GUI.skin.label);
        }
        
        public static bool DrawFoldoutTitle(bool b, string Title)
        {
            EditorStyles.foldout.richText = true;
            if (GUILayout.Button("", GUI.skin.window, GUILayout.Height(25)))
                b = !b;
            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), $"   <size=15><b>{Title}</b></size>", GUI.skin.label);

            return b;
        }
        
        public static void MenuBox(string Title, Action contant, ContentStyle style)
        {
            var origin_font = GUI.skin.label.font;
            DrawTitle(Title);
            GUI.skin.label.font = style.font;
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.Space(10);
                EditorGUILayout.BeginVertical();
                    contant.Invoke();
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            GUI.skin.label.font = origin_font;
        }
        
        public static bool FoldoutMenuBox(string Title, bool b, Action contant, ContentStyle style)
        {
            var origin_font = GUI.skin.label.font;
            b = DrawFoldoutTitle(b, Title);

            if (!b) return b;
            
            GUI.skin.label.font = style.font;
            
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            contant.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            GUI.skin.label.font = origin_font;
            
            return b;
        }
        
        public static void MenuBox(string Title, Action contant)
        {
            DrawTitle(Title);
            contant.Invoke();
        }
    }
}