
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace K13A.KDebug
{
#if UDON
    public class FontInput : UdonSharpBehaviour
    {
        public Text text;
        public Text placeholder;
        public InputField Input;
        public override void Interact()
        {
            placeholder.text = Input.text;
            text.fontSize = int.Parse(Input.text);
            Input.text = "";
        }
    }
#else
    public class FontInput : MonoBehaviour
    {
        public bool PleaseImportUDON = false;
    }
#endif
}