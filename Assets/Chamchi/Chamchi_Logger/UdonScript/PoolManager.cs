
using CHAMCHI.UI;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace CHAMCHI.Behaviour
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PoolManager : UdonSharpBehaviour
    {
        [UdonSynced(UdonSyncMode.None)] private int[] _idArr = new int[100];

        public SyncedObject[] syncedObjectArr;
        public int myIndex = -1;
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
                else if (VRCPlayerApi.GetPlayerById(_idArr[i]) == null)
                {
                    //logPanel.Log(this, "Remove " + _idArr[i].ToString() + " from the pool");
                    syncedObjectArr[i].resetOnPlayerExit();
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
            dropDown.RefreshItem(_idArr);

            /*?????? ??????????????? ??????*/
            ignoreFisrtLog();

            /* 1?????? ???????????? ???*/
            if (myIndex != -1)
                return;
            int myid = Networking.LocalPlayer.playerId;

            /* pool ?????? ?????????????????? ?????? */
            for (int i = 0; i < _idArr.Length; i++)
            {
                if (_idArr[i] == Networking.LocalPlayer.playerId)
                {
                    myIndex = i;
                    break;
                }
            }

            /* ????????????????????? ??????*/
            /* join?????? ???????????? _idArr?????? ?????? ????????? ??????????????? ??? ?????? */
            if (myIndex == -1)
                return;


            syncedObjectArr[myIndex].getOwner(); // KIBA : ??? GetOwner??????...?


        }


        /* ???????????? ??????????????? isFirstLog = false */
        /* ????????? ?????? true??? ?????? ????????? ??? ?????? ?????? */
        public void ignoreFisrtLog()
        {
            for (int i = 0; i < _idArr.Length; i++)
            {
                if (_idArr[i] == 0)
                {
                    /*array out of bound*/
                    /*?????? ?????? ????????? ?????????????????? */
                    if (i >= syncedObjectArr.Length)
                    {
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




        /* LogPanel??? SyncedObject ????????? */
        public void bridgeLog(string data)
        {
            /* pool not registered */
            if (myIndex == -1)
                return;
            syncedObjectArr[myIndex].sendLog(data);
        }

        public string bridgeSavedLog(int index) => syncedObjectArr[index].getSavedLog();

        public void bridgeResetLog(int index) => syncedObjectArr[index].resetSavedLog();

        public void bridgeResetLogAll()
        {
            for (int i = 0; i < syncedObjectArr.Length; i++)
            {
                syncedObjectArr[i].resetSavedLog();

            }

        }

    }

}