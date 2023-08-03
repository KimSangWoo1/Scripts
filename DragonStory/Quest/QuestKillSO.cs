using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestKillSO", menuName = "ScriptableObjects/QuestKillSO", order =1)]
public class QuestKillSO : QuestSO
{
    [Header("Quest Kill")]
    [SerializeField]
    private FightChannelSO fightChannelSO;

    public RequireNPC[] requireNPCs;
    public override void ResetQuest()
    {
        base.ResetQuest();
        for (int i = 0; i < requireNPCs.Length; i++)
        {
            requireNPCs[i].currentAmount = 0;
        }
    }

    public override void ChannelRegister()
    {
        if (questBase.successType == SuccessType.KILL)
        {
            base.questChannelSO.FightKillRequestd += Kill;
        }
    }

    public override void StartQuest()
    {
        base.StartQuest();

        ChannelRegister();
        if (questBase.successType == SuccessType.GIVEITEM)
        {
            KillQuestSuccessCheck();
        }
    }

    public override void EndQuest()
    {
        base.EndQuest();
        if (questBase.successType == SuccessType.KILL)
        {
            base.questChannelSO.FightKillRequestd -= Kill;
        }
    }

    public override DialogSO CheckSuccess()
    {
        switch (questBase.successType)
        {
            case SuccessType.KILL:
                return GetUnCompleteDialog();
        }
        return null;
    }

    //Objser 
    private void Kill(Actor actor)
    {
        Debug.Log("Kill : " + actor);
        for(int i=0; i < requireNPCs.Length; i++)
        {
            if(requireNPCs[i].actor == actor)
            {
                requireNPCs[i].currentAmount++;
            }
        }
        KillQuestSuccessCheck();
    }

    private void KillQuestSuccessCheck()
    {
        int count = 0;

        for (int i = 0; i < requireNPCs.Length; i++)
        {
            if(requireNPCs[i].currentAmount == requireNPCs[i].requireAmount)
            {
                count++;
            }
        }

        if(count == requireNPCs.Length)
        {
            EndQuest();
        }
    }

    [System.Serializable]
    public struct RequireNPC
    {
        public Actor actor;
        public int requireAmount;
        public int currentAmount;
    }
}
