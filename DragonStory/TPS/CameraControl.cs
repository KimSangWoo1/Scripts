using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    FightChannelSO fightChannelSO;

    //Manager
    InputManager inputManager;

    //Inspector
    [Header("카메라 타겟")]
    public Transform player; //플레이어
    public Transform cameraArm; //플레이어 시야

    [Header("카메라 위치")]
    public Transform camera; //카메라
    public GameObject cameraCenterPos; //카메라 센터
    public GameObject cameraUpPos; //카메라 위 (감지용)
    public GameObject cameraLeftPos; //카메라 좌
    public GameObject cameraRightPos; //카메라 우
    public Camera fightCamera;

    [Header("마우스 입력 회전감도")]
    public float sensitiveX; // 마우스 입력 좌우 감도
    public float sensitiveY; // 마우스 입력 위아래 감도

    [Header("카메라 회전 속도 & 카메라 이동 속도")]
    public float turnSpeed; //카메라 회전속도
    public float moveSpeed; //카메라 이동속도

    [Header("카메라 Arm Smooth")]
    public float smooth; // 카메라 Arm 부드러움 정도 

    [Header("카메라 위치 & 거리 & 높이 조정")]
    public Vector3 offset; //카메라 위치 조정
    public float camWidth; // 카메라간의 거리 조정
    public float camHeight; // 카메라간의 높이 조정

    [Header("시야 감도")]
    [Range(0f, 3f)]
    public float sight; // 좁게 ~ 넓게 보기 (Cambody 각 조절)

    [Header("카메라 장애물 레이어")]
    public LayerMask layerMask; //장애물 감지 레이어
    public LayerMask terrainLyaer; //장애물 감지 레이어

    //Variable
    private Transform targetCamPos; //카메라 타겟 위치

    [Header("Fight Y Clamp")] 
    private float totalY;
    [SerializeField]
    private float max;
    [SerializeField]
    private float min;

    private void OnEnable()
    {
        fightChannelSO.OnPlayerFightModeRequested += PlayerFight;
        fightChannelSO.OnPlayerNormalModeRequested += PlayerNormal;
        fightChannelSO.OnNpcDeadRequested += PlayerNormal;
    }

    private void OnDisable()
    {
        fightChannelSO.OnPlayerFightModeRequested -= PlayerFight;
        fightChannelSO.OnPlayerNormalModeRequested -= PlayerNormal;
        fightChannelSO.OnNpcDeadRequested -= PlayerNormal;
    }
    void Start()
    {
        inputManager = InputManager.Instance;

        Init(); //초기화

        targetCamPos = cameraCenterPos.transform;
    }

    void Update()
    {
        // Camera Direct Reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            Init();
        }
    }

    private void LateUpdate()
    {
        if (GameManager.mode == Mode.NONE || GameManager.mode == Mode.TALK || GameManager.mode == Mode.FIGHT)
        {
            CameraCollisionMove(); //카메라 충돌 계산 움직임
            CameraLook(); // 실제 카메라 회전
            CameraMouseAngle();// 마우스 입력에 따른 앵글 (CamArm의 회전)
        }
    }
    //Camera 위치&각도 초기화
    void Init()
    {
        //초기 위치
        cameraArm.position = player.position; //초기 위치
        cameraArm.rotation = Quaternion.LookRotation(player.forward + player.transform.up, Vector3.up); //초기 방향
        //기존 Cam 위치
        cameraCenterPos.transform.localPosition = offset; //센터
        cameraUpPos.transform.localPosition = cameraCenterPos.transform.localPosition + Vector3.forward * camHeight;  //상
        cameraLeftPos.transform.localPosition = cameraCenterPos.transform.localPosition - Vector3.right * camWidth; //좌
        cameraRightPos.transform.localPosition = cameraCenterPos.transform.localPosition + Vector3.right * camWidth; //우
    }

    #region Input Control
    // 시야각 조절 (멀리 보이도록 or 가까이 더 잘 보이도록)
    private void SightControl()
    {
        sight += inputManager.GetSightControl();
        sight = Mathf.Clamp(sight, 0, 2);
        transform.rotation = Quaternion.Euler(0f, sight, 0f); //CameraBody 시야각 조절
    }
    //마우스 줌 인/아웃 입력  (캐릭터와 카메라 사이의 거리를 줄이고 넓혀줌)
    void Zoom()
    {
        float wheel = inputManager.GetMouseScroll();  //ZoomIn < 0f < ZoomOut

        if (wheel != 0f)
        {
            float currentY = cameraCenterPos.transform.localPosition.y - wheel; // 현재 거리
            float currentZ = cameraCenterPos.transform.localPosition.z + wheel;

            currentY = Mathf.Clamp(currentY, 2f, 7.5f);
            currentZ = Mathf.Clamp(currentZ, -5f, 0f);

            cameraCenterPos.transform.localPosition = new Vector3(0, currentY, 0); // 거리 재설정 (Pivot 재설정)
            cameraUpPos.transform.localPosition = new Vector3(0, currentY, cameraUpPos.transform.localPosition.z); // 거리 재설정 (Pivot 재설정)
            cameraLeftPos.transform.localPosition = new Vector3(cameraLeftPos.transform.localPosition.x, currentY, 0); // 거리 재설정 (Pivot 재설정)
            cameraRightPos.transform.localPosition = new Vector3(cameraRightPos.transform.localPosition.x, currentY, 0); // 거리 재설정 (Pivot 재설정)

            offset.y = currentY;
        }
    }
    #endregion

    //카메라 이동
    void CameraLook()
    {
        Zoom(); //마우스 줌
        SightControl(); //시야각 조절

        // 카메라 방향 구하기
        cameraArm.position = player.transform.position; //카메라 Arm이 Player 위치와 동일하게
        Vector3 direct = cameraArm.position - transform.position;
        direct = (direct + transform.rotation.eulerAngles).normalized;   // 실제방향 = 타겟방향 + 시야 조절각

        camera.rotation = Quaternion.Slerp(camera.rotation, Quaternion.LookRotation(direct), Time.deltaTime * turnSpeed); //카메라 회전

        //cameraCenterPos.transform.rotation = Quaternion.LookRotation(cameraArm.position - cameraCenterPos.transform.position); //카메라 회전
        //cameraLeftPos.transform.rotation = Quaternion.LookRotation(cameraArm.position - cameraLeftPos.transform.position); //카메라 회전
        //cameraRightPos.transform.rotation = Quaternion.LookRotation(cameraArm.position - cameraRightPos.transform.position); //카메라 회전
    }
    //카메라(Arm) 회전
    void CameraMouseAngle()
    {
        //Input  마우스 휠 클릭시 마우스 이동 입력 받기
        if (Input.GetKey(KeyCode.Mouse2))
        {
            Vector2 mouseAngle = inputManager.GetMouseAngle();
            mouseAngle = new Vector2(mouseAngle.x * sensitiveX, mouseAngle.y * sensitiveY);  //마우스로부터 회전 값을 받음

            if (!fightCamera.enabled)
            {
                Vector3 cameraAngle = cameraArm.rotation.eulerAngles; // 현재 CamArm 앵글 받음

                float x = cameraAngle.x - mouseAngle.y; //상하  현재값 마우스 입력값 합치기  
                float y = cameraAngle.y + mouseAngle.x; //좌우 현재값 마우스 입력값 합치기

                x = Mathf.Clamp(x, 270f, 350f); // 수직 365f ~ 270f 수평  (350f 넘는 값이 나올때도 있음 수직에 가까움)

                Quaternion euler = Quaternion.Euler(x, y, 0f); //회전 값

                cameraArm.rotation = Quaternion.Lerp(cameraArm.rotation, euler, Time.deltaTime * smooth); //회전
            }
            else
            {
                
                float x = fightCamera.transform.eulerAngles.x - mouseAngle.y; //상하  현재값 마우스 입력값 합치기  
                float y = player.eulerAngles.y + mouseAngle.x; //좌우 현재값 마우스 입력값 합치기
                totalY += mouseAngle.x;

                // X Limit
                if (x >= 15 && x <= 90f)
                {
                    x = 15f;
                }
                else if (x <= 270f && x >= 200f)
                {
                    x = 270f;
                }

                fightCamera.transform.eulerAngles = new Vector3(x, fightCamera.transform.eulerAngles.y, 0f); // Vertical Rot
                // Y Limit
                if(totalY<=max && totalY >= min)
                {
                    player.transform.localEulerAngles = new Vector3(player.eulerAngles.x, y, 0f); // Horizontal Rot
                }
            }
        }
        else
        {
            totalY = 0f;
        }
    }

    private void CameraCollisionMove()
    {
        //캐스팅 오브젝트
        RaycastHit hit;
        RaycastHit hit2;

        //카메라 -> 카메라 위치 이동시 ( Object들 걸릴 때)   ex) 바깥 ->안쪽
        if (Physics.Linecast(targetCamPos.position, camera.transform.position, out hit, layerMask)) 
        {
            //Player 카메라위치 사이에,  카메라  카메라위치 사이에 장애물이 있을 경우
            if (Physics.Linecast(cameraArm.position + Vector3.up, cameraArm.TransformPoint(cameraCenterPos.transform.localPosition), out hit, layerMask))
            {
                transform.position = Vector3.Slerp(transform.position, hit.collider.bounds.ClosestPoint(hit.point) + (Vector3.up * 1.2f), Time.deltaTime * moveSpeed); // 2번 움직임 
            }
            else  //근처 자잘한 물건에 닿을 때
            {
                 transform.position = Vector3.Slerp(transform.position, targetCamPos.position + Vector3.up *1.2f, Time.deltaTime * moveSpeed); //  2번 움직임 (카메라 진동시 1번 움직임으로..)
            }
        }
        else
        {
            //카메라위치 와 카메라가 이미 Object 안쪽일 때   ex) 안쪽
            if (Physics.Linecast(cameraArm.position + Vector3.up, cameraArm.TransformPoint(cameraCenterPos.transform.localPosition), out hit2, layerMask))
            {
                Debug.DrawLine(cameraArm.position, hit2.point, Color.red, 1f);
                //거리 조절
                Vector3 camDistance = cameraArm.position - hit2.collider.bounds.ClosestPoint(hit2.point);
                float sqrDistance = camDistance.sqrMagnitude;

                if (sqrDistance < 3f)
                {  //(너무 가까울 경우)
                    transform.position = Vector3.Slerp(transform.position, hit2.collider.bounds.ClosestPoint(hit2.point) + (Vector3.up * 3f), Time.deltaTime * moveSpeed); //3번 움직임 
                }
                else
                { //(거리가 적당할 경우)
                    transform.position = Vector3.Slerp(transform.position, hit2.collider.bounds.ClosestPoint(hit2.point) + (Vector3.up * 1.2f), Time.deltaTime * moveSpeed); //2번 움직임 
                }
            }
            else  //그냥 평범할 때
            {
                transform.position = Vector3.Slerp(transform.position, targetCamPos.position , Time.deltaTime * moveSpeed); // 1번 움직임 Default
            }
        }
    }

    #region Fight Mode
    private void PlayerFight()
    {
        //fightCamera.gameObject.tag = "MainCamera";
        fightCamera.enabled = true;
        camera.gameObject.SetActive(false);
    }

    private void PlayerNormal()
    {
       //fightCamera.gameObject.tag = "Untagged";
        fightCamera.enabled = false;
        camera.gameObject.SetActive(true);
    }
    #endregion
}
