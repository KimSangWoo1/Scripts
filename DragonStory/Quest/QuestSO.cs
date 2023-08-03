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

    [Tooltip("담당 Quest Group")]
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

    //퀘스트 초기화
    public virtual void ResetQuest()
    {
        questBase.questState.questStep = QuestStep.READY;
        questBase.questState.active = false;
    }

    // 퀘스트 활성화
    public void ActiveQuest()
    {
        questBase.questState.active = true;

        //Possible Quest  
        SetQuestGroup(); 

        questChannelSO.ActiveQuestEvent(this);
    }

    //해당 NPC Quest Group에 추가
    public void SetQuestGroup()
    {
        if (questGroupSO != null)
        {
            Debug.Log(this.name+" Set Group Try");
            questGroupSO.questGroup.Add(questBase.questInfo.questType, this);
        }
    }

    // 퀘스트 시작
    public virtual void StartQuest()
    {
        questBase.questState.questStep = QuestStep.START;
        questChannelSO.StartQuestEvent(this);
    }

    // 퀘스트 종료
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
        questChannelSO.EndQuestEvent(this); //QuestManager에 Start Quest 목록의 현재 Quest를 없애기.
        ChainQuestActive();
        ResetQuest();
    }
    #endregion

    #region 상태에 따른 대본 내용 반환

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

    #region 퀘스트에 상호작용 하는 이벤트들
    public bool IsCutSceneQuest()
    {
        return cutSceneEvent.isCutScene;
    }

    public void CutSceneEvent()
    {
        cutSceneEvent.CutScene.Invoke(cutSceneEvent.chapter, cutSceneEvent.part);
        EndQuest();
    }

    //연계 퀘스트 활성화
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
    [Tooltip("Quest 종류")]
    public QuestType questType;
    [Tooltip("Quest ID")]
    public QuestID questID;

    [Tooltip("Quest 상세 설명")]
    public LocalizedString name;
    public LocalizedString description;
}

[System.Serializable]
public struct QuestState
{
    [Tooltip("이 퀘스트에 첫 시작 Step 설정")]
    public QuestStep questFirstStep;
    [Tooltip("Quest State Step")]
    public QuestStep questStep;
    [Tooltip("Quest 단계 설정")]
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
 *   퀘스트 활성화!!
 *   
 */

