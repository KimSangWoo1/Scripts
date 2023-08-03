using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    //Dialog SO Data
    public DialogGroupSO dialogGroupSO;

    public ChangePerformance changePerformance;
    
    [Header("UGUI")]
    public GameObject dialogBoard;
    public TextMeshProUGUI textPro;

    // Audio
    [SerializeField]
    private SoundControl soundControl;

    [Header("Player Sensor")]
    [SerializeField]
    private LayerMask playerLayer; 
    [SerializeField]
    private float radius; //Player 찾을 반경 범위

    [Header("Dialog Interactive Time")]
    [SerializeField]
    private float waitTime; //반응 시간 (대화상자 켜지는)
    [SerializeField]
    private float autoActiveTime; //자동 꺼지는 시간
    [SerializeField]
    private float readTime; //읽을 시간
    private float currentTime;

    private bool currentTyping; //대화 입력중
    private bool canSee; // Player 보이는지
    private bool timeUp; // 시간 체크

    private WaitForSeconds wait;
    private const float delay = 0.05f;

    void OnEnable()
    {
        currentTyping = false;
        timeUp = false;
    }

    void Start()
    {
        wait = new WaitForSeconds(delay);
    }

    private void LateUpdate()
    {
        if (GameManager.mode == Mode.NONE)
        {
            ScanClosePlayer();

            // 1. Player가 보이고     
            if (canSee)
            {
                // 2. 대화상자에 대화 입력중이 아닐 경우
                if (!currentTyping)
                {
                    //3. 랜덤 시간에 따라 대화를 한다
                    if (TimeUp())
                    {
                        OnDialogTalk();
                        timeUp = false;
                    }
                }
            }
            else
            {
                //Player가 멀리 가면 끄기
                if (dialogBoard.activeSelf)
                {
                    StartCoroutine(AutoActive(autoActiveTime));
                }
            }
        }
    }
    
    public void DialogStop()
    {
        StopAllCoroutines();
        currentTime = 0f;
        dialogBoard.SetActive(false);
    }

    //시간 제한
    private bool TimeUp()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= waitTime)
        {
            currentTime = 0f;
            timeUp = true;
        }
        return timeUp;
    }

    //대화상자로 대화하기
    private void OnDialogTalk()
    {
        StopAllCoroutines();
        changePerformance.ChageFace(dialogGroupSO.GetEmotion());
        changePerformance.Action(dialogGroupSO.GetAct());
        StartCoroutine(AutoComplete(dialogGroupSO.GetDialogText())); //대화창 출력
    }

    #region Auto Dialog Coroutine
    private IEnumerator AutoComplete(string content)
    {
        dialogBoard.SetActive(true);
        currentTyping = true;
        textPro.text = "";

        for (int i = 0; i < content.Length; i++)
        {
            yield return wait;
            textPro.text += content[i];
            if (i % 2 == 0)
                soundControl.Sound(); //음성
        }
        yield return StartCoroutine(AutoActive(readTime)); //읽을 시간 주고 대화창 끄기
        currentTyping = false;
    }

    public IEnumerator AutoActive(float time)
    {
        yield return new WaitForSeconds(time);
        changePerformance.init();
        dialogBoard.SetActive(false);
        timeUp = false;
        currentTyping = false;
        textPro.text = "";
        
    }
    #endregion

    #region Player Scan + Gizmo
    //Player 찾기
    private void ScanClosePlayer()
    {
        if (Physics.CheckSphere(transform.position, radius, playerLayer, QueryTriggerInteraction.Collide))
        {
            canSee = true;
        }
        else
        {
            canSee = false;
        }
    }

#if UNITY_EDITOR
    private void DetectionRange()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
        Gizmos.DrawSphere(transform.position, radius);
    }
    private void OnDrawGizmosSelected()
    {
        DetectionRange();
    }
#endif
    #endregion
}
