namespace CHAMCHI.BehaviourEditor;

public class BehaviourAutoSetter
{
    #region Field

    List<UdonSharpBehaviour> AutoSettedBehaviours = new List<UdonSharpBehaviour>();
    List<string> AutoSettedSymbols = new List<string>();
    
    #endregion
    
    public void Run()
    {
        AutoSet();
    }

    #region Utility

    public void AutoSet()
    {
        Compile();

        var selfUdonBehaviour = UdonSharpEditorUtility.GetBackingUdonBehaviour((UdonSharpBehaviour)target);

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

    public List<UdonSharpBehaviour> GetAllBehaviours()
    {
        var behaviours = List<UdonSharpBehaviour>(); 
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var root in roots)
        {
            behaviours.AddRange(root.GetComponentsInChildren<UdonSharpBehaviour>(true));
        }

        return behaviours;
    }

    public void Compile(UdonSharpBehaviour behaviour)
    {
    }
    
    public void Compile()
    {
        UdonSharpProgramAsset.CompileAllCsPrograms(true);
    }
    
    #endregion
}