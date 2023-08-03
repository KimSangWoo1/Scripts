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
    // ������ ����
    private void StoreItem(ItemBaseSO item)
    {
        //���ο� ������ �߰�
        if (itemList.Count < 12)
        {
            itemList.Add(new ItemBase(item,false));
            item.AddAmount();

            gameDataSO.SaveGameDataToDisk();// ����
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

    //������ �ֱ�
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
        gameDataSO.SaveGameDataToDisk();// ����

        //QuickSlot�� ������ �������̶��?
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

    // ������ ������ ���
    private void QuickItemSplit() 
    {
        if (itemList.Contains(quickItemList[0]))
        {
            quickItemList[0].item.AddUseCount(); //���� Ƚ�� ����
            quickItemList[0].item.currentAmount--; //���� ���� ����
            questChannelSO.EatQuestEvent(quickItemList[0].item); //���� �Դ� Quest ���� ��� Event
            questChannelSO.QuestItemUseEvent(quickItemList[0].item); //Item ������ �־�� �ϴ� Quest�� Item ����� ��� Event
            itemList.Remove(quickItemList[0]);
            quickItemList.RemoveAt(0);

            gameDataSO.SaveGameDataToDisk(); //����
        }
        itemChannelSO.QuickSlotUpdate();
    }

    //������ ������ ����
    private void QuickAddItem(ItemBase itemBase)
    {
        // ���� ���� 
        if (quickItemList.Count >= 3)
        {
            //�� üũ ����
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
        itemBase.isQuick = true; //�� üũ
        quickItemList.Add(itemBase); // �� ������ ����Ʈ�� �߰�

        itemChannelSO.QuickSlotUpdate(); //�� ���� ������Ʈ
        audioChannelSO.UiSoundEvent(AudioManager.UiSoundType.SELECT);
        gameDataSO.SaveGameDataToDisk();//����
    }

    //���� ���۽� Quick Slot �ʱ�ȭ 
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
    //���濡 ������ ����
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
