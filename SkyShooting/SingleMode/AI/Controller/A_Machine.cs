using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[DefaultExecutionOrder(100)]
public class A_Machine : MonoBehaviour
{
    enum State{Wait, Found, Follow, Attack, Avoid, Item};
    State state;
    public TargetScanner scanner;
    public NavMesh_RandomDestination destiation;
    private NavMeshAgent agent;

    [SerializeField]
    private float runSpeed; //이동속도
    [SerializeField]
    private float turnSpeed; //회전속도
    private float runPower; //추가 이동속도

    public bool check;
    private void Awake()
    {
        //이동설정
        runSpeed = 10f;
        runPower = 10f;
        turnSpeed = 2f;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
   
    }
    void Update()
    {
        if (check)
        {
            Vector3 randomPosition;
            if (destiation.RandomPoint(out randomPosition))
            {               
                agent.SetDestination(randomPosition);
                check = false;
            }
        }
        
        NavMeshPath dd = agent.path;
        Vector3[] ss = dd.corners;
        
        if(ss!=null && ss.Length != 0)
        {
            
            for(int i=0; i< ss.Length; i++)
            {
                //Rot2(ss[i]);
   
            }
            
        }
        
        if (agent.destination == null)
        {
            print("null");
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance<10f)
            {
                check = true;
            }
        }
        
        switch (state)
        {
            case State.Wait:
                break;
            case State.Found:
                break;
            case State.Follow:
                break;
            case State.Attack:
                break;
            case State.Avoid:
                break;
            case State.Item:
                break;
        }

        //Move();
        //Rot();   
    }


    #region  적 찾기
    //찾기
    private void Find()
    {
        Transform target = scanner.Detect(transform, TargetScanner.DetectState.Enemy);
        //회전
        if (target != null)
        {
            Vector3 direct = target.position - transform.position;
            if (direct != Vector3.zero)
            {
                if (check)
                {
                    float dot = Mathf.Atan2(direct.normalized.y, direct.normalized.x) * Mathf.Rad2Deg;
                    print(dot);
                }
                else
                {
                    float dot = Vector3.Dot(transform.forward, direct.normalized);
                    Vector3 cross = Vector3.Cross(transform.forward, direct.normalized);
                    dot = Mathf.Acos(dot) * Mathf.Rad2Deg;
                    if (cross.y > 0f)
                    {
                        print("오른쪽");
                    }
                    else if (cross.y < 0f)
                    {
                        print("왼쪽");
                        dot = -dot;
                    }
                    Quaternion quater = Quaternion.AngleAxis(dot, Vector3.up);
                    Quaternion look = Quaternion.LookRotation(direct.normalized, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, quater, Time.fixedDeltaTime * turnSpeed);
                }
            }
        }
    }
    private void Rot2(Vector3 point)
    {
        //회전
        if (point != null &&point!=Vector3.zero)
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

    //레이더 안에 들어온 타겟 바라보기
    private void Rot()
    {
        Transform target = scanner.Detect(transform, TargetScanner.DetectState.Enemy);
        //회전
        if (target != null)
        {
            Vector3 diret = target.position - transform.position;
            if (diret != Vector3.zero)
            {
                diret = diret.normalized;

                Quaternion diretion = Quaternion.LookRotation(diret, Vector3.up);
                transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion, Time.deltaTime * turnSpeed);
            }
        }
    }
    #endregion
    //이동
    private void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * runSpeed, Space.Self);
    }
    #region 돌아다니기

    private void Nav_AroudMove()
    {
        agent.SetDestination(Vector3.zero);

    }
    //랜덤 회전
        //transform.rotation = Quaternion.Lerp(this.transform.rotation, diretion * Quaternion.AngleAxis(-90f, Vector3.right), Time.deltaTime * turnSpeed);
    
    //장애물 피하기

    //경계선 가지 않기

    #endregion

    //레이더 Editor
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
            scanner.Editor_TargetScanner(this.transform);
    }
#endif
    
}
