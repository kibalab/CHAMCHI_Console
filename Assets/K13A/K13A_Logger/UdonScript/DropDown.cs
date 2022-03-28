
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

        public void ChangeSelected(int DropDownIndex)
        {
            SelectedID = DropDownIndex;
            Interact();
            UpdateItemSetList();
            logPanel.OnDropDownChanged();
        }

        public int getCurrentItemData(){
            return (int)Items[SelectedID].Data;
            
        }
        public void RefreshItem(int[] idArr){
            ItemCount = 2;

            for(int i = 0; i < idArr.Length; i++){
                /* player not exist */
                if(idArr[i] == 0){
                    continue;
                    /* local player */
                }else if(VRCPlayerApi.GetPlayerById(idArr[i]) == Networking.LocalPlayer){
                    /* remote player */
                }else{
                    Items[ItemCount].Title = VRCPlayerApi.GetPlayerById(idArr[i]).displayName+'.'+idArr[i].ToString();
                    Items[ItemCount].Data = i;
                    ItemCount++;
                }
            }          


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