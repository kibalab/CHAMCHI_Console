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
using VRC.Udon.Common.Interfaces;
using VRC.Udon.Common;
using UnityEditorInternal;

namespace CHAMCHI.BehaviourEditor
{
    [CustomEditor(typeof(LogPanel), true)]
    public class DebugEditor : Editor
    {
        bool showBase = false;

        List<UdonSharpBehaviour> AutoSettedBehaviours = new List<UdonSharpBehaviour>();
        List<string> AutoSettedSymbols = new List<string>();

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

            list = new ReorderableList(AutoSettedBehaviours, typeof(List<UdonSharpBehaviour>), false, true, false, false); // Element 가 그려질 때 Callback

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            { // 현재 그려질 요소
                var behaviour = AutoSettedBehaviours[index];
                var symbol = AutoSettedSymbols[index];
                rect.y += 2; // 위쪽 패딩
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, 500, EditorGUIUtility.singleLineHeight), symbol, behaviour, typeof(UdonSharpBehaviour));
                EditorGUI.EndDisabledGroup();
            };

            list.drawHeaderCallback = (rect) =>
                         EditorGUI.LabelField(rect, "Updated Variables");

        }

        static IUdonVariable CreateUdonVariable(string symbolName, object value, System.Type type)
        {
            System.Type udonVariableType = typeof(UdonVariable<>).MakeGenericType(type);
            return (IUdonVariable)Activator.CreateInstance(udonVariableType, symbolName, value);
        }

        public void UpdateIncludeScripts()
        {
            UdonSharpProgramAsset.CompileAllCsPrograms(true);

            var roots = SceneManager.GetActiveScene().GetRootGameObjects();

            var selfUdonBehaviour = UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)target);

            AutoSettedBehaviours.Clear();
            AutoSettedSymbols.Clear();


            foreach (var root in roots)
            {
                var behaviours = root.GetComponentsInChildren<UdonSharpBehaviour>();
                foreach (var behaviour in behaviours)
                {
                    var type = behaviour.GetType();
                    FieldInfo[] variables = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                    FieldInfo debugVar = null;
                    foreach (var variable in variables)
                    {
                        if (variable.FieldType == typeof(LogPanel))
                        {
                            debugVar = variable;
                            var udon = behaviour.GetComponent<UdonBehaviour>();

                            ((UdonSharpProgramAsset)udon.programSource).UpdateProgram();


                            Undo.RecordObject(udon, "Modify variable");
                            if (!udon.publicVariables.TrySetVariableValue(variable.Name, selfUdonBehaviour))
                            {
                                var symbolTable = GetSymbolTable(udon);
                                var symbolType = symbolTable.GetSymbolType(variable.Name);
                                if (!udon.publicVariables.TryAddVariable(CreateUdonVariable(variable.Name, selfUdonBehaviour,
                                    symbolType)))
                                {
                                    Debug.LogError($"Failed to set public variable '{variable.Name}' value");
                                    AutoSettedBehaviours.Add(behaviour);
                                    AutoSettedSymbols.Add(debugVar.Name + "_NOTSETTED");
                                }
                                else
                                {
                                    AutoSettedBehaviours.Add(behaviour);
                                    AutoSettedSymbols.Add(debugVar.Name);
                                }
                            }
                            udon.SetProgramVariable(variable.Name, target);
                        }
                    }

                    GUI.changed = true;

                    if (PrefabUtility.IsPartOfPrefabInstance(behaviour))
                    {
                        PrefabUtility.RecordPrefabInstancePropertyModifications(behaviour);
                    }
                }
            }
        }

        IUdonSymbolTable GetSymbolTable(UdonBehaviour udonBehaviour)
        {
            if (!udonBehaviour || !(udonBehaviour.programSource is UdonSharpProgramAsset))
            {
                throw new Exception("ProgramSource is not an UdonSharpProgramAsset");
            }

            var programAsset = (UdonSharpProgramAsset)udonBehaviour.programSource;
            if (!programAsset)
            {
                throw new Exception("UdonBehaviour has no UdonSharpProgramAsset");
            }

            programAsset.UpdateProgram();

            var program = programAsset.GetRealProgram();
            if (program?.SymbolTable == null)
            {
                throw new Exception("UdonBehaviour has no public variables");
            }

            return program.SymbolTable;
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

        //UdonSharpEditorUtility.ConvertToUdonBehavioursInternal(Array.ConvertAll(targets, e => e as UdonSharpBehaviour).Where(e => e != null && !UdonSharpEditorUtility.IsProxyBehaviour(e)).ToArray(), true, true, true);
        public override void OnInspectorGUI()
        {
            GUI.skin.label.richText = true;
            if (showBase)
            {
                base.OnInspectorGUI();
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("<b>[Editor]</b>", GUI.skin.label);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Show Base Inspector");
                showBase = EditorGUILayout.Toggle(showBase);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUI.skin.label.richText = false;
                return;

            }

            var behaviour = ((LogPanel)target);

            serializedObject.Update();


            if (m_maxlen.intValue > 30000)
                EditorGUILayout.HelpBox("Values above 3,0000 may cause rendering problems for continuous output.", MessageType.Warning);
            else
                GUILayout.Space(42);


            EditorGUILayout.LabelField("<b>[Message]</b>", GUI.skin.label);
            EditorGUILayout.LabelField("Maximum Count");
            var origin_font = GUI.skin.label.font;
            if (behaviour.text != null) GUI.skin.label.font = behaviour.text.font;
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField($"<b><size=15>" +
                                       $"0000" +
                                       $" / " +
                                       $"</size><size=10>" +
                                       $"{m_maxlen.intValue}" +
                                       $"</size></b>", GUI.skin.label, GUILayout.Width(80));
            m_maxlen.intValue = EditorGUILayout.IntSlider(m_maxlen.intValue, 10, 100000);
            EditorGUILayout.EndHorizontal();
            GUI.skin.label.font = origin_font;


            GUILayout.Space(25);
            if (behaviour.text != null) GUI.skin.label.font = behaviour.text.font;
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"<b><size=15>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                       $" | " +
                                       $"<color=grey>[System] </color>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>Hello, KIBA!</color>" +
                                       $"</size></b>", GUI.skin.label);

            White = EditorGUILayout.ColorField(White);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"<b><size=15>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                       $" | " +
                                       $"<color=grey>[System] </color>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(Yellow)}>Hello, Iris!</color>" +
                                       $"</size></b>", GUI.skin.label);

            Yellow = EditorGUILayout.ColorField(Yellow);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"<b><size=15>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                       $" | " +
                                       $"<color=grey>[System] </color>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(Red)}>Hello, Udon!</color>" +
                                       $"</size></b>", GUI.skin.label);

            Red = EditorGUILayout.ColorField(Red);
            EditorGUILayout.EndHorizontal();
            GUI.skin.label.font = origin_font;

            EditorGUILayout.BeginHorizontal();
            Green = EditorGUILayout.ColorField(Green, GUILayout.Width(110));
            if (GUILayout.Button("Save System Color"))
            {
                UpdateSystemColor();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (behaviour.text != null) GUI.skin.label.font = behaviour.text.font;

            EditorGUILayout.LabelField("<b>[Name Highlight]</b>", GUI.skin.label);
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField($"<b><size=15>" +
                                       $"<color=#{ColorUtility.ToHtmlStringRGB(Green)}>[00:00:00.00]</color>" +
                                       $" | " +
                                       $"<color=grey>[System] </color>" +
                                            (m_onChamchi.boolValue ? $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>Do you know <b><color=#50bcdf>CHAMCHI</color></b>, <b><color=#50bcdf>참치</color></b>?</color>"
                                            : $"<color=#{ColorUtility.ToHtmlStringRGB(White)}>Do you know CHAMCHI, 참치?</color>") +
                                       $"</size></b>", GUI.skin.label);

            m_onChamchi.boolValue = EditorGUILayout.ToggleLeft("Toggle Highlight", m_onChamchi.boolValue);
            EditorGUILayout.EndHorizontal();

            GUI.skin.label.font = origin_font;

            EditorGUILayout.EndVertical();




            GUILayout.Space(15);

            if (GUILayout.Button("Compile Program & Set Debug Fields"))
            {
                UpdateIncludeScripts();
            }

            if (m_pool.objectReferenceValue == null || m_text.objectReferenceValue == null || m_playername.objectReferenceValue == null || m_instanceOwner == null)
                EditorGUILayout.HelpBox("The component setting is missing.\nIt doesn't work at runtime.", MessageType.Error);
            else
                GUILayout.Space(40);

            behaviour.m_foldAdvanced = EditorGUILayout.Foldout(behaviour.m_foldAdvanced, "Advance Settings");
            if (behaviour.m_foldAdvanced)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.LabelField("<b>[Auto Setter]</b>", GUI.skin.label);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Auto Set All Debug Fields");
                behaviour.m_AutoSet = EditorGUILayout.Toggle(behaviour.m_AutoSet);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(15);

                EditorGUILayout.LabelField("<b>[System Components]</b>", GUI.skin.label);
                EditorGUILayout.PropertyField(m_pool);
                EditorGUILayout.PropertyField(m_text);
                EditorGUILayout.PropertyField(m_playername);
                EditorGUILayout.PropertyField(m_instanceOwner);
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

                if(AutoSettedBehaviours.Count > 0) list.DoLayoutList();

            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("<b>[Editor]</b>", GUI.skin.label);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Show Base Inspector");
            showBase = EditorGUILayout.Toggle(showBase);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            GUI.skin.label.richText = false;
        }
    }
}