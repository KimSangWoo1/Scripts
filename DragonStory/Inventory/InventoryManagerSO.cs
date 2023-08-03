using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "InventoryManagerSO", menuName = "ScriptableObjects/InventoryManagerSO", order =1)]
public class InventoryManagerSO : ScriptableObject
{
    [SerializeField]
    private GameDataSO gameDataSO;
    [SerializeField]
    private ItemChannelSO itemChannelSO;
    [SerializeField]
    private AudioChannelSO audioChannelSO;
    [SerializeField]
    private QuestChannelSO questChannelSO;

    public List<ItemBase> itemList;
    public List<ItemBase> quickItemList;

    [System.Serializable]
    public class ItemBase
    {
        public ItemBaseSO item;
        public bool isQuick;
        public int quickNumber;
        public string guid;
        [HideInInspector]
        public string AddressableKey;
        public ItemBase(ItemBaseSO _item, bool _isQuick, int quickNum = -1)
        {
            item = _item;
            isQuick = _isQuick;
            quickNumber = quickNum;
            guid = Guid.NewGuid().ToString();
            AddressableKey = item.AddressableGuid;
        }
    }

    private void OnEnable()
    {
        itemChannelSO.OnStoreRequested += StoreItem;
        itemChannelSO.OnQuickRequested += QuickAddItem;
        itemChannelSO.OnSplitRequested += QuickItemSplit;
        itemChannelSO.OnGiveRequested += GiveItem;
        itemChannelSO.OnFireReadyRequested += FireReady;
    }

    private void OnDisable()
    {
        itemChannelSO.OnStoreRequested -= StoreItem;
        itemChannelSO.OnQuickRequested -= QuickAddItem;
        itemChannelSO.OnSplitRequested -= QuickItemSplit;
        itemChannelSO.OnGiveRequested -= GiveItem;
        itemChannelSO.OnFireReadyRequested -= FireReady;
    }

    #region Inventory
    // 아이템 보관
    private void StoreItem(ItemBaseSO item)
    {
        //새로운 아이템 추가
        if (itemList.Count < 12)
        {
            itemList.Add(new ItemBase(item,false));
            item.AddAmount();

            gameDataSO.SaveGameDataToDisk();// 저장
        }
    }

    public void LoadSaveItem(SaveData saveData)
    {
        itemList.Clear();
        for (int i=0; i<saveData.itemList.Count; i++)
        {
            itemList.Add(saveData.itemList[i]);
        }
        QuickItemListInit();
    }

    //아이템 주기
    private void GiveItem(ItemBaseSO item)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].item == item)
            {
                itemList[i].item.currentAmount--;
                itemList.Remove(itemList[i]);
            }
        }
        gameDataSO.SaveGameDataToDisk();// 저장

        //QuickSlot에 장착된 아이템이라면?
        QuickItemListInit();
        itemChannelSO.QuickSlotUpdate();
    }

    #endregion

    #region Quick Slot

    private void FireReady()
    {
        itemChannelSO.FireEvent(quickItemList[0].item.type);
        QuickItemSplit();
    }

    // 퀵슬롯 아이템 뱉기
    private void QuickItemSplit() 
    {
        if (itemList.Contains(quickItemList[0]))
        {
            quickItemList[0].item.AddUseCount(); //먹은 횟수 증가
            quickItemList[0].item.currentAmount--; //가방 갯수 감소
            questChannelSO.EatQuestEvent(quickItemList[0].item); //음식 먹는 Quest 있을 경우 Event
            questChannelSO.QuestItemUseEvent(quickItemList[0].item); //Item 가지고 있어야 하는 Quest의 Item 사용할 경우 Event
            itemList.Remove(quickItemList[0]);
            quickItemList.RemoveAt(0);

            gameDataSO.SaveGameDataToDisk(); //저장
        }
        itemChannelSO.QuickSlotUpdate();
    }

    //퀵슬롯 아이템 장착
    private void QuickAddItem(ItemBase itemBase)
    {
        // 선입 선출 
        if (quickItemList.Count >= 3)
        {
            //퀵 체크 해제
            for(int i=0; i< itemList.Count; i++)
            {
                if (itemList[i].guid.Equals(quickItemList[0].guid))
                {
                    itemList[i].isQuick = false;
                    itemList[i].quickNumber = -1;
                    break;
                }
            }
            quickItemList.RemoveAt(0);

        }
        itemBase.isQuick = true; //퀵 체크
        quickItemList.Add(itemBase); // 퀵 아이템 리스트에 추가

        itemChannelSO.QuickSlotUpdate(); //퀵 슬롯 업데이트
        audioChannelSO.UiSoundEvent(AudioManager.UiSoundType.SELECT);
        gameDataSO.SaveGameDataToDisk();//저장
    }

    //게임 시작시 Quick Slot 초기화 
    public void QuickItemListInit()
    {
        quickItemList.Clear();

        int number = 0;

        for(int i=0; i<3; i++)
        {
            for (int j = 0; j < itemList.Count; j++)
            {
                if (itemList[j].quickNumber == i)
                {
                    quickItemList.Add(itemList[j]);
                    if(itemList[j].quickNumber != number)
                    {
                        itemList[j].quickNumber = number;
                    }
                    number++;
                }
            }
        }
        
    }

    #endregion

    #region ETC
    //가방에 아이템 갯수
    public int GetItemAmout(ItemBaseSO item)
    {
        int amount=0;
        for(int i=0; i < itemList.Count; i++)
        {
            if(itemList[i].item == item)
            {
                amount++;
            }
        }
        return amount;
    }
    #endregion
}
