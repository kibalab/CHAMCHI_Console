using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharp;
using UdonSharpEditor;
using VRC.Udon;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.Graphs;
using VRC.Udon.Common.Interfaces;
using VRC.Udon.Common;
using UnityEditorInternal;

namespace CHAMCHI.BehaviourEditor
{
    [CustomEditor(typeof(LogPanel), true)]
    public class DebugEditor : Editor
    {
        #region Fields

        bool showBase = false;

        SerializedProperty m_pool;
        SerializedProperty m_text;
        SerializedProperty m_playername;
        SerializedProperty m_instanceOwner;
        SerializedProperty m_maxlen;

        SerializedProperty m_onChamchi;

        SerializedProperty m_prefix_red;
        SerializedProperty m_prefix_white;
        SerializedProperty m_prefix_green;
        SerializedProperty m_prefix_yellow;

        Color Red = Color.red, White = Color.white, Green = new Color(0, 0.5f, 0), Yellow = Color.yellow;

        private ReorderableList list;

        #region SubUtilClasses

        private static BehaviourAutoSetter BehaviourAutoSetter;

        #endregion

        #endregion

        private void OnEnable()
        {
            m_pool = serializedObject.FindProperty("pool");
            m_text = serializedObject.FindProperty("text");
            m_playername = serializedObject.FindProperty("playername");
            m_instanceOwner = serializedObject.FindProperty("instanceowner");
            m_maxlen = serializedObject.FindProperty("maxlen");
            m_prefix_red = serializedObject.FindProperty("prefix_red");
            m_prefix_white = serializedObject.FindProperty("prefix_white");
            m_prefix_green = serializedObject.FindProperty("prefix_green");
            m_prefix_yellow = serializedObject.FindProperty("prefix_yellow");

            m_onChamchi = serializedObject.FindProperty("OnChamchi");

            LoadSystemColor();
            
            BehaviourAutoSetter = new BehaviourAutoSetter();

            list = new ReorderableList(BehaviourAutoSetter.GetSettedBehaviours, typeof(List<UdonSharpBehaviour>), false, true, false, false); // Element 가 그려질 때 Callback

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            { // 현재 그려질 요소
                var behaviour = BehaviourAutoSetter.GetSettedBehaviours[index];
                var symbol = BehaviourAutoSetter.GetSettedSymbols[index];
                rect.y += 2; // 위쪽 패딩
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, 500, EditorGUIUtility.singleLineHeight), symbol, behaviour, typeof(UdonSharpBehaviour));
                EditorGUI.EndDisabledGroup();
            };

            list.drawHeaderCallback = (rect) =>
                         EditorGUI.LabelField(rect, "Updated Variables");

        }

        public override void OnInspectorGUI()
        {
            var titleStyle = new GUIStyle();
            titleStyle.normal.background = null;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Box(((LogPanel)target).Title, titleStyle, GUILayout.ExpandWidth(true), GUILayout.Height(100));
            GUILayout.Space(20);
            
            GUI.skin.label.richText = true;
            if (showBase)
            {
                EditorUtil.MenuBox("Origin Editor", () =>
                {
                    base.OnInspectorGUI();
                }, new ContentStyle(GUI.skin.label.font));
                
                GUILayout.Space(20);
                EditorUtil.DrawSubTitle("Editor");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Show Base Inspector");
                showBase = EditorGUILayout.Toggle(showBase);
                EditorGUILayout.EndHorizontal();
                return;

            }

            var behaviour = ((LogPanel)target);

            serializedObject.Update();
            if (m_maxlen.intValue > 30000)
                EditorGUILayout.HelpBox("Values above 3,0000 may cause rendering problems for continuous output.", MessageType.Warning);
            else
                GUILayout.Space(40f);
            GUILayout.Space(10);
            
            EditorUtil.MenuBox("System", () =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"<b><size=15>" +
                                           $"0000" +
                                           $" / " +
                                           $"</size><size=10>" +
                                           $"{m_maxlen.intValue.ToString("000000")}" +
                                           $"</size>   <size=15>|</size>                 </b>", GUI.skin.label, GUILayout.Width(100));
                
                EditorGUILayout.LabelField("Maximum Character Count", GUILayout.Width(180));
                m_maxlen.intValue = EditorGUILayout.IntSlider(m_maxlen.intValue, 10, 100000);
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(15);

                if (GUILayout.Button("Compile Program & Set Debug Fields"))
                {
                    UpdateIncludeScripts();
                }
            }, new ContentStyle(behaviour.text.font));

            GUILayout.Space(20);
            
            EditorUtil.MenuBox("Style", () =>
            {
                EditorUtil.SubMenuBox("Color", () =>
                {
                    var inspectorWidth = EditorGUIUtility.currentViewWidth;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"<b><size=15>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                               $" | " +
                                               $"<color=grey>[System] </color>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>Hello, KIBA!</color>" +
                                               $"</size></b>", GUI.skin.label);
                    if(inspectorWidth > 600) GUILayout.FlexibleSpace();
                    White = EditorGUILayout.ColorField(White, GUILayout.MaxWidth(100));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"<b><size=15>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                               $" | " +
                                               $"<color=grey>[System] </color>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(Yellow)}>Hello, Iris!</color>" +
                                               $"</size></b>", GUI.skin.label);
                    if(inspectorWidth > 600) GUILayout.FlexibleSpace();
                    Yellow = EditorGUILayout.ColorField(Yellow, GUILayout.MaxWidth(100));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"<b><size=15>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                               $" | " +
                                               $"<color=grey>[System] </color>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(Red)}>Hello, Udon!</color>" +
                                               $"</size></b>", GUI.skin.label);
                    if(inspectorWidth > 600) GUILayout.FlexibleSpace();
                    Red = EditorGUILayout.ColorField(Red, GUILayout.MaxWidth(100));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    Green = EditorGUILayout.ColorField(Green, GUILayout.Width(110));
                    if (GUILayout.Button("Save System Color"))
                    {
                        UpdateSystemColor();
                    }
                    EditorGUILayout.EndHorizontal();
                }, new ContentStyle(behaviour.text.font));

                GUILayout.Space(10);

                if (behaviour.text != null) GUI.skin.label.font = behaviour.text.font;

                EditorUtil.SubMenuBox("Highlight", () =>
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"<b><size=15>" +
                                               $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                               $" | " +
                                               $"<color=grey>[System] </color>" +
                                               (m_onChamchi.boolValue ? $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>Do you know <b><color=#50bcdf>CHAMCHI</color></b>, <b><color=#50bcdf>참치</color></b>?</color>"
                                                   : $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>Do you know CHAMCHI, 참치?</color>") +
                                               $"</size></b>", GUI.skin.label);

                    m_onChamchi.boolValue = EditorGUILayout.ToggleLeft("Toggle Highlight", m_onChamchi.boolValue);
                    EditorGUILayout.EndHorizontal();
                }, new ContentStyle(behaviour.text.font));
                

            }, new ContentStyle(behaviour.text.font));
            

            GUILayout.Space(10);

            if (m_pool.objectReferenceValue == null || m_text.objectReferenceValue == null || m_playername.objectReferenceValue == null || m_instanceOwner == null)
                EditorGUILayout.HelpBox("The component setting is missing.\nIt doesn't work at runtime.", MessageType.Error);
            else
                GUILayout.Space(40f);
            
            GUILayout.Space(10);

            behaviour.m_foldAdvanced = EditorUtil.FoldoutMenuBox("Advance Settings", behaviour.m_foldAdvanced, () =>
            {

                EditorUtil.SubMenuBox("Auto Setter", () =>
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Auto Set All Debug Fields");
                    behaviour.m_AutoSet = EditorGUILayout.Toggle(behaviour.m_AutoSet);
                    EditorGUILayout.EndHorizontal();
                }, new ContentStyle(behaviour.text.font));
                GUILayout.Space(15);
                EditorUtil.SubMenuBox("System Components", () =>
                {
                    EditorGUILayout.PropertyField(m_pool);
                    EditorGUILayout.PropertyField(m_text);
                    EditorGUILayout.PropertyField(m_playername);
                    EditorGUILayout.PropertyField(m_instanceOwner);
                }, new ContentStyle(behaviour.text.font));

                GUILayout.Space(10);

                if(BehaviourAutoSetter.GetSettedBehaviours.Count > 0) list.DoLayoutList();
            }, new ContentStyle(behaviour.text.font));

            GUILayout.Space(20);
            EditorUtil.DrawSubTitle("Editor");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Show Base Inspector");
            showBase = EditorGUILayout.Toggle(showBase);
            EditorGUILayout.EndHorizontal();
            
            

            serializedObject.ApplyModifiedProperties();

            GUI.skin.label.richText = false;
        }

        #region Sub Invoke Functions

        public void UpdateIncludeScripts()
        {
            BehaviourAutoSetter.Run(target);
        }

        public void UpdateSystemColor()
        {
            m_prefix_red.stringValue = $"<color=#{ColorUtility.ToHtmlStringRGB(Red)}>";
            m_prefix_white.stringValue = $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>";
            m_prefix_green.stringValue = $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>";
            m_prefix_yellow.stringValue = $"<color=#{ColorUtility.ToHtmlStringRGB(Yellow)}>";
        }

        public void LoadSystemColor()
        {
            ColorUtility.TryParseHtmlString(m_prefix_red.stringValue.Replace("<color=", "").Replace(">", ""), out Red);
            ColorUtility.TryParseHtmlString(m_prefix_white.stringValue.Replace("<color=", "").Replace(">", ""), out White);
            ColorUtility.TryParseHtmlString(m_prefix_green.stringValue.Replace("<color=", "").Replace(">", ""), out Green);
            ColorUtility.TryParseHtmlString(m_prefix_yellow.stringValue.Replace("<color=", "").Replace(">", ""), out Yellow);
        }
        

        #endregion
    }
}