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
    //대화 시작
    public void OnTalk(Vector3 pos)
    {
        dialogSystem.DialogStop(); // Dialog Board Off
        talkChannelSO.EndTalkRequested += EndTalk;
        transform.forward = pos - transform.position;

        //Quest가 있을경우 
        if (questGroupSO.IsActiveQuest())
        {
            //Cut 씬으로 표현되기 바랄 경우
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
        //평범한 대화로 할 경우
        else
        {
            TalkManager.Instance.OnTalk(talkGroupSO.GetDialogTalk());
        }

    }
    //대화 종료
    private void EndTalk()
    {
        talkChannelSO.EndTalkRequested -= EndTalk;
        StartCoroutine(SlowTurn());
    }
    // 캐릭터 초기 방향으로 회전
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


// Active가 된 Quest를 어떻게 가지고 있고 알고 전달하나
// 각 NPC 마다 Quest가 있으니까 NPC를 판별하는 ID가 필요하다 (있으면 유용하다)
// Player - (Talk) > NPC - ID Check - Player Active Quest List by ID - NPC have Quest M W N Check - Quest Play
// Player Acrice Quest가 어디에 저장 되어 있느냐 - QuestManger 에서 Game Start시 매번 동기화 퀘스트 진행 완료시 동기화
// NPC Quest가 어디에 저장 되어 있느냐 - 각 NPC가 QuestBookSO를 가져서 UnActive , Active  QuestList가 있으면 될듯
// 전체 QuestList Line을 담당하는 Script 또한 필요
// 단 Data가 중복 저장되어 있다는게 걸리는데....
// 중복 저장을 피하기 위해서 전체 Quest Data는 ID만 가지고 있고 Player Active Quest도 ID만 가지고 있으면 될거 같다.
// ㅇㅋ 약간 능동적으로 Quest가 있을수 있기때문에 나는 여러 클래스를 더 만들어 나눠야 하는게 맞는거 같다

// UOP1에서는
// QuestManager - QuestLines로 전체 Quest Sequence 가지고 있음
// QUestManager - ID로 QuestManager에 있는 QuestLine 접근 - > Step 접근 -> QuestStart
// QuestManager에서 모든 퀘스트 데이터 접근 찾기 진행 발생 한다.
// 모든 순서가 차례대로 나열되어 있기 때문에 그런듯