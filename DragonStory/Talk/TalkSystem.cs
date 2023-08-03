using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkSystem : MonoBehaviour
{
    public TalkChannelSO talkChannelSO;
    public QuestChannelSO questChannelSO;
    public DialogGroupSO talkGroupSO;
    public QuestGroupSO questGroupSO;

    //public QuestLine questLine;
    [SerializeField]
    private DialogSystem dialogSystem;

    private Quaternion startRot;

    private void Start()
    {
        startRot = transform.rotation;    
    }
    //��ȭ ����
    public void OnTalk(Vector3 pos)
    {
        dialogSystem.DialogStop(); // Dialog Board Off
        talkChannelSO.EndTalkRequested += EndTalk;
        transform.forward = pos - transform.position;

        //Quest�� ������� 
        if (questGroupSO.IsActiveQuest())
        {
            //Cut ������ ǥ���Ǳ� �ٶ� ���
            if (questGroupSO.IsCutSceneQuest())
            {
                questGroupSO.CutSceneEvent();
            }
            // NPC have Quest M W N Check - Quest Play
            else
            {
                TalkManager.Instance.OnTalk(questGroupSO.GetQuestDialog().GetDialogTalk());
            }
        }
        //����� ��ȭ�� �� ���
        else
        {
            TalkManager.Instance.OnTalk(talkGroupSO.GetDialogTalk());
        }

    }
    //��ȭ ����
    private void EndTalk()
    {
        talkChannelSO.EndTalkRequested -= EndTalk;
        StartCoroutine(SlowTurn());
    }
    // ĳ���� �ʱ� �������� ȸ��
    IEnumerator SlowTurn()
    {
        float dot=0;
        while(0.99>dot)
        {
            dot = Quaternion.Dot(transform.rotation, startRot);
            dot = Mathf.Abs(dot);
            transform.rotation = Quaternion.Lerp(transform.rotation, startRot, Time.deltaTime * 4f);
            yield return null;
        }
    }
}


// Active�� �� Quest�� ��� ������ �ְ� �˰� �����ϳ�
// �� NPC ���� Quest�� �����ϱ� NPC�� �Ǻ��ϴ� ID�� �ʿ��ϴ� (������ �����ϴ�)
// Player - (Talk) > NPC - ID Check - Player Active Quest List by ID - NPC have Quest M W N Check - Quest Play
// Player Acrice Quest�� ��� ���� �Ǿ� �ִ��� - QuestManger ���� Game Start�� �Ź� ����ȭ ����Ʈ ���� �Ϸ�� ����ȭ
// NPC Quest�� ��� ���� �Ǿ� �ִ��� - �� NPC�� QuestBookSO�� ������ UnActive , Active  QuestList�� ������ �ɵ�
// ��ü QuestList Line�� ����ϴ� Script ���� �ʿ�
// �� Data�� �ߺ� ����Ǿ� �ִٴ°� �ɸ��µ�....
// �ߺ� ������ ���ϱ� ���ؼ� ��ü Quest Data�� ID�� ������ �ְ� Player Active Quest�� ID�� ������ ������ �ɰ� ����.
// ���� �ణ �ɵ������� Quest�� ������ �ֱ⶧���� ���� ���� Ŭ������ �� ����� ������ �ϴ°� �´°� ����

// UOP1������
// QuestManager - QuestLines�� ��ü Quest Sequence ������ ����
// QUestManager - ID�� QuestManager�� �ִ� QuestLine ���� - > Step ���� -> QuestStart
// QuestManager���� ��� ����Ʈ ������ ���� ã�� ���� �߻� �Ѵ�.
// ��� ������ ���ʴ�� �����Ǿ� �ֱ� ������ �׷���