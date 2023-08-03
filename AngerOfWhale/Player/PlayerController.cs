using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : WhaleBase
{
    public enum State { IDLE, DIVE, ATTACK, DEAD, DAMAGE };
    public State state;

    // Manager
    SoundManager soundManager;

    [Header("Player Interaction Script")]
    // Componet
    public PlayerInteraction PlayerInteraction;
    public DiveBarControl diveBarControl;
    public HeartControl heartControl;
    public ShildControl shildControl;

    [Header("Player Interaction Object")]
    public Material skin; //고래 스킨
    public ParticleSystem[] effects;

    [HideInInspector]
    public Animator animator;

    [Header("Player Detail Setting")]
    // Variable
    public float timeSpeed = 29f; // 정지 시간 속도
    public float blinkSpeed; //색 변화 속도

    public float InvincibleTime = 1f; // 무적 시간
    private float blinkTime; //색 변화 시간
    private float slowTime; //느려지는 시간

    private float startPosY; // Player 초기 위치Y
    private float startPosZ; // Player 초기 위치Z
    [SerializeField]
    private int hp; // 체력
    private int stemina; // 구급상자 추가 HP
    private int helmat; // 헬멧 갯수

    private bool damage; //피격체크

    WaitForSeconds waitSeconds = new WaitForSeconds(0.1f);
    private void Awake()
    {
        animator = GetComponent<Animator>();
        soundManager = SoundManager.Instance;
    }
    private void Start()
    {
        hp = 3;
        stemina = 0;
        helmat = 0;
        slowTime = 0.1f;
        moveSpeed = 10f; // 이동 속도

        startPosY = transform.position.y;
        startPosZ = transform.position.z;

        Init();
    }
    private void Update()
    {
        switch (state)
        {
            case State.IDLE:
                Move(); //이동
                LimitMove(); //이동제한
                Dive(); //잠수 애니
                break;
            case State.DIVE:
                //Move(); //이동
                //LimitMove(); //이동제한
                AttackWay(); //공격 애니
                break;
            case State.ATTACK:
                if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    StartCoroutine(TimeDelay()); //시간 딜레이

                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
                    {
                        Init();
                    }
                }
                break;
            case State.DEAD:
                skin.color = Color.black;
                GameManager.endGame = true;
                break;
            case State.DAMAGE:
                Damage();
                if (damage)
                {
                    DamageBlink();
                }
                Move(); //이동
                LimitMove(); //이동제한 
                PlayerInteraction.Attack = false;
                PlayerInteraction.Dive = false;
                break;
        }
    }
    //초기화
    private void Init()
    {
        PlayerInteraction.Dive = false;
        PlayerInteraction.Attack = false;

        //animator.ResetTrigger("Dive");
        //animator.ResetTrigger("Attack");

        skin.color = Color.black;

        state = State.IDLE;
    }

    #region 고래 이동
    //Player 이동 
    private void Move()
    {
        if (PlayerInteraction.DirectPos != Vector3.zero)
        {
            Vector3 move = new Vector3(PlayerInteraction.DirectPos.x, startPosY, startPosZ);
            transform.position = Vector3.Slerp(transform.position, move, Time.smoothDeltaTime * moveSpeed);
        }
    }

    //Player가 카메라 화면밖으로 나가지 않도록
    private void LimitMove()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        if (pos.x < 0.1f)
        {
            pos.x = 0.1f;
            transform.position = Camera.main.ViewportToWorldPoint(pos);
        }
        if (pos.x > 0.9f)
        {
            pos.x = 0.9f;
            transform.position = Camera.main.ViewportToWorldPoint(pos);
        }
    }
    #endregion

    #region Animaor Control
    //잠수
    private void Dive()
    {
        if (PlayerInteraction.Dive)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Dive");
            diveBarControl.OnDiveBar(); // 다이브바 UI On
            soundManager.UnderWaterSound();
            state = State.DIVE;
        }
    }

    //공격
    private void Attack()
    {
        animator.ResetTrigger("Dive");
        animator.SetTrigger("Attack");
        diveBarControl.OffDiveBar(); // 다이브바 UI Off
        soundManager.AttackSound();
        AttackEffect();
        state = State.ATTACK;
    }

    private void Damage()
    {
        animator.SetTrigger("Damage");
    }
    #endregion

    #region 아이템 효과

    // HP 회복
    void HpRecover()
    {
        hp++;
        hp = Mathf.Clamp(hp, 0, 3);
        hp += stemina;
        heartControl.ChangeHP(hp);
    }
    // 스테미나 추가 (하트 추가)
    void AddStemina()
    {
        if (stemina != 2)
        {
            stemina++;
            stemina = Mathf.Clamp(stemina, 0, 2);
            heartControl.AddHeart();
        }
    }
    #endregion

    #region 피격시
    public void OnDamage()
    {
        soundManager.DamageSound(); //피격 사운드
        diveBarControl.OffDiveBar(); // 잠수중일경우 다이브바 UI Off
        if (helmat == 0)
        {
            if (stemina != 0)
            {
                SteminaControl(); //스테미나 깎임
            }
            else
            {
                HpControl(); //체력 깎임 or 죽음
            }
        }
        else
        {
            HelmatBroken(); //핼맷 파괴됨
        }
    }
    private void SteminaControl()
    {
        stemina--;
        heartControl.RemoveHeart(); //추가 체력 없애기

        damage = true;
        state = State.DAMAGE;
    }
    private void HpControl()
    {
        hp--;
        heartControl.ChangeHP(hp); //UI 하트 변경

        //죽음
        if (hp <= 0)
        {
            soundManager.OnBGM(2);
            state = State.DEAD;
        }
        else
        {
            damage = true;
            state = State.DAMAGE;
        }
    }
    private void HelmatBroken()
    {
        helmat--; //방어막 깎임
        helmat = Mathf.Clamp(helmat, 0, 2);
        if (helmat == 0)
        {
            shildControl.OnBroken(false); //방어막 부서짐 + 사라짐
        }
        else
        {
            shildControl.OnBroken(true); //방어막 부서짐 
        }
    }
    //피격 받았을 때 고래 색깔 빨간색으로
    private void DamageBlink()
    {
        blinkTime += Time.unscaledDeltaTime;
        if (blinkTime <= InvincibleTime)
        {
            skin.color = new Color(Mathf.PingPong(Time.unscaledTime * blinkSpeed, 1f), 0f, 0f);
        }
        else
        {
            blinkTime = 0f;
            damage = false;
            Init(); //초기화
        }
    }
    #endregion

    #region 공격시
    //공격방법 (1. 기본 클릭 공격, 2. 강제공격)
    private void AttackWay()
    {
        if (PlayerInteraction.Attack) //클릭 공격
        {
            Attack();
        }
        if (!diveBarControl.Diving) //강제 공격
        {
            Attack();
        }
    }
    IEnumerator TimeDelay()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            slowTime = Mathf.Lerp(slowTime, 1f, Time.unscaledDeltaTime * timeSpeed);
            Time.timeScale = slowTime;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        slowTime = 0.1f;
    }

    #endregion

    #region 이팩트
    public void AidKitEffect(int num)
    {
        soundManager.AidKitSound();

        //체력 우선 회복
        if (hp < 3)
        {
            //체력 회복
            effects[0].gameObject.SetActive(true);
            HpRecover();
        }
        else
        {
            if (num == 2)
            {
                //하트 추가
                effects[1].gameObject.SetActive(true);
                AddStemina();
            }
        }

    }
    //보호막 이팩트
    public void HelmatEffect(int count)
    {
        shildControl.gameObject.SetActive(true);
        soundManager.HelmatSound();
        if (helmat == 2)
        {
            return;
        }
        else
        {
            helmat = count;
        }
    }

    //무적 이팩트?
    public void AttackEffect()
    {
        if (effects[2].gameObject.activeSelf)
        {
            effects[2].gameObject.SetActive(false);
        }
        effects[2].transform.position = transform.position + new Vector3(0f, 0f, 1f);
        effects[2].gameObject.SetActive(true);
    }
    #endregion
}