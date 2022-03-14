
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace K13A.KDebug.UIElement.DropDown
{
    public class KDropDown : UdonSharpBehaviour
    {
        public bool isOpen = false;

        public int SelectedID = 0;

        public string TitleConents = "";
        public Text Title;

        public GameObject Tamplate;

        public KUIItem[] Items;
        public int ItemCount = 2; // 1 부터 시작함 (개수)

        public UdonSharpBehaviour EventBehaviour;
        public string EventMethod = "";

        private void Start()
        {
            UpdateItemSetList();

            isOpen = !isOpen; // 게임 시작할때 isOpen의 상태를 UI에 반영하는 코드
            Interact(); // "
        }

        public override void Interact()
        {
            isOpen = !isOpen;

            Tamplate.SetActive(isOpen);

            Title.text = isOpen ? TitleConents : Items[SelectedID].Title;
        }

        public void ChangeSelected(int id)
        {
            SelectedID = id;
            Interact();
            UpdateItemSetList();
            EventBehaviour.SendCustomEvent(EventMethod);
        }

        public void AddItem(string title, UserNetworkUnit data)
        {
            Items[ItemCount].Title = title;
            Items[ItemCount].Data = data;
            ItemCount++;

            UpdateItemSetList();
        }

        public void SetItems(UserNetworkUnit[] datas)
        {
            var i = 2;
            foreach (var Item in Items)
            {

                Item.Title = $"{VRCPlayerApi.GetPlayerById(datas[i].OwnerID)}{datas[i].OwnerID}";
                Item.Data = datas[i];
                i++;
            }
            ItemCount = i;
        }

        public void DeleteItembyData(UserNetworkUnit data)
        {
            for (var i = 0; i < Items.Length; i++)
            {
                if(Items[i].Data == data)
                {
                    var tmp = Items[i];
                    Items[i] = Items[ItemCount - 1];
                    Items[ItemCount - 1] = tmp;
                }
            }
            ItemCount--;

            UpdateItemSetList();
        }

        public void UpdateItemSetList()
        {
            for(var i = 0; i < Items.Length; i++)
            {
                Items[i].ItemID = i;
                Items[i].isChecked = (Items[i].ItemID == SelectedID);
                Items[i].gameObject.SetActive(i < ItemCount);
            }
        }

        public UserNetworkUnit GetDataByID(int index) => Items[index].Data;

        public bool IsExists(UserNetworkUnit data)
        {
            for (var i = 0; i < ItemCount; i++)
            {
                if (Items[i].Data == data) return true;
            }

            return false;
        }

        public void SetEvent(UdonSharpBehaviour target, string name)
        {
            EventBehaviour = target;
            EventMethod = name;
        }
    }
}