
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System;

public class LogPanel : UdonSharpBehaviour
{
    public PoolManager pool;
    public Text text;
    public Text playername;
    public Text instanceowner;
    public int maxlen = 40000;

    private string prefix_red = "<color=red>";
    private string prefix_white = "<color=white>";
    private string prefix_green = "<color=green>";
    private string prefix_yellow = "<color=yellow>";
    private string suffix = "</color>";
    private char[] hex = { '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', };
    private string prefix_username;

    private string logAll;

    public DropDown dropDown;

    public Text LogSize;

    public void Log(UnityEngine.Object classObject, string data)
    {
        string logdata = setLogData(classObject, data, 0);
        pool.bridgeLog(logdata);
    }

    public void LogWarn(UnityEngine.Object classObject, string data)
    {
        string logdata = setLogData(classObject, data, 1);
        pool.bridgeLog(logdata);
    }
    public void LogError(UnityEngine.Object classObject, string data)
    {
        string logdata = setLogData(classObject, data, 2);
        pool.bridgeLog(logdata);

    }

    private void Start() {
        string ColorCode = $"#{hex[UnityEngine.Random.Range(0, 7)]}{hex[UnityEngine.Random.Range(0, 7)]}{hex[UnityEngine.Random.Range(0, 7)]}{hex[UnityEngine.Random.Range(0, 7)]}{hex[UnityEngine.Random.Range(0, 7)]}{hex[UnityEngine.Random.Range(0, 7)]}";
        prefix_username = $"<color={ColorCode}>";
        playername.text = "PlayerName : " + Networking.LocalPlayer.displayName;
    }
    public override void OnOwnershipTransferred(VRCPlayerApi player) {
        instanceowner.text = "Instance Owner : " + Networking.GetOwner(this.gameObject).displayName;
    }

    private string setLogData(UnityEngine.Object classObject, string data, int logtype)
    {
        

        string timedata = prefix_green +"[" + DateTime.Now.ToString("HH:mm:ss.ff") + "]"+ suffix + " | ";

        string username;
        if(Networking.LocalPlayer.isMaster){
            username = prefix_username + "[Owner : " + Networking.LocalPlayer.displayName + "] "+ suffix;
        }else{
            username = prefix_username + "[" + Networking.LocalPlayer.displayName + "] "+ suffix;
        }
        string logdata;
        string colortype = null;
        switch (logtype)
        {
            case 0:
                colortype = prefix_white;
                break;
            case 1:
                colortype = prefix_yellow;
                break;
            case 2:
                colortype = prefix_red;
                break;
        }
         logdata = colortype + "[" + ((UdonSharpBehaviour)classObject).GetUdonTypeName() + "] " + data + "</color>";

        return timedata + username + logdata;

    }

    public void OnDropDownChanged() // Occurs when DropDown Item is changed. 드롭다운 항목이 변경될 때 발생합니다.
    {  
        if(dropDown.SelectedID == 1){
         text.text = logAll;
        }else if(dropDown.SelectedID == 0){
            text.text = pool.bridgeSavedLog(pool.myIndex);
        }else{
            text.text = pool.bridgeSavedLog(dropDown.getCurrentItemData());
        }
    }

    public void PrintLog(string data, int syncedObjectIndex)
    {
        if(dropDown.SelectedID == 1){
            /* 전체출력 */
            text.text += data + '\n';
        }else if(dropDown.SelectedID == 0){
            /* 로컬 출력 */
            if(pool.myIndex == syncedObjectIndex){
                text.text += data + '\n';
            }
        }else{
            if( dropDown.getCurrentItemData() == syncedObjectIndex){
                text.text += data + '\n';
            }
        }
        logAll += data + '\n';
        LengthAdjust();
    }


    public void LengthAdjust(){
        while(text.text.Length > maxlen){

            int found = text.text.IndexOf('\n');
            text.text = text.text.Substring(found+1);
        }

        LogSize.text = String.Format("{0} / <size=7>{1}</size>", text.text.Length, maxlen);
        
    }
    public void Clear(){
        text.text = "";
        LogSize.text = String.Format("{0} / <size=7>{1}</size>", 0, maxlen);
        /* remote log clear code */

        int num = dropDown.SelectedID;

        switch(num){
            case 0://local
                pool.bridgeResetLog(pool.myIndex);
                break;
            case 1://all
                pool.bridgeResetLogAll();
                logAll = "";
                break;
            default://remote user
                pool.bridgeResetLog(dropDown.getCurrentItemData());
                break;
        }
    }   


}
