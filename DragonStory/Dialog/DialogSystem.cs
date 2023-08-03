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
    private float radius; //Player ã�� �ݰ� ����

    [Header("Dialog Interactive Time")]
    [SerializeField]
    private float waitTime; //���� �ð� (��ȭ���� ������)
    [SerializeField]
    private float autoActiveTime; //�ڵ� ������ �ð�
    [SerializeField]
    private float readTime; //���� �ð�
    private float currentTime;

    private bool currentTyping; //��ȭ �Է���
    private bool canSee; // Player ���̴���
    private bool timeUp; // �ð� üũ

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

            // 1. Player�� ���̰�     
            if (canSee)
            {
                // 2. ��ȭ���ڿ� ��ȭ �Է����� �ƴ� ���
                if (!currentTyping)
                {
                    //3. ���� �ð��� ���� ��ȭ�� �Ѵ�
                    if (TimeUp())
                    {
                        OnDialogTalk();
                        timeUp = false;
                    }
                }
            }
            else
            {
                //Player�� �ָ� ���� ����
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

    //�ð� ����
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

    //��ȭ���ڷ� ��ȭ�ϱ�
    private void OnDialogTalk()
    {
        StopAllCoroutines();
        changePerformance.ChageFace(dialogGroupSO.GetEmotion());
        changePerformance.Action(dialogGroupSO.GetAct());
        StartCoroutine(AutoComplete(dialogGroupSO.GetDialogText())); //��ȭâ ���
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
                soundControl.Sound(); //����
        }
        yield return StartCoroutine(AutoActive(readTime)); //���� �ð� �ְ� ��ȭâ ����
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
    //Player ã��
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
