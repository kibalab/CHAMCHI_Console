
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToggleDebug : UdonSharpBehaviour
{
    public LogPanel logPanel;

    private void Start() {
        logPanel.gameObject.SetActive(!logPanel.gameObject.activeSelf);
    }
    
    public override void Interact() {
        logPanel.gameObject.SetActive(!logPanel.gameObject.activeSelf);
    }
}
