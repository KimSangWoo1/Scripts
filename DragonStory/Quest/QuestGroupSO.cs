using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestGroupSO", menuName = "ScriptableObjects/QuestGroupSO", order = 1)]
public class QuestGroupSO : SerializeSO
{
    public QuestGroup questGroup;

    private bool main;
    private bool world;
    private bool normal;

    //����Ʈ �� �� �ִ� ��� ã��
    public bool IsActiveQuest()
    {
        main = questGroup.mainQuest.Count == 0 ? false : true;
        world = questGroup.worldQuest.Count == 0 ? false : true;
        normal = questGroup.normalQuest.Count == 0 ? false : true;
        
        if (main || world || normal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //����Ʈ �� �� �ִ� �� �� ���� 1�� �����ϱ�
    public DialogSO GetQuestDialog()
    {
        if(main)
        {
            return QuestSequnce(questGroup.mainQuest[0], questGroup.mainQuest[0].GetQuestStep());
        }

        if (world)
        {
            return QuestSequnce(questGroup.worldQuest[0], questGroup.worldQuest[0].GetQuestStep());
        }

        if (normal)
        {
            return QuestSequnce(questGroup.normalQuest[0], questGroup.normalQuest[0].GetQuestStep());
        }
        return null;
    }
    //����Ʈ Step�� ���� �����ϱ�
    private DialogSO QuestSequnce(QuestSO quest, QuestStep step)
    {
        switch (step)
        {
            case QuestStep.READY:
                quest.StartQuest();
                return quest.GetStartDialog();
            case QuestStep.START:
                return quest.CheckSuccess();
            case QuestStep.END:
                break;
        }
        return null;
    }

    //����Ʈ �� �� Ȯ���ϱ�
    public bool IsCutSceneQuest()
    {
        if (main)
        {
            return questGroup.mainQuest[0].IsCutSceneQuest();
        }
        else
        {
            return false;
        }
    }

    //����Ʈ �� �� Event
    public void CutSceneEvent()
    {
        questGroup.mainQuest[0].CutSceneEvent();
    }
}

[System.Serializable]
public class QuestGroup
{
    public List<QuestSO> mainQuest;
    public List<QuestSO> worldQuest;
    public List<QuestSO> normalQuest;

    public void Init()
    {
        if (mainQuest != null)
        {
            mainQuest.Clear();
        }
        else
        {
            mainQuest = new List<QuestSO>();
        }

        if (worldQuest != null)
        {
            worldQuest.Clear();
        }
        else
        {
            worldQuest = new List<QuestSO>();
        }
        if (normalQuest != null)
        {
            normalQuest.Clear();
        }
        else
        {
            normalQuest = new List<QuestSO>();
        }
    }

    public void Add(QuestType type, QuestSO quset)
    {
        switch (type)
        {
            case QuestType.MAIN:
                if (!mainQuest.Exists(o => o.AddressableGuid == quset.AddressableGuid))
                {
                    mainQuest.Add(quset);
                }
                break;
            case QuestType.WORLD:
                if (!worldQuest.Exists(o => o.AddressableGuid == quset.AddressableGuid)) worldQuest.Add(quset);
                break;
            case QuestType.NORMAL:
                if (!normalQuest.Exists(o => o.AddressableGuid == quset.AddressableGuid)) normalQuest.Add(quset);
                break;
        }
    }
}

//������ �����
[System.Serializable]
public class QuestSerial
{
    public List<QuestSerialInfo> questList = new List<QuestSerialInfo>();

    public void Init()
    {
        if (questList != null)
        {
            questList.Clear();
        }
    }
}

[System.Serializable]
public struct QuestSerialInfo
{
    public string key;
    public string groupKey;
    public QuestSerialInfo(string _key, string _groupKey)
    {
        key = _key;
        groupKey = _groupKey;
    }
}