using System;
using System.Collections.Generic;
using System.Reflection;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using Object = UnityEngine.Object;

namespace CHAMCHI.BehaviourEditor
{

    public class BehaviourAutoSetter
    {
        #region Field

        List<UdonSharpBehaviour> AutoSettedBehaviours = new List<UdonSharpBehaviour>();
        List<string> AutoSettedSymbols = new List<string>();

        #endregion

        #region Properties

        public List<UdonSharpBehaviour> GetSettedBehaviours
        {
            get => AutoSettedBehaviours;
        }

        public List<string> GetSettedSymbols
        {
            get => AutoSettedSymbols;
        }

        #endregion

        public bool Run(Object target)
        {
            try
            {
                Compile();
                AutoSet(target);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        #region Utility

        public void AutoSet(Object target)
        {
            var selfUdonBehaviour = UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour) target);

            AutoSettedBehaviours.Clear();
            AutoSettedSymbols.Clear();

            var behaviours = GetAllBehaviours();

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

                        ((UdonSharpProgramAsset) udon.programSource).UpdateProgram();


                        Undo.RecordObject(udon, "Modify variable");
                        if (!udon.publicVariables.TrySetVariableValue(variable.Name, selfUdonBehaviour))
                        {
                            var symbolTable = GetSymbolTable(udon);
                            var symbolType = symbolTable.GetSymbolType(variable.Name);
                            if (!udon.publicVariables.TryAddVariable(CreateUdonVariable(variable.Name,
                                    selfUdonBehaviour,
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

        static IUdonVariable CreateUdonVariable(string symbolName, object value, System.Type type)
        {
            System.Type udonVariableType = typeof(UdonVariable<>).MakeGenericType(type);
            return (IUdonVariable) Activator.CreateInstance(udonVariableType, symbolName, value);
        }

        IUdonSymbolTable GetSymbolTable(UdonBehaviour udonBehaviour)
        {

            if (!udonBehaviour || !(udonBehaviour.programSource is UdonSharpProgramAsset))
            {
                throw new Exception("ProgramSource is not an UdonSharpProgramAsset");
            }

            var programAsset = (UdonSharpProgramAsset) udonBehaviour.programSource;
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

        public List<UdonSharpBehaviour> GetAllBehaviours()
        {
            var behaviours = new List<UdonSharpBehaviour>();
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                behaviours.AddRange(root.GetComponentsInChildren<UdonSharpBehaviour>(true));
            }

            return behaviours;
        }

        public void Compile()
        {
            UdonSharpProgramAsset.CompileAllCsPrograms(true);
        }

        #endregion
    }
}