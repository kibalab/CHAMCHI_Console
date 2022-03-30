
using CHAMCHI.Behaviour;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace CHAMCHI.Test
{
    public class printtestlog : UdonSharpBehaviour
    {
        public string testlog;
        public LogPanel logPanel;
        public PoolManager pool;
        public override void Interact()
        {

            switch (int.Parse(this.gameObject.name))
            {
                case 0:
                    logPanel.Log(this, "참치이");
                    logPanel.Log(this, "This is CHAMCHI!!");
                    logPanel.Log(this, testlog + '1');
                    logPanel.Log(this, testlog + '2');
                    logPanel.Log(this, testlog + '3');
                    logPanel.Log(this, testlog + '4');
                    logPanel.Log(this, testlog + '5');
                    break;
                case 1:
                    logPanel.LogWarn(this, testlog);
                    break;
                case 2:
                    logPanel.LogError(this, testlog);
                    break;
                case 3:
                    pool.getStatus();
                    break;
            }
        }
    }
}