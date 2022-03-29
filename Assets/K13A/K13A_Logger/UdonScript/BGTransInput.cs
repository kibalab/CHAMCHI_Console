
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace CHAMCHI.UI
{
#if UDON
    public class BGTransInput : UdonSharpBehaviour
    {
        public Image[] images;
        public Text placeholder;
        public InputField Input;
        public override void Interact()
        {
            placeholder.text = Input.text;
            var color = new Color(0, 0, 0, int.Parse(Input.text) * 0.01f); ;
            foreach(var image in images)
            {
                image.color = color;
            }
            Input.text = "";
        }
    }
#else
    public class BGTransInput : MonoBehaviour
    {
        public bool PleaseImportUDON = false;
    }
#endif
}