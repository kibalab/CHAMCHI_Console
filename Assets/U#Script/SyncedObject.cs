
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
    private string perUserSavedLog = "";

    
    public void getOwner(){
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }


    /* 기존 유저들 저장되어있는 첫 로그값은 무시 */
    /* 나중에 들어올 유저들 로그값은 무시하면 안됨*/
    public void setIsFirstSync(bool value){
        isFirstSync = value;
    }

    public void sendLog(string data){
        /* print log : Local only */
        printLog(data);
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
        /* print log : Remote only */
        printLog(_syncedLogData);
    }
    
    public void printLog(string log){
        perUserSavedLog += log +'\n';
        logPanel.PrintLog(log, int.Parse(this.gameObject.name));

    }

    public string getSavedLog() => perUserSavedLog;


    /* 수신한 remote log 해당 유저 나갈때 초기화 */
    public override void OnOwnershipTransferred(VRCPlayerApi player) {
        perUserSavedLog = "";
    }
    public void resetLog(){
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        _syncedLogData = "default";
        RequestSerialization();
        //logPanel.LogError(this, this.gameObject.name + " resetcomplete");
    }



}
