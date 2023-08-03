using UnityEngine;
using UnityEngine.UI;
using Message;
[DefaultExecutionOrder(100)]
public class PlaneController : PlaneBase ,IMessageReceiver
{
    enum PlayerState {Ghost, Play};
    private PlayerState playerSate;

    [Header("발사")]
    public FireController fireController; //발사 시스템
    [Header("총구 설정")]
    public MuzzleController muzzleController; //총구
    public BusterController busterController; //부스터

    [Header("UI")]
    public JoyStick joystick; //조이스틱
    public Health health;
    public HitBlinking hitBlinking;
    public ResultBoardControl resultBoardControl;
    public Projector projector;
    //입력값
    private float h;
    private float v;

    private void OnEnable()
    {
        base.OnEnable();
        playerSate = PlayerState.Ghost; //첫 시작시 Player 상태 설정
    }

    void Start()
    {
        base.Start();
        //다른 Player Projector 안보이도록
        projector.gameObject.SetActive(false);
    }

    void Update()
    {
        switch (playerSate)
        {
            case PlayerState.Ghost:
                GhostAction();//3초 무적상태(유령상태)
                break;
            case PlayerState.Play:
                fireController.Player_FireTrigger(); //발사 Possible
                break;
        }

        PlayerDeadCheck(); // Player 죽음 체크
        busterController.PlayerBusterControl(); //부스터 관리
        runPower = Mathf.Clamp(runPower, 10, 30); //부스터 최소 최대 속도 설정

        Move(); //비행기 이동
        Rot(); //비행기 회전 
    }

    //3초 무적상태(유령상태)
    protected override void GhostAction()
    {
        base.GhostAction();
        if (!ghostMode)
        {
            playerSate = PlayerState.Play; //상태변경
        }
    }
    // Player 죽음 체크
    private void PlayerDeadCheck() {
        hp = Mathf.Clamp(hp, 0, 100);

        if (hp <= 0f)
        {
            base.FXM.FX_Pop(transform, deadState); // 파괴 연출
            IM.ItemRandom(transform); //아이템 생성

            profile.UpdateScore(UI_BM.GetScore(profile.name));//결과 점수 가져오기

            resultBoardControl.SetResultBoard(profile); //결과 내용 전송


            UI_BM.ResetScore(profile.name);//플레이 점수 보드 변경
            PlaneDead();//삭제 - PM.Push()
        }

    }
    #region Player 이동 회전 부스터
    //비행기 이동 & 부스터
    private void Move()
    {
#if UNITY_ANDROID
        //Mobile 부스터
        if (busterController.Get_BusterClick())
        {
            transform.Translate(Vector3.forward * Time.deltaTime * (runSpeed + runPower), Space.Self);
            engineFX.gameObject.SetActive(false);
            if (!busterFx.isPlaying)
            {
                busterFx.Play();
            }
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * runSpeed, Space.Self);
            engineFX.gameObject.SetActive(true);
            busterFx.Pause();
        }
#endif

  #if UNITY_EDITOR_WIN
 //#if UNITY_STANDALONE_WIN
        //PC 부스터
        if (Input.GetKeyDown(KeyCode.Space))
        {
            busterController.buster = true;
            if (!busterFx.isPlaying)
            {
                busterFx.Pause();
                busterFx.Play(); 
            }
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            engineFX.gameObject.SetActive(false);
            if (!busterController.buster)
            {
                busterFx.Pause();
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed, Space.Self);
            }
            else
            {
                busterFx.Play();
                transform.Translate(Vector3.forward * Time.deltaTime * (runSpeed + runPower), Space.Self);
            }

        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            engineFX.gameObject.SetActive(true);
            busterFx.Pause();
            busterController.buster = false;
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * runSpeed, Space.Self);
        }        
#endif
    }
    //비행기 회전
    private void Rot()
    {

#if UNITY_EDITOR_WIN
//#if UNITY_STANDALONE_WIN
        //PC용
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            //입력
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            //회전
            Vector3 diret = new Vector3(h, 0f, v);
            if (diret != Vector3.zero)
            {
                diret = diret.normalized;

                Quaternion diretion = Quaternion.LookRotation(diret, Vector3.up);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion , Time.deltaTime * turnSpeed);
            }
        }
#endif
#if UNITY_ANDROID
        //Mobile 용
        if (joystick.move)
        {
            Vector2 joyDirect = joystick.getDirection();
            Vector3 direct = new Vector3(joyDirect.x, 0f, joyDirect.y);
            direct = direct.normalized;

            Quaternion diretion = Quaternion.LookRotation(direct, Vector3.up);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion, Time.deltaTime * turnSpeed);
            /* 곧바로 회전
            float angle = Mathf.Atan2(joyDirect.x, joyDirect.y) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0f, angle, 0f) ;
            */
        }
#endif
    }
#endregion

#region Message System
    //아이템 메시지 받기
    public void OnReceiver_InteractMessage(MessageType type, object msg)
    {
        Interaction.InteractMessage message = (Interaction.InteractMessage)msg;

        switch (type)
        {
            case MessageType.HEALTH:
                hp += message.amount;
                HPCheck();
                health.ChangeHP(hp);
                break;
            case MessageType.DOLLAR:
                profile.UpdateDollar(message.amount);
                break;
            case MessageType.BULLET:
                if (message.upgrade)
                {
                    muzzleController.Add_Bullet();
                }
                break;
            case MessageType.MUZZLE:
                if (message.upgrade)
                {
                    muzzleController.Add_Muzzle();
                }
                break;
            case MessageType.TURBIN:
                if (message.upgrade)
                {
                    runSpeed += message.amount;
                }
                break;
        }

    }
    //점수 메시지 받기
    public void OnReceiver_DamageMessage(MessageType type, object msg)
    {
        Interaction.DamageMessage message = (Interaction.DamageMessage)msg;
        switch (type)
        {
            case MessageType.DAMAGE:
                hp -= message.damage;
                HPCheck(); //hp 체크
                base.hitFx.GetComponent<FX_SoundControl>().Play(); //타격 FX 

                //점수 보드 변경
                if (hp <= 0f)
                {
                    UI_BM.Add_Score(message.name, 100); // 죽인 Player에게 100점
                }
                else
                {
                    UI_BM.Add_Score(message.name, 10); // 맞춘 Player에게 10점
                }

                hitBlinking.Blinking(true); //UI 빨간색 깜박임
                health.ChangeHP(hp); //UI hp 변경
                break;
            case MessageType.CLASH:
                hp -= message.damage;
                HPCheck();//hp 체크

                hitBlinking.Blinking(true); //UI 빨간색 깜박임
                health.ChangeHP(hp); //UI hp 변경
                break;
        }
    }
#endregion
}
