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
    /*
     * USB : UdonSharpBehaviour
     */

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

        #region MainFunctions

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
        
        public void Compile() => UdonSharpProgramAsset.CompileAllCsPrograms(true);
        
        public void AutoSet(Object target)
        {
            var selfUdonBehaviour = UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour) target);

            AutoSettedBehaviours.Clear();
            AutoSettedSymbols.Clear();

            var behaviours = GetAllUSB(true);

            foreach (var behaviour in behaviours)
            {
                var variables = GetUSBPublicFields(behaviour);

                bool isChanged = false;
                foreach (var variable in variables)
                {
                    if (variable.FieldType == typeof(LogPanel))
                    {
                        isChanged = SetVariableValue(behaviour, variable, selfUdonBehaviour) || isChanged;
                    }
                }

                GUI.changed = isChanged;

                if(isChanged) SaveChangedUSBPrefab(behaviour);
            }
        }
        
        #endregion

        #region Utility

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

        public List<UdonSharpBehaviour> GetAllUSB(bool includeInacitve)
        {
            var behaviours = new List<UdonSharpBehaviour>();
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                behaviours.AddRange(root.GetComponentsInChildren<UdonSharpBehaviour>(includeInacitve));
            }

            return behaviours;
        }

        public FieldInfo[] GetUSBPublicFields(UdonSharpBehaviour behaviour)
        {
            var type = behaviour.GetType();
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        public bool SetVariableValue(UdonSharpBehaviour behaviour, FieldInfo field, object value)
        {
            var udon = behaviour.GetComponent<UdonBehaviour>();

            ((UdonSharpProgramAsset) udon.programSource).UpdateProgram();


            Undo.RecordObject(udon, "Modify variable");
            if (!udon.publicVariables.TrySetVariableValue(field.Name, value))
            {
                var symbolTable = GetSymbolTable(udon);
                var symbolType = symbolTable.GetSymbolType(field.Name);
                if (!udon.publicVariables.TryAddVariable(CreateUdonVariable(field.Name,
                        value,
                        symbolType)))
                {
                    Debug.LogError($"Failed to set public variable '{field.Name}' value");
                    AutoSettedBehaviours.Add(behaviour);
                    AutoSettedSymbols.Add(field.Name + "_NOTSETTED");
                }
                else
                {
                    AutoSettedBehaviours.Add(behaviour);
                    AutoSettedSymbols.Add(field.Name);
                }
            }

            udon.SetProgramVariable(field.Name, value);
            return true;
        }

        public void SaveChangedUSBPrefab(UdonSharpBehaviour behaviour)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(behaviour))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(behaviour);
            }
        }

        #endregion
    }
}