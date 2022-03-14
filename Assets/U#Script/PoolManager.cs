
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PoolManager : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)] private int[] _idArr = new int[100];

    public SyncedObject[] syncedObjectArr;
    private int myIndex = -1;
    public LogPanel logPanel;

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        /* player join => allocateID to list */
        /*************Owner Only**************/
        logPanel.PrintLog("playerjoineed : " + player.displayName);
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            logPanel.PrintLog("you are not owner");
            return;
        }


        for (int i = 0; i < _idArr.Length; i++)
        {
            if (_idArr[i] == 0)
            {
                _idArr[i] = player.playerId;
                break;
            }
        }
        RequestSerialization();
        SyncArr();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        logPanel.LogWarn(this, "player left : " + player.displayName + " " + player.playerId);
        
        /* player left => Deallocate from list */
        /***************Owner Only**************/
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)){
            logPanel.LogError(this, "OnplayerLeft : You are not pool owner");
            return;
        }
        logPanel.Log(this, "OnplayerLeft : you are pool owner!");

        for (int i = 0; i < _idArr.Length; i++)
        {
            if (VRCPlayerApi.GetPlayerById(_idArr[i]) == null)
            {
                logPanel.Log(this,"Remove " + _idArr[i].ToString() + " from the pool");
                _idArr[i] = 0;
            }
        }

        RequestSerialization();
        SyncArr();
    }

    public override void OnDeserialization()
    {
        SyncArr();
    }

    public void SyncArr()
    {
        /* 1회만 작동하면 됨*/
        if (myIndex != -1)
            return;
        int myid = Networking.LocalPlayer.playerId;

        /* pool 내에 할당되었는지 체크 */
        for (int i = 0; i < _idArr.Length; i++)
        {
            if (_idArr[i] == Networking.LocalPlayer.playerId)
            {
                myIndex = i;
                break;
            }
        }

        /* 할당안되었으면 무시*/
        /* join직후 받아오는 _idArr에는 아직 동기화 안되어있을 수 있음 */
        if (myIndex == -1)
            return;


        syncedObjectArr[myIndex].getOwner();
        Debug.Log("SyncArr success");
    }



    public void getStatus()
    {
        /* this is for debug*/
        string tmp = "";

        for (int i = 0; i < 20; i++)
        {
            tmp += _idArr[i].ToString();
        }
    }





    public void bridgeLog(string data)
    {
        /* pool not registered */
        if (myIndex == -1)
            return;

        syncedObjectArr[myIndex].sendLog(data);
    }

}
