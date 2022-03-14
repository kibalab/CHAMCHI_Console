
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedObject : UdonSharpBehaviour
{
    
    [UdonSynced] private string _syncedLogData;
    public LogPanel logPanel;
    private bool isLogSaved = false;
    
    public void getOwner(){
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        logPanel.Log(this, "GetOwnerSucess");

    }

    

    public void sendLog(string data){
        localLog(data);

        if(!isLogSaved){
            _syncedLogData = data;
            isLogSaved = true;
        }else{
            _syncedLogData +='\n' + data;
        }
        RequestSerialization();
    }
    public override void OnPreSerialization() {
        //logPanel.Log(this, "OnPreSerialization");
        isLogSaved = false;
    }
    public void localLog(string data){
        logPanel.PrintLog(data);
    }

    public override void OnDeserialization() {
        if(_syncedLogData == "")
            return;
        remoteLog(_syncedLogData);
    }
    

    public void remoteLog(string data){
        logPanel.PrintLog(data);
    }

    public void resetLog(){
        _syncedLogData="";
        RequestSerialization();
    }

}
