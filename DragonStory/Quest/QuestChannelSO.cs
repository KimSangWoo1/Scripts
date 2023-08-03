using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "QuestChannelSO", menuName = "ScriptableObjects/QuestChannelSO", order = 1)]
public class QuestChannelSO : ScriptableObject
{
    //�⺻ ����Ʈ Event
    public UnityAction<QuestSO> ActiveQuestRequested;
    public UnityAction<QuestSO> StartQuestRequested;
    public UnityAction<QuestSO> EndQuestRequested;

    // ���� ����Ʈ
    public UnityAction<ItemBaseSO> EatQuestRequeted;
    public UnityAction<ItemBaseSO> GiveItemQuestRequeted;
    public UnityAction<ItemBaseSO> HaveItemQuestRequeted;
    public UnityAction<ItemBaseSO> QuestItemUseRequeted;

    public UnityAction<Actor> FightKillRequestd;

    #region Ordinary
    public void ActiveQuestEvent(QuestSO quest)
    {
        if (ActiveQuestRequested != null)
        {
            ActiveQuestRequested.Invoke(quest);
        }
        else
        {
            Debug.LogWarning("OnQuestRequested Null");
        }
    }

    public void StartQuestEvent(QuestSO quest)
    {
        if (StartQuestRequested != null)
        {
            StartQuestRequested.Invoke(quest);
        }
        else
        {
            Debug.LogWarning("StartQuestRequested Null");
        }
    }

    public void EndQuestEvent(QuestSO quest)
    {
        if (EndQuestRequested != null)
        {
            EndQuestRequested.Invoke(quest);
        }
        else
        {
            Debug.LogWarning("EndQuestRequested Null");
        }
    }
    #endregion

    #region Addtive 
    //���� �Դ� ����Ʈ
    public void EatQuestEvent(ItemBaseSO item)
    {
        if (EatQuestRequeted != null)
        {
            EatQuestRequeted.Invoke(item);
        }
        else
        {
            Debug.LogWarning(item.name +" : "+"EatQuestRequeted Null");
        }
    }
    // ������ �־�� �ϴ� ����Ʈ
    public void HaveItemEvent(ItemBaseSO item)
    {
        if (HaveItemQuestRequeted != null)
        {
            HaveItemQuestRequeted.Invoke(item);
        }
        else
        {
            Debug.LogWarning(item.name + " : " + "HaveItemQuestRequeted Null");
        }
    }
    // ������ ��� �ϴ� ����Ʈ
    public void GiveItemEvent(ItemBaseSO item)
    {
        if (GiveItemQuestRequeted != null)
        {
            GiveItemQuestRequeted.Invoke(item);
        }
        else
        {
            Debug.LogWarning(item.name + " : " + "GiveItemQuestRequeted Null");
        }
    }

    public void QuestItemUseEvent(ItemBaseSO item)
    {
        if (QuestItemUseRequeted != null)
        {
            QuestItemUseRequeted.Invoke(item);
        }
        else
        {
            Debug.LogWarning(item.name + " : " + "QuestItemUseRequeted Null");
        }
    }
    #endregion

    public void FightKillEvent(Actor actor)
    {
        if (FightKillRequestd != null)
        {
            FightKillRequestd.Invoke(actor);
        }
        else
        {
            Debug.LogWarning(actor + ": FightKillRequestd Null");
        }
    }
}



