using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class TargetScanner
{
    public enum DetectState {Enemy, Item};
    DetectState detectState;
    [Header("거리 및 반경")]
    public float radius;
    [Range(0f,360f)]
    public float angle;
    [Header("레이더 검출 최대 갯수")]
    public int maxColliders;
    [Header("색 조정")]
    public Color red;
    public Color blue;
    public Color pupple;
    [Header("Layer설정")]
    public LayerMask blockLayerMask; //장애물 레이어
    public LayerMask targetLayerMask; //목표물 레이어
    public LayerMask ItemLyaerMask;
    [SerializeField]
    private bool canSee;
    [SerializeField]
    private int count; //범위 안에 있는 타겟 갯수

    private Collider[] hitColliders;
    private int numColliders;
    //적 탐색
    public Transform Detect(Transform transform, DetectState state)
    {
        //받아들일 콜라이더 갯수 설정
        hitColliders = new Collider[maxColliders];
        if(state == DetectState.Enemy)
        {
            //Player Layer된 콜라이더 갯수만 가져오기
            numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, targetLayerMask, QueryTriggerInteraction.Collide);
        }
        else
        {
            // Item Layer만
            numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, ItemLyaerMask, QueryTriggerInteraction.Collide);
        }
        count = 0; //발견 갯수

        if (numColliders != 0)
        {
            float shortDistance = radius * radius;
            int index = maxColliders+1; //넘길 번호
          
            for (int i = 0; i < numColliders; i++)
            {
                Vector3 _target = (hitColliders[i].transform.position + Vector3.up) - transform.position;
                _target.y = 0;
                //angle안에 있을 경우
                if (Vector3.Dot(_target.normalized, transform.forward) > Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
                {
                    canSee = false;
                    //장애물에 가릴 경우
                    canSee |= !Physics.Raycast(transform.position, _target.normalized, radius, blockLayerMask, QueryTriggerInteraction.Ignore);

                    if (canSee)
                    {
                        count++;
                        Debug.DrawRay(transform.position, _target, Color.black);

                        float targetDistance = _target.sqrMagnitude;
                        //가장 가까운 적 index
                        if (targetDistance <= shortDistance)
                        {
                            //Debug.Log(targetDistance);
                            shortDistance = targetDistance;
                            index = i; //넘길 번호 설정
                        }
                    }
                }
                //범위 안에는 있지만 angle안에 없을 경우
                else
                {
                    if (count <= 0)
                    {
                        canSee = false;
                    }                            
                    //target이 범위 안에 있다가 밖으로 나간 경우 (아직 구현안됨)
                }
            }
            if(index!= (maxColliders+1)) //초기 설정한 번호가 아니면 return
            {
                return hitColliders[index].transform;
            }
        }
        //싹다 범위 밖에 있었던 경우
        else
        {
            canSee = false;
            count = 0;
        }
        count = Mathf.Clamp(count, 0, maxColliders);
        return null;
    }

    public Transform Item_Detect(Transform transform)
    {
        //받아들일 콜라이더 갯수 설정
        hitColliders = new Collider[maxColliders];
        //Player Layer된 콜라이더 갯수만 가져오기
        numColliders = Physics.OverlapSphereNonAlloc(transform.position, radius, hitColliders, targetLayerMask, QueryTriggerInteraction.Ignore);
        count = 0; //발견 갯수

        if (numColliders != 0)
        {
            float shortDistance = radius * radius;
            int index = maxColliders + 1; //넘길 번호

            for (int i = 0; i < numColliders; i++)
            {
                Vector3 _target = (hitColliders[i].transform.position + Vector3.up) - transform.position;
                _target.y = 0;
                //angle안에 있을 경우
                if (Vector3.Dot(_target.normalized, transform.forward) > Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad))
                {
                    canSee = false;
                    //장애물에 가릴 경우
                    canSee |= !Physics.Raycast(transform.position, _target.normalized, radius, blockLayerMask, QueryTriggerInteraction.Ignore);

                    if (canSee)
                    {
                        count++;
                        Debug.DrawRay(transform.position, _target, Color.black);

                        float targetDistance = _target.sqrMagnitude;
                        //가장 가까운 적 index
                        if (targetDistance <= shortDistance)
                        {
                            //Debug.Log(targetDistance);
                            shortDistance = targetDistance;
                            index = i; //넘길 번호 설정
                        }
                    }
                }
                //범위 안에는 있지만 angle안에 없을 경우
                else
                {
                    if (count <= 0)
                    {
                        canSee = false;
                    }
                    //target이 범위 안에 있다가 밖으로 나간 경우 (아직 구현안됨)
                }
            }
            if (index != (maxColliders + 1)) //초기 설정한 번호가 아니면 return
            {
                return hitColliders[index].transform;
            }
        }
        //싹다 범위 밖에 있었던 경우
        else
        {
            canSee = false;
            count = 0;
        }
        count = Mathf.Clamp(count, 0, maxColliders);
        return null;
    }

    //공격 가능 탐색
    public bool AttackDetect(Transform transform, Transform enemyTransform)
    {
        Vector3 _target = enemyTransform.position - transform.position;
        //전방 10도 안에 있으면 공격 가능
        if (Vector3.Dot(_target.normalized, transform.forward) > Mathf.Cos(10f * 0.5f * Mathf.Deg2Rad))
        {
            return true;
        }   
        return false;
    }

#if UNITY_EDITOR
    public void Editor_TargetScanner(Transform transform)
    {
        Handles.color = canSee ? red : blue; //색 설정
        //부채꼴로 각도 그려줌
        Vector3 rotateForward = Quaternion.Euler(0, -angle * 0.5f, 0f) * transform.forward;
        Handles.DrawSolidArc(transform.position, Vector3.up, rotateForward, angle, radius);
    }

    public void Editor_AttackScanner(Transform transform)
    {
        Gizmos.color = pupple;
        Gizmos.DrawRay(transform.position, transform.forward * radius);
    }
#endif
}
