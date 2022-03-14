
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
namespace K13A.KDebug
{
#if UDON

    public class TestLogger : UdonSharpBehaviour
    {
        public KDebug Debug;

        public bool isNormal = true;
        public bool isWarnning = false;
        public bool isError = false;

        public string LogMassage = "Here...";

        public override void Interact()
        {
            if (isNormal)
            {
                Debug.Log(this, LogMassage);
            }
            if (isWarnning)
            {
                Debug.LogWarn(this, LogMassage);
            }
            if (isError)
            {
                Debug.LogError(this, LogMassage);
            }
        }

        private void Start()
        {
            Debug.Log(this, "Tester Ready.");
        }
    }

#else
    public class TestLogger : MonoBehaviour
    {
        public bool PleaseImportUDON = false;
    }
#endif
}
