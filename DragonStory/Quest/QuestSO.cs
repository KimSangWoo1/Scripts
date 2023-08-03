using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "QuestSO", menuName = "ScriptableObjects/QuestSO", order = 1)]
public class QuestSO : SerializeSO
{
    public QuestBase questBase;

    [SerializeField]
    private ChainQuest[] chainQuests;

    [Tooltip("��� Quest Group")]
    [SerializeField]
    public QuestGroupSO questGroupSO;
    [SerializeField]
    protected QuestChannelSO questChannelSO;

    [SerializeField]
    protected CutSceneEvent cutSceneEvent;

    public virtual void ChannelRegister() { }

    #region Quest State Change
    public void QuestBeginning()
    {
        if (questBase.questState.questFirstStep == QuestStep.READY)
            ActiveQuest();
        if (questBase.questState.questFirstStep == QuestStep.START)
        {
            ActiveQuest();
            StartQuest();
        }
    }

    //����Ʈ �ʱ�ȭ
    public virtual void ResetQuest()
    {
        questBase.questState.questStep = QuestStep.READY;
        questBase.questState.active = false;
    }

    // ����Ʈ Ȱ��ȭ
    public void ActiveQuest()
    {
        questBase.questState.active = true;

        //Possible Quest  
        SetQuestGroup(); 

        questChannelSO.ActiveQuestEvent(this);
    }

    //�ش� NPC Quest Group�� �߰�
    public void SetQuestGroup()
    {
        if (questGroupSO != null)
        {
            Debug.Log(this.name+" Set Group Try");
            questGroupSO.questGroup.Add(questBase.questInfo.questType, this);
        }
    }

    // ����Ʈ ����
    public virtual void StartQuest()
    {
        questBase.questState.questStep = QuestStep.START;
        questChannelSO.StartQuestEvent(this);
    }

    // ����Ʈ ����
    public virtual void EndQuest()
    {
        questBase.questState.questStep = QuestStep.END;
        if (questGroupSO != null)
        {
            switch (questBase.questInfo.questType)
            {
                case QuestType.MAIN:
                    questGroupSO.questGroup.mainQuest.Remove(this);
                    break;
                case QuestType.WORLD:
                    questGroupSO.questGroup.worldQuest.Remove(this);
                    break;
                case QuestType.NORMAL:
                    questGroupSO.questGroup.normalQuest.Remove(this);
                    break;
            }
        }
        questChannelSO.EndQuestEvent(this); //QuestManager�� Start Quest ����� ���� Quest�� ���ֱ�.
        ChainQuestActive();
        ResetQuest();
    }
    #endregion

    #region ���¿� ���� �뺻 ���� ��ȯ

    public virtual DialogSO CheckSuccess()
    {
        switch (questBase.successType)
        {
            case SuccessType.TALK:
                EndQuest();
                return GetCompleteDialog();
        }
        return null;
    }

    public DialogSO GetStartDialog()
    {
        return questBase.startDialog;
    }

    public DialogSO GetUnCompleteDialog()
    {
        return questBase.unCompleteDialog;
    }
    public DialogSO GetCompleteDialog()
    {
        return questBase.completeDialog;
    }

    public QuestStep GetQuestStep()
    {
        return questBase.questState.questStep;
    }
    #endregion

    #region ����Ʈ�� ��ȣ�ۿ� �ϴ� �̺�Ʈ��
    public bool IsCutSceneQuest()
    {
        return cutSceneEvent.isCutScene;
    }

    public void CutSceneEvent()
    {
        cutSceneEvent.CutScene.Invoke(cutSceneEvent.chapter, cutSceneEvent.part);
        EndQuest();
    }

    //���� ����Ʈ Ȱ��ȭ
    private void ChainQuestActive()
    {
        if (chainQuests.Length != 0)
        {
            for (int i = 0; i < chainQuests.Length; i++)
            {
                chainQuests[i].quest.ResetQuest();
                chainQuests[i].quest.QuestBeginning();
            }
        }
    }
    #endregion
    //Locale Entry
    public string GetEntry()
    {
        StringTable table = LocalizationSettings.StringDatabase.GetTableAsync(questBase.questInfo.name.TableReference).Result;
        return table.SharedData.GetEntryFromReference(questBase.questInfo.name.TableEntryReference).Key;
    }
}

[System.Serializable]
public struct QuestInfo
{
    [Tooltip("Quest ����")]
    public QuestType questType;
    [Tooltip("Quest ID")]
    public QuestID questID;

    [Tooltip("Quest �� ����")]
    public LocalizedString name;
    public LocalizedString description;
}

[System.Serializable]
public struct QuestState
{
    [Tooltip("�� ����Ʈ�� ù ���� Step ����")]
    public QuestStep questFirstStep;
    [Tooltip("Quest State Step")]
    public QuestStep questStep;
    [Tooltip("Quest �ܰ� ����")]
    public bool active;
}

[System.Serializable]
public struct QuestBase
{
    public QuestInfo questInfo;
    public QuestState questState;

    [Header("Event Detail Info")]
    public DialogSO startDialog;
    public DialogSO unCompleteDialog;
    public DialogSO completeDialog;

    public SuccessType successType;
}

[System.Serializable]
public struct ChainQuest
{
    public QuestSO quest;
}

[System.Serializable]
public struct CutSceneEvent
{
    public bool isCutScene;
    public Chapter chapter;
    public Part part;
    public UnityEvent<Chapter, Part> CutScene;
}

public enum QuestStep
{
    READY,
    START,
    END
}

public enum SuccessType
{
    TALK,
    GIVEITEM,
    HAVEITEM,
    EAT,
    HAVEITEM_TALK,
    EAT_TALK,
    KILL
}

/*
 * Questtt List 
 * 1. World
 * 2. Normal
 * 3. Main
 * 
 * Player Quest List
 * - W : a, a-1, a-2, b, c, d, e, f, g
 * - N : a, b, c, d, e, f, g
 * - M : a, b, c, d, e, f, g
 * 
 *  requestQuest
 *  AfterQuest
 *  
 *  
 *    NPC <- QuestBook  - W - Q1, Q2, Q3
 *                      - N - Q1
 *   ����Ʈ Ȱ��ȭ!!
 *   
 */

