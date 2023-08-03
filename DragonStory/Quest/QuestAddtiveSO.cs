using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestAddtiveSO", menuName = "ScriptableObjects/QuestAddtiveSO", order = 1)]
public class QuestAddtiveSO : QuestSO
{
    [Header("Quest Addtive")]
    [SerializeField]
    private InventoryManagerSO inventoryManagerSO;
    [SerializeField]
    private ItemChannelSO itemChannelSO;

    [Header("Quest Item")]
    public RequireItem[] requireItems;
    public RewardItem[] rewardItems;
    // RewardType : 아이템, 능력치, 미션성공

    [SerializeField]
    private bool success;

    private void OnDisable()
    {
        if (questBase.successType == SuccessType.EAT)
        {
            //TestChaneelSO.Instance.EatQuestRequeted -= QuestItemAdd;
            base.questChannelSO.EatQuestRequeted -= QuestItemAdd;
        }
        else if (questBase.successType == SuccessType.GIVEITEM)
        {
            base.questChannelSO.HaveItemQuestRequeted -= QuestItemAdd;
            base.questChannelSO.QuestItemUseRequeted -= QuestItemUse;
        }
        else if (questBase.successType == SuccessType.HAVEITEM)
        {
            base.questChannelSO.HaveItemQuestRequeted -= QuestItemAdd;
        }
        else if (questBase.successType == SuccessType.HAVEITEM_TALK)
        {
            base.questChannelSO.HaveItemQuestRequeted -= QuestItemAdd;
            base.questChannelSO.QuestItemUseRequeted -= QuestItemUse;
        }
    }

    public override void ResetQuest()
    {
        base.ResetQuest();
        for (int i = 0; i < requireItems.Length; i++)
        {
            requireItems[i].currentAmount = 0;
        }

        success = false;
    }

    public override void ChannelRegister()
    {
        if (questBase.successType == SuccessType.EAT)
        {
            //TestChaneelSO.Instance.EatQuestRequeted += QuestItemAdd;
            questChannelSO.EatQuestRequeted += QuestItemAdd;
        }
        else if (questBase.successType == SuccessType.GIVEITEM)
        {
            base.questChannelSO.HaveItemQuestRequeted += QuestItemAdd;
            base.questChannelSO.QuestItemUseRequeted += QuestItemUse;
        }
        else if (questBase.successType == SuccessType.HAVEITEM)
        {
            base.questChannelSO.HaveItemQuestRequeted += QuestItemAdd;
        }
        else if (questBase.successType == SuccessType.HAVEITEM_TALK)
        {
            base.questChannelSO.HaveItemQuestRequeted += QuestItemAdd;
            base.questChannelSO.QuestItemUseRequeted += QuestItemUse;
        }
    }

    public override void StartQuest()
    {
        base.StartQuest();

        ChannelRegister();
        if (questBase.successType == SuccessType.GIVEITEM)
        {
            QuestItemInvetoryCheck();
            QuestSuccessCheck(); 
        }
        else if (questBase.successType == SuccessType.HAVEITEM)
        {
            //이미 아이템 있을 경우
            QuestItemInvetoryCheck(); 
            QuestSuccessCheck(); 
            if (success)
            {
                EndQuest();
            }
        }
        else if(questBase.successType == SuccessType.HAVEITEM_TALK)
        {
            QuestItemInvetoryCheck();
            QuestSuccessCheck();
        }
    }

    public override void EndQuest()
    {
        base.EndQuest();
        if (questBase.successType == SuccessType.EAT)
        {
            //TestChaneelSO.Instance.EatQuestRequeted -= QuestItemAdd;
            base.questChannelSO.EatQuestRequeted -= QuestItemAdd;
        }
        else if (questBase.successType == SuccessType.GIVEITEM)
        {
            base.questChannelSO.HaveItemQuestRequeted -= QuestItemAdd;
            base.questChannelSO.QuestItemUseRequeted -= QuestItemUse; 
        }
        else if (questBase.successType == SuccessType.HAVEITEM)
        {
            base.questChannelSO.HaveItemQuestRequeted -= QuestItemAdd;
        }
        else if (questBase.successType == SuccessType.HAVEITEM_TALK)
        {
            base.questChannelSO.HaveItemQuestRequeted -= QuestItemAdd;
            base.questChannelSO.QuestItemUseRequeted -= QuestItemUse; 
        }
    }

    #region QuestChannel 구독 이벤트
    //퀘스트에 필요한 음식을 새로 먹었을 경우 +  새로 인벤토리에 아이템이 추가 됐을 경우
    private void QuestItemAdd(ItemBaseSO item)
    {
        for (int i = 0; i < requireItems.Length; i++)
        {
            if (requireItems[i].Item.type == item.type) 
            {
                requireItems[i].currentAmount++;
                Debug.Log("맞아요" +i);
            }
            else
            {
                Debug.Log(requireItems[i].Item.AddressableGuid + " : " + item.AddressableGuid);
                Debug.Log("달라요");
            }
        }
        //퀘스트 목표 실시간 달성 확인
        QuestSuccessCheck();

        //Eat Have Quest는 상호작용 없이 바로 퀘스트 완료
        if (questBase.successType == SuccessType.HAVEITEM || questBase.successType == SuccessType.EAT)
        {
            if (success)
            {
                EndQuest();
            }
        }
    }

    //퀘스트 Item 사용할 경우
    private void QuestItemUse(ItemBaseSO item)
    {
        for (int i = 0; i < requireItems.Length; i++)
        {
            if (requireItems[i].Item == item) 
            {
                requireItems[i].currentAmount--;
            }
        }
        //Quest Item 사용해서 Require 갯수 못 채우는지 확인
        QuestSuccessCheck();
    }

    //NPC랑 Talk 시도 할 경우 Invetory Item Check
    private void QuestGiveItem() {

        for(int i=0; i<requireItems.Length; i++)
        {
            for(int j=0; j< requireItems[i].requiredAmount; j++)
            {
                itemChannelSO.GiveEvent(requireItems[i].Item);
            }
        }
    }

    // Quest 필요 아이템 갯수 체크
    private void QuestSuccessCheck()
    {
        int successCount = 0;
        for (int i = 0; i < requireItems.Length; i++)
        {
            if(requireItems[i].currentAmount >= requireItems[i].requiredAmount)
            {
                successCount++;
            }
        }

        if(successCount == requireItems.Length)
        {
            success = true;
        }
        else
        {
            success = false;
        }
    }

    private void QuestItemInvetoryCheck()
    {
        //인벤토리에 퀘스트 아이템 있는지 체크
        for (int i = 0; i < requireItems.Length; i++)
        {
            requireItems[i].currentAmount = inventoryManagerSO.GetItemAmout(requireItems[i].Item);
        }
    }

    public override DialogSO CheckSuccess()
    {
        switch (questBase.successType)
        {
            case SuccessType.GIVEITEM:
                //아이템 체크
                if (success)
                {
                    EndQuest();
                    QuestGiveItem(); 
                    return GetCompleteDialog();
                }
                else
                {
                    return GetUnCompleteDialog();
                }
                //아이템 있으면 Inventroy Item Remove + Quick Slot Update
            case SuccessType.HAVEITEM_TALK:
                //아이템 체크
                if (success)
                {
                    EndQuest();
                    return GetCompleteDialog();
                }
                else
                {
                    return GetUnCompleteDialog();
                }
        }
        return null;
    }

    [System.Serializable]
    public struct RequireItem
    {
        public ItemBaseSO Item; //퀘스트 성공에 필요한 Item
        public int requiredAmount; //퀘스트 성공에 필요한 Item 갯수
        public int currentAmount; // 퀘스트 성공에 필요한 Item 현재 갯수;
    }

    [System.Serializable]
    public struct RewardItem{
        public bool reward;//퀘스트 보상 여부
        public ItemBaseSO rewardItem;  //퀘스트 성공 후 보상 Item 
        public int Amount; //퀘스트 성공 후 보상 Item 갯수
        public int currentAmount; // 퀘스트 성공에 필요한 Item 현재 갯수;
    }
    #endregion
}
