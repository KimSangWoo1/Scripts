using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "QusetManagerSO", menuName = "ScriptableObjects/QusetManagerSO", order =1)]
public class QusetManagerSO : ScriptableObject
{
    [SerializeField]
    private GameDataSO gameDataSO;

    public QuestLine[] questLines;

    [SerializeField]
    private QuestChannelSO questChannelSO;

    public QuestGroup activeQuest; //퀘스트 시작 할 수 있는 그룹
    public QuestGroup startQuest; //퀘스트 시작한 그룹

    [SerializeField]
    public List<QuestGroupSO> npcQuestGroup;

    private void OnEnable()
    {
        //QuestRefreshing();// 
        questChannelSO.ActiveQuestRequested += OnQuestActive;
        questChannelSO.StartQuestRequested += OnQuestStart;
        questChannelSO.EndQuestRequested += OnQuestEnd;
    }

    private void OnDisable()
    {
        questChannelSO.ActiveQuestRequested -= OnQuestActive;
        questChannelSO.StartQuestRequested -= OnQuestStart;
        questChannelSO.EndQuestRequested -= OnQuestEnd;
    }

    // Main Story End -> Current Story Quest Beginning
    public void OnStoryQuestBeginning(int chapter, int part)
    {
        for (int i = 0; i < questLines[chapter].partLines[part].questGroup.mainQuest.Count; i++)
        {
            //if (!questLines[chapter].partLines[part].questGroup.mainQuest[i].questBase.questState.active)
            {
                questLines[chapter].partLines[part].questGroup.mainQuest[i].ResetQuest();
                questLines[chapter].partLines[part].questGroup.mainQuest[i].QuestBeginning();
            }
        }

        for (int i = 0; i < questLines[chapter].partLines[part].questGroup.worldQuest.Count; i++)
        {
            //if (!questLines[chapter].partLines[part].questGroup.worldQuest[i].questBase.questState.active)
            {
                questLines[chapter].partLines[part].questGroup.worldQuest[i].ResetQuest();
                questLines[chapter].partLines[part].questGroup.worldQuest[i].QuestBeginning();
            }
        }

        for (int i = 0; i < questLines[chapter].partLines[part].questGroup.normalQuest.Count; i++)
        {
            //if (!questLines[chapter].partLines[part].questGroup.normalQuest[i].questBase.questState.active)
            {
                questLines[chapter].partLines[part].questGroup.normalQuest[i].ResetQuest();
                questLines[chapter].partLines[part].questGroup.normalQuest[i].QuestBeginning();
            }
        }
    }

    private void OnQuestActive(QuestSO quest)
    {
        if (QuestType.MAIN == quest.questBase.questInfo.questType)
        {
            //startQuest.mainQuest.Remove(quest);
            activeQuest.mainQuest.Add(quest);
        }
        else if (QuestType.WORLD == quest.questBase.questInfo.questType)
        {
            //startQuest.worldQuest.Remove(quest);
            activeQuest.worldQuest.Add(quest);
        }
        else
        {
            //startQuest.normalQuest.Remove(quest);
            activeQuest.normalQuest.Add(quest);
        }

        gameDataSO.SaveGameDataToDisk();
    }

    private void OnQuestStart(QuestSO quest)
    {
        if (QuestType.MAIN == quest.questBase.questInfo.questType)
        {
            if (activeQuest.mainQuest.Exists(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID))
            {
                int index = activeQuest.mainQuest.FindIndex(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID);
                activeQuest.mainQuest.RemoveAt(index);
                startQuest.mainQuest.Add(quest);
            }
        }
        else if (QuestType.WORLD == quest.questBase.questInfo.questType)
        {
            if (activeQuest.worldQuest.Exists(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID))
            {
                int index = activeQuest.worldQuest.FindIndex(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID);
                activeQuest.worldQuest.RemoveAt(index);
                startQuest.worldQuest.Add(quest);
            }
        }
        else
        {
            if (activeQuest.normalQuest.Exists(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID))
            {
                int index = activeQuest.normalQuest.FindIndex(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID);
                activeQuest.normalQuest.RemoveAt(index);
                startQuest.normalQuest.Add(quest);
            }
        }

        gameDataSO.SaveGameDataToDisk();
    }

    private void OnQuestEnd(QuestSO quest)
    {
        if (QuestType.MAIN == quest.questBase.questInfo.questType)
        {
            if (startQuest.mainQuest.Exists(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID))
            {
                int index = startQuest.mainQuest.FindIndex(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID);
                startQuest.mainQuest.RemoveAt(index);
            }
        }
        else if (QuestType.WORLD == quest.questBase.questInfo.questType)
        {
            if (startQuest.worldQuest.Exists(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID))
            {
                int index = startQuest.worldQuest.FindIndex(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID);
                startQuest.worldQuest.RemoveAt(index);
            }
        }
        else
        {
            if (startQuest.normalQuest.Exists(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID))
            {
                int index = startQuest.normalQuest.FindIndex(q => q.questBase.questInfo.questID == quest.questBase.questInfo.questID);
                startQuest.normalQuest.RemoveAt(index);
            }
        }

        gameDataSO.SaveGameDataToDisk();
    }

    // Quest Observer 재등록 (Scene 이동시 Quest Observer 등록 취소되기 때문에)
    public void QuestRegister() 
    {
        for(int i=0; i< startQuest.mainQuest.Count; i++)
        {
            startQuest.mainQuest[i].ChannelRegister();
        }
        for (int i=0; i < startQuest.worldQuest.Count; i++)
        {
            startQuest.worldQuest[i].ChannelRegister();
        }
        for (int i=0; i < startQuest.normalQuest.Count; i++)
        {
            startQuest.normalQuest[i].ChannelRegister();
        }
    }

    public void NpcQuestGroupInit()
    {
        for(int i=0; i<npcQuestGroup.Count; i++)
        {
            npcQuestGroup[i].questGroup.Init();
        }
    }

    [System.Serializable]
    public struct QuestLine
    {
        public Chapter Chapter;
        public StoryLine[] partLines;
    }

    [System.Serializable]
    public struct StoryLine
    {
        public Part part;
        public QuestGroup questGroup;
    }
}
