
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

    public DropDown dropDown;

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        /* player join => allocateID to list */
        /*************Owner Only**************/
        
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
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

        logPanel.LogError(this, "player joined! pool allocation success : " + player.displayName);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        /* player left => Deallocate from list */
        /***************Owner Only**************/
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            return;

        for (int i = 0; i < _idArr.Length; i++)
        {
            if (_idArr[i] == 0)
            {
                continue;
            }
            else if(VRCPlayerApi.GetPlayerById(_idArr[i]) == null)
            {
                //logPanel.Log(this, "Remove " + _idArr[i].ToString() + " from the pool");
                dropDown.DeleteItembyData(syncedObjectArr[i]);
                syncedObjectArr[i].resetLog(); 
                _idArr[i] = 0;
            }
        }

        RequestSerialization();
        SyncArr();
        logPanel.LogError(this, "player left! pool deallocation success : " + player.displayName);
        
    }

    public override void OnDeserialization()
    {
        SyncArr();
    }

    public void SyncArr()
    {
        /*유저 나갈때마다 처리*/
        ignoreFisrtLog();

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


        syncedObjectArr[myIndex].getOwner(); // KIBA : 왜 GetOwner에요...?

        
    }


    /* 미할당된 오브젝트는 isFirstLog = false */
    /* 입장시 모두 true로 기존 유저들 첫 로그 무시 */
    public void ignoreFisrtLog(){
        for(int i = 0; i < _idArr.Length; i++){
            if(_idArr[i] == 0){
                /*array out of bound*/
                /*현재 개수 안맞음 나중에맞출것 */
                if(i >= syncedObjectArr.Length){
                    break;
                }
                syncedObjectArr[i].setIsFirstSync(false);
            }
        }
    }



      /* this is for debug*/
    public void getStatus()
    {
        string tmp = "";

        for (int i = 0; i < 20; i++)
        {
            tmp += _idArr[i].ToString();
        }
        logPanel.Log(this, tmp);
    }




    /* LogPanel과 SyncedObject 연결용 */
    public void bridgeLog(string data)
    {
        /* pool not registered */
        if (myIndex == -1)
            return;
        syncedObjectArr[myIndex].sendLog(data);
    }

}
