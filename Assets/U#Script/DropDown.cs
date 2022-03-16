
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

    public class DropDown : UdonSharpBehaviour
    {
        public bool isOpen = false;

        public int SelectedID = 0;

        public string TitleConents = "";
        public Text Title;

        public GameObject Tamplate;

        public DropDownItem[] Items;

        public int ItemCount = 2; // 1 부터 시작함 (개수)
        public LogPanel logPanel;

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

        public void ChangeSelected(int DropDownIndex, int poolIndex)
        {
            SelectedID = DropDownIndex;
            Interact();
            UpdateItemSetList();
            logPanel.OnDropDownChanged(poolIndex);
        }

        public void AddItem(string title, object data)
        {
            Items[ItemCount].Title = title;
            Items[ItemCount].Data = data;
            ItemCount++;

            UpdateItemSetList();
        }

        public void RefreshItem(int[] idArr){


            for(int i = 0; i < idArr.Length; i++){
                if(idArr[i] == 0){
                    continue;
                }else if(VRCPlayerApi.GetPlayerById(idArr[i]) == Networking.LocalPlayer){
                    continue;
                }else{
                    Items[ItemCount].Title = VRCPlayerApi.GetPlayerById(idArr[i]).displayName;
                    Items[ItemCount].Data = i;
                    ItemCount++;
                }
            }          


            UpdateItemSetList();  
        }

        public void DeleteItembyData(object data)
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

        public object GetDataByID(int index) => Items[index].Data;

        public bool IsExists(object data)
        {
            for (var i = 0; i < ItemCount; i++)
            {
                if (Items[i].Data == data) return true;
            }

            return false;
        }
    }