using UnityEngine;
using UnityEngine.AI;
using Message;
using Mapmesh;

[DefaultExecutionOrder(100)]
public class Nav_Machine : PlaneBase, IMessageReceiver
{
    enum State { Wait, Found, Attack, Avoid, Item, Dead }; //비행기 상태 : (대기, 찾음, 공격, 도망, 아이템, 죽음)
    enum WaitState { GetPosition, GoPosition }; //비행기 대기 상태 : (랜덤좌표 얻기, 랜덤좌표 이동)
    enum AvoidState { GetDirection, RunAway, Return, Emergency }//비행기 도망 상태 : (방향 얻기 ,도망가기, 다시 싸우러가기, 위급 상황)
    State state;
    WaitState waitState;
    AvoidState avoidState;

    public TargetScanner scanner; //스캐너
    public Map_V2 map; // 맵
    public MuzzleController muzzle; //총구
    public FireController fireController; //발사

    private Transform target; //적군

    private Vector3 random_Position; // Map  랜덤 좌표
    private Vector3 fightPosition; //싸웠던 위치
    private Vector3 avoidPosition; //도망칠 방향
    private float closeDistance; // Map 랜덤 좌표 접근 인정 거리
    private float avoidTime; //도망치는 시간

    private int randomNumber; //랜덤 번호
    // AI 형에 따른 설정 가능
    [SerializeField]
    private float avoidWaitTime; //도망가는 시간 설정 기본 2초  //시간이 : 짧을수록 공격형 - 밸런스형 - 방어형  시간이 길수록
    [SerializeField]
    private float busterWaitTime; //부스터
                                  // 스피드 

    //test
    public bool check;
    private Bounds bound;
    Vector3 left;
    Vector3 right;
    private NavMeshAgent agent;
    private void OnEnable()
    {
        base.OnEnable();
        //상태 초기화
        state = State.Wait;
        waitState = WaitState.GetPosition;
        avoidState = AvoidState.GetDirection;
        fightPosition = Vector3.zero;
        avoidPosition = Vector3.zero;
    }

    void Start()
    {
        closeDistance = 10f;
        avoidWaitTime = 2f;

        //Test
        agent = GetComponent<NavMeshAgent>();
        bound = GetComponent<Renderer>().bounds;
        right = bound.extents;
        left = new Vector3(-right.x, right.y, right.z);

    }
    void Update()
    {
        if (hp <= 0)
        {
            //죽음
            state = State.Dead;
            //초기화
            waitState = WaitState.GetPosition;
            avoidState = AvoidState.GetDirection;
        }

        //Move(); //이동
        //Machine_State(); //상태 관리
        if (check)
        {
            Test();
            check = false;
        }
        else
        {
            Test2();
        }
    }

    private void Test()
    {
        Coord coord = map.GetRandomCoord();  //랜덤 좌표 받기
        random_Position = new Vector3(coord.x, transform.position.y, coord.y);
        
    }
    private void Test2()
    {
        agent.SetDestination(random_Position);
        Vector3 direct = random_Position - transform.position ;
        float distance = direct.sqrMagnitude;
        float stopDistance = agent.stoppingDistance * agent.stoppingDistance;

        if (distance < stopDistance)
        {
            check = true;
        }
        else
        {
            check = false;
        }
        NavMeshPath path = agent.path;
        Vector3[] paths = new Vector3[20];
        int count = path.GetCornersNonAlloc(paths);
        for(int i = 0; i<count; i++)
        {
            Debug.DrawRay(paths[i], Vector3.up, Color.blue, 5f);
        }
    }
    //이동
    private void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * runSpeed, Space.Self);
    }
    #region AI 상태 설정
    private void Machine_State()
    {
        //자폭 위험 방지
        EmerGencyPrevention();
        switch (state)
        {
            case State.Wait:
                target = scanner.Detect(transform, TargetScanner.DetectState.Enemy);
                if (target != null)
                {
                    state = State.Found; //발견
                }
                else
                {
                    WaitAction(); //미발견
                }
                break;
            case State.Found:
                Rot(); //타겟 방향 회전
                CheckObstacle(); //장애물이 앞에 있을 경우
                // 쏘면 맞는지 확인
                if (target != null)
                {
                    if (scanner.AttackDetect(transform, target))
                    {
                        state = State.Attack;
                    }
                }
                break;
            case State.Attack:
                AttackAction();
                break;
            case State.Avoid:
                AvoidAction();
                break;
            case State.Item:
                break;
            case State.Dead:
                // 파괴 연출

                //삭제
                this.gameObject.SetActive(false);
                break;
        }
    }

    private void EmerGencyPrevention()
    {
        if(state==State.Found || state == State.Attack || avoidState == AvoidState.RunAway)
        {
            if(target != null)
            {
                Vector3 _target = target.position - transform.position;
                float targetDistance = _target.sqrMagnitude;
                float safeDistance = closeDistance * closeDistance *5f ;
                //거리 점검
                if (targetDistance <= safeDistance )
                {
                    avoidState = AvoidState.GetDirection;
                    state = State.Avoid;
                }
            }
        }
    }

    #endregion
    #region Wait State (돌아다니기)
    private void WaitAction()
    {
        switch (waitState)
        {
            case WaitState.GetPosition:
                Coord  coord = map.GetRandomCoord();  //랜덤 좌표 받기
                random_Position = new Vector3(coord.x, transform.position.y, coord.y);
                //Debug.DrawRay(random_Position, Vector3.up, Color.red, 5f);
                waitState = WaitState.GoPosition; //상태 변경
                break;
            case WaitState.GoPosition:
                if (random_Position != Vector3.zero || random_Position != null)
                {
                    Check_DistancePosition(random_Position); //거리 체크하기
                }
                else
                {
                    waitState = WaitState.GetPosition; //상 태변경
                }
                break;
        }
    }
    //새로운 랜덤 좌표 받아오기
    private void Check_DistancePosition(Vector3 random_Position)
    {
        float distance = Vector3.Distance(transform.position, random_Position);
        if (distance < closeDistance)
        {
            waitState = WaitState.GetPosition; //어느정도 거리 접근하면 상태변경
        }
        else
        {
            Random_Rot(random_Position); // 좌표로 방향 회전
        }
    }
    // 새로운 랜덤 좌표 방향으로 돌기
    private void Random_Rot(Vector3 point)
    {
        //회전
        if (point != null && point != Vector3.zero)
        {
            Vector3 diret = point - transform.position;
            if (diret != Vector3.zero)
            {
                diret = diret.normalized;
                Quaternion diretion = Quaternion.LookRotation(diret, Vector3.up);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion, Time.deltaTime * turnSpeed);
            }
        }
    }
    #endregion

    #region Found State (적방향 회전 & 적 쫒기)
    //레이더 안에 들어온 타겟 방향 설정
    private void Rot()
    {
        target = scanner.Detect(transform, TargetScanner.DetectState.Enemy);
        //회전
        if (target != null)
        {
            Vector3 diret = target.position - transform.position;
            if (diret != Vector3.zero)
            {
                diret = diret.normalized;

                Quaternion diretion = Quaternion.LookRotation(diret, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, diretion, Time.deltaTime * turnSpeed * 2f);
            }
        }
        else
        {  //놓칠 경우 상태 변경
            state = State.Wait;
        }
    }
    //앞에 장애물 있을 경우 놓친걸로 판단
    private void CheckObstacle()
    {
        if (Physics.Raycast(transform.position, transform.forward, scanner.radius, scanner.blockLayerMask, QueryTriggerInteraction.Collide))
        {
            state = State.Wait;
        }
    }
    #endregion

    #region Attack State (적 쏘기)
    private void AttackAction()
    {
        //쏠 수 있는 탄알이 있는지
        if (fireController.IsRemainMagazine())
        {
            fireController.AI_FireTrigger();
        }
        //없으면 도망
        else
        {
            fightPosition = transform.position; //싸웠던 위치 기억
            state = State.Avoid;
        }
    }
    #endregion

    #region Avoid State (도망가기)

    private void AvoidAction()
    {
        target = scanner.Detect(transform, TargetScanner.DetectState.Enemy);

        switch (avoidState)
        {
            //0. 도망칠 방향 정하기
            case AvoidState.GetDirection:
                randomNumber = Random.Range(1, 3);
//avoidPosition = map.Spot_RendomPosition(transform);
                avoidState = AvoidState.RunAway;
                break;
            //1. 도망 
            case AvoidState.RunAway:
                RandomAway(avoidPosition);
                break;
            //2. 도망 갔다가 싸운 지점 복귀
            case AvoidState.Return:
                //복귀 시 타겟 발견하면 쫒아감
                if (target != null )
                {
                    state = State.Found;
                }
                // 싸웠던 장소 복귀
                else
                {
                    Return_FightPosition(fightPosition);
                }
                break;
        }
    }
    //1. 도망 (설정 : 도망 칠 거리, 도망칠 방향 범위, 장애물 검사, Map 경계 검사)
    private void RandomAway(Vector3 _avoidPosition)
    {
        //가까우면 복귀
        if (Vector3.Distance(transform.position, _avoidPosition) < closeDistance)
        {
            avoidTime = 0f;
            avoidState = AvoidState.Return;
        }
        else
        {
            //정해진 시간동안 도망감
            if (avoidTime <= avoidWaitTime)
            {
                avoidTime += Time.deltaTime;
                Vector3 diret = _avoidPosition - transform.position;
                diret = diret.normalized;
                //회전
                Quaternion diretion = Quaternion.LookRotation(diret, Vector3.up);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion, Time.deltaTime * turnSpeed);
            }
            else
            {
                // 탄창이 없으면 도망가도록
                if (fireController.Get_BulletCount() == 0)
                {
                    avoidTime = 0f;
                }
                else
                {
                    avoidTime = 0f;
                    avoidState = AvoidState.Return;
                }
            }
        }
    }

    //2. 도망 갔다가 싸운 지점 복귀
    private void Return_FightPosition(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        //3. 복귀시 타겟 없으면 Wait 상태로
        if (distance < closeDistance)
        {
            state = State.Wait;
            avoidState = AvoidState.GetDirection;// 초기화
        }
        // 싸운 지점 복귀중
        else
        {
            Vector3 diret = position - transform.position;
            diret = diret.normalized;
            //회전
            Quaternion diretion = Quaternion.LookRotation(diret, Vector3.up);
            transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion, Time.deltaTime * turnSpeed);
        }
    }
    #endregion

    public void OnReceiver_InteractMessage(MessageType type, object msg)
    {
        Interaction.InteractMessage message = (Interaction.InteractMessage)msg;

        switch (type)
        {
            case MessageType.HEALTH:
                hp += message.amount;
                HPCheck();
                break;
            case MessageType.BULLET:
                if (message.upgrade)
                {
                    muzzle.Add_Bullet();
                }
                break;
            case MessageType.MUZZLE:
                if (message.upgrade)
                {
                    muzzle.Add_Muzzle();
                }
                break;
            case MessageType.TURBIN:
                runSpeed += message.amount;
                break;
        }
    }

    //메시지 받기2
    public void OnReceiver_DamageMessage(MessageType type, object msg)
    {
        Interaction.DamageMessage message = (Interaction.DamageMessage)msg;
        switch (type)
        {
            case MessageType.DAMAGE:
                //HP 변경
                hp -= message.damage;
                HPCheck(); //HP Check
                base.hitFx.Play(); // Hp FX

                //점수 보드 변경
                if (hp <= 0f)
                {
                    UI_BM.Add_Score(message.name, 100); // 죽인 Player에게 100점
                }
                else
                {
                    UI_BM.Add_Score(message.name, 10); // 맞춘 Player에게 10점
                }

                //상태변경
                avoidState = AvoidState.Emergency;
                state = State.Avoid;
                break;
            case MessageType.CLASH:
                hp -= message.damage;
                HPCheck();
                state = State.Dead;
                break;
        }
    }

    //레이더 Editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        scanner.Editor_TargetScanner(this.transform);
        scanner.Editor_AttackScanner(this.transform);
    }
#endif
}
