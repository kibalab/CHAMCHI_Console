
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace K13A.KDebug.UIElement.DropDown
{
    public class KUIItem : UdonSharpBehaviour
    {

        public Text TitleText;
        public GameObject CkeckBox;

        public KDropDown Parents;

        public int ItemID = 0;

        private bool m_isCkecked = false;
        private UserNetworkUnit m_data;
        

        public bool isChecked
        {
            set
            {
                m_isCkecked = value;
                CkeckBox.SetActive(m_isCkecked);
            }
            get => m_isCkecked;
        }

        public UserNetworkUnit Data
        {
            set
            {
                m_data = value;
            }
            get => m_data;
        }

        public string Title
        {
            set
            {
                TitleText.text = value;
            }
            get => TitleText.text;
        }

        public override void Interact()
        {
            Parents.ChangeSelected(ItemID);
        }
    }
}