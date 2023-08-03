using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("맵 설정")]
    [Range(5, 100)]
    public float scale; //Map 크기
    [SerializeField]
    [Range(5,30)]
    private int margine; //Map 경계선 거리두기 길이 (마진 남기기)

    [Header("랜덤 좌표 범위 정하기")]
    public float limitRange; // 범위 정하기 
    [Tooltip("무조건 permitRangeMax 보다 1이상 작아야 작동")]
    [Range(1,9)]
    public float permitRangeMin; // 허용 범위 최소
    [Range(2, 10)]
    public float permitRangeMax; // 허용 범위 최대 
    [Range(60, 360)]
    public float permitRangeAngle; // 허용 범위 앵글
    [SerializeField]
    private float ray_Duration; // 레이져 시간

    [Header("장애물 레이어")]
    public LayerMask obstacleLayer;
    [Header("비행기 레이어")]
    public LayerMask planeLayer;
    [HideInInspector]
    public Renderer rend; //MeshRender

    private float eyePos; //비행기 높이

    private Vector3 size; // 맵 실제 크기
    private Vector3 point; // 랜덤 Point
    private Vector3 inside; //InsideSquere 값

    //public Transform target; //test
    void Start()
    {
        rend = GetComponent<Renderer>();
        //scale = 10f;
        margine = 10;
        limitRange = 4;
        permitRangeMin = 5;
        permitRangeMax = 10;
        ray_Duration = 3f;
        permitRangeAngle = 270f;
        eyePos = 2f;

    }
    private void Update()
    {

    }
    //Map 크기 설정
    internal void GeneratorMap()
    {
        transform.localScale = new Vector3(scale, 1f, scale);
    }

    //Map 랜덤 좌표 받기
    public Vector3 Random_Position()
    {
        size = rend.bounds.extents; //bounds는 Object 실제 크기  .extends는 size의 반값
        size = size - Vector3.one * margine; //margine두기
        float x = Random.Range(-size.x, size.x);
        float z = Random.Range(-size.z, size.z);
        point = new Vector3(x, eyePos, z);
       // Debug.DrawRay(point, Vector3.up, Color.red, ray_Duration);

        /*
        //장애물이 있을 경우
        Collider[] hitColliders = new Collider[1];
        int numColliders = Physics.OverlapSphereNonAlloc(postion, margine, hitColliders, obstacleLayer, QueryTriggerInteraction.Collide);
        */

        return point;
    }
    // Point 중심의 주변 랜덤 좌표 얻기
    public Vector3 Spot_RendomPosition(Transform plane)
    {
        size = rend.bounds.extents; //bounds는 Object 실제 크기  .extends는 size의 반값
        size = size - Vector3.one * margine; //margine두기
        point = Vector3.zero; //결과값

        inside = Random.insideUnitSphere;
        float x = inside.x * 10f;
        float z = inside.z * 10f;

        //원형구멍
        if (z >= 0 && z <= permitRangeMin)
        {
            if (x > 0 && x < permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = permitRangeMin;
                    z = permitRangeMin;
                }
            }
            else if (x < 0 && x > -permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = -permitRangeMin;
                    z = permitRangeMin;
                }
            }
        }
        else if (z <= 0 && z >= -permitRangeMin)
        {
            if (x > 0 && x < permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = permitRangeMin;
                    z = -permitRangeMin;
                }
            }
            else if (x < 0 && x > -permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = -permitRangeMin;
                    z = -permitRangeMin;
                }
            }
        }

        //범위 키우기
        x = x * limitRange;
        z = z * limitRange;

        point = plane.position + new Vector3(x, 0f, z);

        //장애물이 있을 경우
        Collider[] hitColliders = new Collider[1]; //장애물 인지 갯수 1개 해도 되고 여러개 해도 됨
        int numColliders = Physics.OverlapSphereNonAlloc(point, 10f, hitColliders, obstacleLayer, QueryTriggerInteraction.Collide);
        if (numColliders != 0)
        {
            Vector3 _point = hitColliders[0].bounds.ClosestPoint(point);
            Vector3 center = hitColliders[0].bounds.center;

            Vector3 dir = _point - center;
            dir = dir.normalized;
            point = _point + (dir * margine);
        }

        //비행기 360도 안에 좌표중에서만
        Vector3 _target = point - plane.position;
        if (Vector3.Dot(_target.normalized, -plane.forward) > Mathf.Cos(permitRangeAngle * 0.5f * Mathf.Deg2Rad))
        {
           // Debug.DrawRay(point, Vector3.up, Color.blue, 5f);
        }
        else
        {
            Quaternion v3Rotation = Quaternion.Euler(0f, 180f, 180f);  // 회전각     
            Vector3 v3RotatedDirection = v3Rotation * _target; // (회전각 * 회전시킬 벡터)
            point = v3RotatedDirection + plane.position; //따라서 현재 포지션 더해줌
        }

        //좌표가 맵 경계선을 넘지 않도록 
        x = Mathf.Clamp(point.x, -size.x, size.x);
        z = Mathf.Clamp(point.z, -size.z, size.z);

        point = new Vector3(x, plane.position.y, z);

        //고착된 경우 너무 가까이 좌표가 되는걸 방지
        float distance = Vector3.Distance(plane.position, new Vector3(point.x, plane.position.y, point.z));
        if (distance < permitRangeMin * limitRange) //limitRange 곱해주는 이유는  이미 범위를 키웠기 때문에 그 범위와 동일하게 거리를 재야해서
        {
            return Random_Position();
        }

        return point;
    }

    //비행기 끼리 부딪히는거 피하기 좌표
    public Vector3 ClashAvoid_RandomPosition(Transform plane)
    {
        size = rend.bounds.extents; //bounds는 Object 실제 크기  .extends는 size의 반값
        size = size - Vector3.one * margine; //margine두기
        point = Vector3.zero; //결과값

        inside = Random.insideUnitSphere;
        float x = inside.x * 10f;
        float z = inside.z * 10f;

        //원형구멍
        if (z >= 0 && z <= permitRangeMin)
        {
            if (x > 0 && x < permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = permitRangeMin;
                    z = permitRangeMin;
                }
            }
            else if (x < 0 && x > -permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = -permitRangeMin;
                    z = permitRangeMin;
                }
            }
        }
        else if (z <= 0 && z >= -permitRangeMin)
        {
            if (x > 0 && x < permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = permitRangeMin;
                    z = -permitRangeMin;
                }
            }
            else if (x < 0 && x > -permitRangeMin)
            {
                Vector3 point2 = point + new Vector3(x, 0f, z);
                float _distance = Vector3.Distance(point, point2);
                if (_distance <= permitRangeMin)
                {
                    x = -permitRangeMin;
                    z = -permitRangeMin;
                }
            }
        }

        point = point + (new Vector3(x, 0f, z) * limitRange);

        //비행기 60도 안에 좌표중에서만
        Vector3 _target = point - plane.position;
        if (Vector3.Dot(_target.normalized, plane.right) > Mathf.Cos(60f * 0.5f * Mathf.Deg2Rad))
        {

        }
        //우회전하기
        else
        {
            Quaternion quater = Quaternion.FromToRotation(_target, plane.right); //회전 방향
            Vector3 v3RotatedDirection = quater * _target; // (회전각 * 회전시킬 벡터)
            point = v3RotatedDirection + plane.position; //따라서 현재 포지션 더해줌

        }

        //좌표가 맵 경계선을 넘지 않도록 
        x = Mathf.Clamp(point.x, -size.x, size.x);
        z = Mathf.Clamp(point.z, -size.z, size.z);
        point = new Vector3(x, plane.position.y, z);

        return point;
    }

}
