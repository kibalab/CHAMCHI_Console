
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncedObject : UdonSharpBehaviour
{
    
    [UdonSynced] private string _syncedLogData = "default"; //string null 비교시 or 동기화시 문제있는듯?
    public LogPanel logPanel;
    private bool isLogSaved = false;

    private bool isFirstSync = true;

    private string lastlog = null;

    public string savedRemoteLog = "";
    public string savedLocalLog = "";

    
    public void getOwner(){
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }


    /* 기존 유저들 저장되어있는 첫 로그값은 무시 */
    /* 나중에 들어올 유저들 로그값은 무시하면 안됨*/
    public void setIsFirstSync(bool value){
        isFirstSync = value;
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

    public override void OnDeserialization() {
        //logPanel.PrintLog("OnDeserialization : " + this.gameObject.name);
        if(lastlog == _syncedLogData || _syncedLogData == "default")
            return;

        lastlog = _syncedLogData;
        /* 첫 싱크시 저장되어있는 다른 유저들 로그들 나오는거 방지*/
        if(isFirstSync){
            isFirstSync = false;
            return;
        }
        //logPanel.PrintLog("on deserialization 통과 : " + this.gameObject.name);
        remoteLog(_syncedLogData);
    }
    

    public void localLog(string data){
        logPanel.PrintLog(data);
    }

    public void remoteLog(string data){
        logPanel.PrintLog(data);
    }

    public void resetLog(){
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        _syncedLogData = "default";
        RequestSerialization();
        //logPanel.LogError(this, this.gameObject.name + " resetcomplete");
    }


    public void getlog(){
        logPanel.PrintLog(this.gameObject.name + " : " + _syncedLogData);
    }
    private void Update() {
        
        if(Input.GetKeyDown(KeyCode.F10)){
            getlog();
        }
    }

}
