
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace K13A.KDebug
{
#if UDON
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UserNetworkUnit : UdonSharpBehaviour
    {
        [UdonSynced(UdonSyncMode.None), HideInInspector, FieldChangeCallback(nameof(NetworkLog))] public string m_NetworkLog = "";
        public KDebug Debug;
        [UdonSynced(UdonSyncMode.None), HideInInspector, FieldChangeCallback(nameof(UserID))] public int OwnerID = -1;
        public string ColorCode = "";

        public string receivedLog = "";

        private char[] hex = { '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', };

        public string NetworkLog
        {
            set
            {
                receivedLog += value;

                Debug.m_NetworkLog(value);
                m_NetworkLog = value;
            }

            get => m_NetworkLog;
        }

        public string SendLog
        {
            set
            {
                m_NetworkLog = value;
                receivedLog += value;
                Debug.AllLogs += value;
            }
        }

        public int UserID
        {
            set
            {
                OwnerID = value;

                var player = VRCPlayerApi.GetPlayerById(value);

                if(player == null)
                {
                    player = Networking.GetOwner(gameObject);
                    Debug.LogError(this, $"Debug Network Unit is broken ID : {value}, Current Owner : {player.displayName}.{player.playerId}");
                    return;
                }
                Debug.Log(this, $"Initialize Network Unit | {player.displayName}.{value}");
                Debug.Log(this, $"Debug Network Unit is ready");

                if(!Debug.FilterDropDown.IsExists(this))
                    Debug.FilterDropDown.AddItem($"{player.displayName}.{player.playerId}", this);
                else
                    Debug.Log(this, $"Network Unit already exists.");
            }
        }

        public void TrySync()
        {
            RequestSerialization();
        }

        public void SetOwner()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            OwnerID = Networking.LocalPlayer.playerId;
            RequestSerialization();
        }

        public void Init()
        {
            ResetUnit();
            ColorCode = $"#{hex[Random.Range(0, 7)]}{hex[Random.Range(0, 7)]}{hex[Random.Range(0, 7)]}{hex[Random.Range(0, 7)]}{hex[Random.Range(0, 7)]}{hex[Random.Range(0, 7)]}";
        }

        public void ClearLog()
        {
            receivedLog = "";
        }

        public void ResetUnit()
        {
            ClearLog();
            OwnerID = -1;
            ColorCode = "";
            m_NetworkLog = "";

            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ClearLog));

            Debug.Log(this, $"Reset NetworkUnit | OwnerID: {OwnerID}");
        }

        public override string ToString()
        {
            var player = VRCPlayerApi.GetPlayerById(OwnerID);
            return $"[UNU({OwnerID}, {ColorCode})]";
        }

        public void LogSelf()
        {
            Debug.Log(this, ToString());
        }
    }

#else
    public class TestLogger : MonoBehaviour
    {
        public bool PleaseImportUDON = false;
    }
#endif
}