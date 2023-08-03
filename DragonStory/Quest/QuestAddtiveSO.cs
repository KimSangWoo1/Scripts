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
    // RewardType : ������, �ɷ�ġ, �̼Ǽ���

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
            //�̹� ������ ���� ���
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

    #region QuestChannel ���� �̺�Ʈ
    //����Ʈ�� �ʿ��� ������ ���� �Ծ��� ��� +  ���� �κ��丮�� �������� �߰� ���� ���
    private void QuestItemAdd(ItemBaseSO item)
    {
        for (int i = 0; i < requireItems.Length; i++)
        {
            if (requireItems[i].Item.type == item.type) 
            {
                requireItems[i].currentAmount++;
                Debug.Log("�¾ƿ�" +i);
            }
            else
            {
                Debug.Log(requireItems[i].Item.AddressableGuid + " : " + item.AddressableGuid);
                Debug.Log("�޶��");
            }
        }
        //����Ʈ ��ǥ �ǽð� �޼� Ȯ��
        QuestSuccessCheck();

        //Eat Have Quest�� ��ȣ�ۿ� ���� �ٷ� ����Ʈ �Ϸ�
        if (questBase.successType == SuccessType.HAVEITEM || questBase.successType == SuccessType.EAT)
        {
            if (success)
            {
                EndQuest();
            }
        }
    }

    //����Ʈ Item ����� ���
    private void QuestItemUse(ItemBaseSO item)
    {
        for (int i = 0; i < requireItems.Length; i++)
        {
            if (requireItems[i].Item == item) 
            {
                requireItems[i].currentAmount--;
            }
        }
        //Quest Item ����ؼ� Require ���� �� ä����� Ȯ��
        QuestSuccessCheck();
    }

    //NPC�� Talk �õ� �� ��� Invetory Item Check
    private void QuestGiveItem() {

        for(int i=0; i<requireItems.Length; i++)
        {
            for(int j=0; j< requireItems[i].requiredAmount; j++)
            {
                itemChannelSO.GiveEvent(requireItems[i].Item);
            }
        }
    }

    // Quest �ʿ� ������ ���� üũ
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
        //�κ��丮�� ����Ʈ ������ �ִ��� üũ
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
                //������ üũ
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
                //������ ������ Inventroy Item Remove + Quick Slot Update
            case SuccessType.HAVEITEM_TALK:
                //������ üũ
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
        public ItemBaseSO Item; //����Ʈ ������ �ʿ��� Item
        public int requiredAmount; //����Ʈ ������ �ʿ��� Item ����
        public int currentAmount; // ����Ʈ ������ �ʿ��� Item ���� ����;
    }

    [System.Serializable]
    public struct RewardItem{
        public bool reward;//����Ʈ ���� ����
        public ItemBaseSO rewardItem;  //����Ʈ ���� �� ���� Item 
        public int Amount; //����Ʈ ���� �� ���� Item ����
        public int currentAmount; // ����Ʈ ������ �ʿ��� Item ���� ����;
    }
    #endregion
}
