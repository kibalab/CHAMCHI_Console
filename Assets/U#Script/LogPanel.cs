
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

    private string prefix_red = "<color=red>";
    private string prefix_white = "<color=white>";
    private string prefix_green = "<color=green>";
    private string prefix_yellow = "<color=yellow>";
    private string suffix = "</color>";
    private char[] hex = { '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', };
    private string prefix_username;

    private string logAll;

    public DropDown dropDown;

    /*
        <color=green>[22:22:22.22]</color> | <color=white> Hello, World!</color>
    <color=green>[22:22:22.22]</color> | <color=orange> Oh, Warning!</color>
    <color=green>[22:22:22.22]</color> | <color=red> No, Error!</color>
      */

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
    }

    private string setLogData(UnityEngine.Object classObject, string data, int logtype)
    {
        

        string timedata = prefix_green +"[" + DateTime.Now.ToString("HH:mm:ss.ff") + "]"+ suffix + " | ";

        string username;
        if(Networking.IsInstanceOwner){
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

    public void OnDropDownChanged(int arrIndex) // Occurs when DropDown Item is changed. 드롭다운 항목이 변경될 때 발생합니다.
    {
        switch(arrIndex){
            case 0:
                text.text = pool.getLocalLog(arrIndex);
                break;
            case 1:
                /* all */
                text.text = logAll;
                break;
            default:
                text.text = pool.getRemoteLog(arrIndex);
                break;
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
            if( (int)dropDown.Items[dropDown.SelectedID].Data == syncedObjectIndex){
                text.text += data + '\n';
            }
        }
        logAll += data + '\n';
    }


}
