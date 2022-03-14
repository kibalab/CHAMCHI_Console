
using K13A.KDebug.UIElement.DropDown;
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace K13A.KDebug
{
#if UDON
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class KDebug : UdonSharpBehaviour
    {
        #region LocalField
        public string normalColor = "white";
        public string warnColor = "orange";
        public string errorColor = "red";


        public int AllowStringCount = 5000;

        public Image BlockButton;
        public Text BlockText;
        public Text CountText;
        public Text logText;
        public Text nameText;
        public Text instOwnerText;
        public Text pingText;

        public KDropDown FilterDropDown;

        private bool isBlock = true;

        public string PausedLogs = "";

        public string AllLogs = "";

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            //if(!player.Equals(Networking.LocalPlayer))
            CheckPool();
            IsPause = false;
        }

        public bool IsPause
        {
            set
            {
                isBlock = value;

                BlockButton.color = value ? new Color(1, 0, 0, 0.1f) : new Color(0, 1, 0, 0.1f);
                BlockText.text = value ? "Pause" : "Play";
                if (value)
                {
                    PausedLogs = "";
                }
                else
                {
                    logText.text += CutLogs(PausedLogs, AllowStringCount);
                }
            }

            get => isBlock;
        }

        #endregion

        #region NetworkField

        public VRCObjectPool UnitPool;
        public UserNetworkUnit NetworkUnit;

        public UserNetworkUnit[] OthersUnits;

        #endregion
        

        public void Update()
        {

            if (NetworkUnit != null && !IsPause)
            {
                switch (FilterDropDown.SelectedID)
                {
                    case 0:
                        NetworkUnit.receivedLog = CutLogs(NetworkUnit.receivedLog, AllowStringCount);
                        logText.text = NetworkUnit.receivedLog;
                        break;
                    case 1:
                        AllLogs = CutLogs(AllLogs, AllowStringCount);
                        logText.text = AllLogs;
                        break;
                    default:
                        var unit = FilterDropDown.GetDataByID(FilterDropDown.SelectedID);
                        if(unit == null)
                        {
                            GlobalAlrt(this, "-------------------------------------------------------" + FilterDropDown.SelectedID);
                            GlobalAlrt(this, "Can Not Find Player");
                            GlobalAlrt(this, "Current Log Filter : No." + FilterDropDown.SelectedID);
                            GlobalAlrt(this, "-------------------------------------------------------" + FilterDropDown.SelectedID);
                            break;
                        }
                        unit.receivedLog = CutLogs(unit.receivedLog, AllowStringCount);
                        logText.text = unit.receivedLog;
                        break;
                }
            }

            if (Networking.LocalPlayer == null) return;
#if !UNITY_EDITOR

        nameText.text = $"{Networking.LocalPlayer.displayName}.{Networking.LocalPlayer.playerId}";
        pingText.text = $"Local Ping : {Networking.GetServerTimeInMilliseconds() - DateTime.Now.Millisecond}ms";
        instOwnerText.text = $"InstanceOwner : {Networking.GetOwner(gameObject).displayName}.{Networking.GetOwner(gameObject).playerId}";
#endif
        }

        public void Log(UnityEngine.Object classObject, string msg)
        {
            if(NetworkUnit == null)
            {
                GlobalAlrt(classObject, msg);
                GlobalAlrt(this, "NetworkUnit is not ready.");
                return;
            }
            var className = ((UdonSharpBehaviour)classObject).GetUdonTypeName();
#if UNITY_EDITOR
            Debug.Log(msg, classObject);
            m_Log(className, msg);
            return;
#endif
            var NetworkTag = Networking.IsOwner(((UdonSharpBehaviour)classObject).gameObject) ? "(Owner)" : "";
            NetworkUnit.SendLog = $"<color={NetworkUnit.ColorCode}>[{NetworkTag}{Networking.LocalPlayer.displayName}.{Networking.LocalPlayer.playerId}]</color>" + m_Log(className, msg);
            NetworkUnit.TrySync();
        }

        public void LogWarn(UnityEngine.Object classObject, string msg)
        {
            if (NetworkUnit == null)
            {
                GlobalAlrt(classObject, msg);
                return;
            }
            var className = ((UdonSharpBehaviour)classObject).GetUdonTypeName();
#if UNITY_EDITOR
            Debug.Log(msg, classObject);
            m_LogWarn(className, msg);
            return;
#endif
            var NetworkTag = Networking.IsOwner(((UdonSharpBehaviour)classObject).gameObject) ? "(Owner)" : "";
            NetworkUnit.SendLog = $"<color={NetworkUnit.ColorCode}>[{NetworkTag}{Networking.LocalPlayer.displayName}.{Networking.LocalPlayer.playerId}]</color>" + m_LogWarn(className, msg);
            NetworkUnit.TrySync();
        }

        public void LogError(UnityEngine.Object classObject, string msg)
        {
            if (NetworkUnit == null)
            {
                GlobalAlrt(classObject, msg);
                GlobalAlrt(this, "NetworkUnit is not ready.");
                return;
            }
            var className = ((UdonSharpBehaviour)classObject).GetUdonTypeName();
#if UNITY_EDITOR
            Debug.Log(msg, classObject);
            m_LogError(className, msg);
            return;
#endif
            var NetworkTag = Networking.IsOwner(((UdonSharpBehaviour)classObject).gameObject) ? "(Owner)" : "";
            NetworkUnit.SendLog = 
                $"<color={NetworkUnit.ColorCode}>[{NetworkTag}{Networking.LocalPlayer.displayName}.{Networking.LocalPlayer.playerId}]</color>" 
                + $"<color=green>[{DateTime.Now.ToString("HH:mm:ss.ff")}]</color> | <color={errorColor}>[{className}] {msg}</color>\n";
            NetworkUnit.TrySync();
        }


        private string m_Log(string className, string msg)
        {
            msg = $"<color=green>[{DateTime.Now.ToString("HH:mm:ss.ff")}]</color> | <color={normalColor}>[{className}] {msg}</color>\n";

            return msg;
        }

        private string m_LogWarn(string className, string msg)
        {
            msg = $"<color=green>[{DateTime.Now.ToString("HH:mm:ss.ff")}]</color> | <color={warnColor}>[{className}] {msg}</color>\n";

            return msg;
        }

        private string m_LogError(string className, string msg)
        {
            msg = $"<color=green>[{DateTime.Now.ToString("HH:mm:ss.ff")}]</color> | <color={errorColor}>[{className}] {msg}</color>~\n";

            return msg;
        }

        public string m_NetworkLog(string msg)
        {
            AllLogs = AllLogs + msg;

            return msg;
        }

        public string GlobalAlrt(UnityEngine.Object classObject, string msg)
        {
            msg = $"<color=green>[{DateTime.Now.ToString("HH:mm:ss.ff")}]</color> | <color={errorColor}>[{((UdonSharpBehaviour)classObject).GetUdonTypeName()}] {msg}</color>\n";

            logText.text  = logText.text + msg;

            return msg;
        }

        public void Clear()
        {
            AllLogs = "";
            if (NetworkUnit != null)
            {
                NetworkUnit.ClearLog();
                foreach (var unit in OthersUnits)
                {
                    if (unit == null) return;
                    unit.ClearLog();
                }
            }
            Log(this, "Log All Clear");
        }

        public void ToggleBlock()
        {
            IsPause = !IsPause;
        }

        private void LateUpdate()
        {
            CountText.text = $"{logText.text.Length} / <size=7>{AllowStringCount}</size>";

            if(NetworkUnit != null && Networking.LocalPlayer != null)
            {
                if (NetworkUnit.OwnerID != Networking.LocalPlayer.playerId)
                {
                    NetworkUnit.Init();
                    NetworkUnit.SetOwner();
                }
                foreach (var item in FilterDropDown.Items)
                {
                    if (!item.Data) continue;
                    var player = VRCPlayerApi.GetPlayerById(item.Data.OwnerID);
                    if(player != null) item.Title = $"{player.displayName}.{player.playerId}";
                    else item.Title = "{LOST_TARGET}";
                }
            }
        }


        private void CheckPool()
        {
            if(NetworkUnit == null) // NetworkUnit을 할당받지 못한 클라가 있을때
            {
                Networking.SetOwner(Networking.LocalPlayer, UnitPool.gameObject);
                NetworkUnit = UnitPool.TryToSpawn().GetComponent<UserNetworkUnit>();
                NetworkUnit.Init();
                NetworkUnit.SetOwner();
                RequestSerialization();
                Log(this, "Try Set NetworkUnit for LocalPlayer");
            }

            if(Networking.IsOwner(Networking.LocalPlayer, NetworkUnit.gameObject))
            {
                NetworkUnit.SetOwner();
                RequestSerialization();
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            Log(this, $"{player.displayName}.{player.playerId} left the instance.");
            foreach (var unit in UnitPool.Pool)
            {
                var n_unit = unit.GetComponent<UserNetworkUnit>();
                if (VRCPlayerApi.GetPlayerById(n_unit.OwnerID) == null)
                {
                    ReturnUnit(n_unit);
                    return;
                }else if (player.playerId == n_unit.OwnerID)
                {
                    ReturnUnit(n_unit);
                    return;
                }
            }
            Log(this, $"Can not find left player {player.displayName}.{player.playerId}");
        }

        public void ReturnUnit(UserNetworkUnit n_unit)
        {
            Log(this, $"Return NetworkUnit by {Networking.LocalPlayer.displayName}.{Networking.LocalPlayer.playerId} | OwnerID: {n_unit.OwnerID}");
            n_unit.SetOwner();
            FilterDropDown.DeleteItembyData(n_unit); // 앞에 Local과 ALL이 있기떄문에 - 2 해줌
            UnitPool.Return(n_unit.gameObject);
            RequestSerialization();
        }

        public void ChangeFilter()
        {
            
            switch (FilterDropDown.SelectedID)
            {
                case 0:
                    GlobalAlrt(this, "Change Log Filter -> Local");
                    logText.text = NetworkUnit.receivedLog;
                    break;
                case 1:
                    GlobalAlrt(this, "Change Log Filter -> ALL");
                    logText.text = AllLogs;
                    break;
                default:
                    GlobalAlrt(this, "Change Log Filter -> Player");
                    var unit = FilterDropDown.GetDataByID(FilterDropDown.SelectedID);
                    if (unit == null) break;
                    logText.text = unit.receivedLog;
                    break;
            }
        }

        public string CutLogs(string logs, int count)
        {
            if (logs.Length >= count)
            {
                var sp = logs.Split('\n');

                var textCountbyLine = 0;
                foreach (var st in sp)
                {
                    textCountbyLine += st.Length;
                    if (logs.Length - count < textCountbyLine)
                    {
                        return logs.Substring(textCountbyLine);
                    }
                }
            }

            return logs;

            // 구 코드
            if (logs.Length >= count)
            {
                var sp = logs.Split('\n');

                var textCountbyLine = 0;
                foreach (var st in sp)
                {
                    textCountbyLine += st.Length;
                    if (logs.Length - count < textCountbyLine)
                    {
                        return logs.Substring(textCountbyLine);
                    }
                }
            }
            return "";
        }
    }
#else
    public class KDebug : MonoBehaviour
    {
        public bool PleaseImportUDON = false;
    }
#endif
}