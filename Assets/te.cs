
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TestClass : UdonSharpBehaviour
{
    public LogPanel Debug;
    
    void Start()
    {
        Debug.Log(this, "Hello, World!");

        Debug.LogWarn(this, "Warning!!");

        Debug.LogError(this, "Udon Error!!");
    }
}
