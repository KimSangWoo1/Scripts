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
    [Header("ī�޶� Ÿ��")]
    public Transform player; //�÷��̾�
    public Transform cameraArm; //�÷��̾� �þ�

    [Header("ī�޶� ��ġ")]
    public Transform camera; //ī�޶�
    public GameObject cameraCenterPos; //ī�޶� ����
    public GameObject cameraUpPos; //ī�޶� �� (������)
    public GameObject cameraLeftPos; //ī�޶� ��
    public GameObject cameraRightPos; //ī�޶� ��
    public Camera fightCamera;

    [Header("���콺 �Է� ȸ������")]
    public float sensitiveX; // ���콺 �Է� �¿� ����
    public float sensitiveY; // ���콺 �Է� ���Ʒ� ����

    [Header("ī�޶� ȸ�� �ӵ� & ī�޶� �̵� �ӵ�")]
    public float turnSpeed; //ī�޶� ȸ���ӵ�
    public float moveSpeed; //ī�޶� �̵��ӵ�

    [Header("ī�޶� Arm Smooth")]
    public float smooth; // ī�޶� Arm �ε巯�� ���� 

    [Header("ī�޶� ��ġ & �Ÿ� & ���� ����")]
    public Vector3 offset; //ī�޶� ��ġ ����
    public float camWidth; // ī�޶��� �Ÿ� ����
    public float camHeight; // ī�޶��� ���� ����

    [Header("�þ� ����")]
    [Range(0f, 3f)]
    public float sight; // ���� ~ �а� ���� (Cambody �� ����)

    [Header("ī�޶� ��ֹ� ���̾�")]
    public LayerMask layerMask; //��ֹ� ���� ���̾�
    public LayerMask terrainLyaer; //��ֹ� ���� ���̾�

    //Variable
    private Transform targetCamPos; //ī�޶� Ÿ�� ��ġ

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

        Init(); //�ʱ�ȭ

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
            CameraCollisionMove(); //ī�޶� �浹 ��� ������
            CameraLook(); // ���� ī�޶� ȸ��
            CameraMouseAngle();// ���콺 �Է¿� ���� �ޱ� (CamArm�� ȸ��)
        }
    }
    //Camera ��ġ&���� �ʱ�ȭ
    void Init()
    {
        //�ʱ� ��ġ
        cameraArm.position = player.position; //�ʱ� ��ġ
        cameraArm.rotation = Quaternion.LookRotation(player.forward + player.transform.up, Vector3.up); //�ʱ� ����
        //���� Cam ��ġ
        cameraCenterPos.transform.localPosition = offset; //����
        cameraUpPos.transform.localPosition = cameraCenterPos.transform.localPosition + Vector3.forward * camHeight;  //��
        cameraLeftPos.transform.localPosition = cameraCenterPos.transform.localPosition - Vector3.right * camWidth; //��
        cameraRightPos.transform.localPosition = cameraCenterPos.transform.localPosition + Vector3.right * camWidth; //��
    }

    #region Input Control
    // �þ߰� ���� (�ָ� ���̵��� or ������ �� �� ���̵���)
    private void SightControl()
    {
        sight += inputManager.GetSightControl();
        sight = Mathf.Clamp(sight, 0, 2);
        transform.rotation = Quaternion.Euler(0f, sight, 0f); //CameraBody �þ߰� ����
    }
    //���콺 �� ��/�ƿ� �Է�  (ĳ���Ϳ� ī�޶� ������ �Ÿ��� ���̰� ������)
    void Zoom()
    {
        float wheel = inputManager.GetMouseScroll();  //ZoomIn < 0f < ZoomOut

        if (wheel != 0f)
        {
            float currentY = cameraCenterPos.transform.localPosition.y - wheel; // ���� �Ÿ�
            float currentZ = cameraCenterPos.transform.localPosition.z + wheel;

            currentY = Mathf.Clamp(currentY, 2f, 7.5f);
            currentZ = Mathf.Clamp(currentZ, -5f, 0f);

            cameraCenterPos.transform.localPosition = new Vector3(0, currentY, 0); // �Ÿ� �缳�� (Pivot �缳��)
            cameraUpPos.transform.localPosition = new Vector3(0, currentY, cameraUpPos.transform.localPosition.z); // �Ÿ� �缳�� (Pivot �缳��)
            cameraLeftPos.transform.localPosition = new Vector3(cameraLeftPos.transform.localPosition.x, currentY, 0); // �Ÿ� �缳�� (Pivot �缳��)
            cameraRightPos.transform.localPosition = new Vector3(cameraRightPos.transform.localPosition.x, currentY, 0); // �Ÿ� �缳�� (Pivot �缳��)

            offset.y = currentY;
        }
    }
    #endregion

    //ī�޶� �̵�
    void CameraLook()
    {
        Zoom(); //���콺 ��
        SightControl(); //�þ߰� ����

        // ī�޶� ���� ���ϱ�
        cameraArm.position = player.transform.position; //ī�޶� Arm�� Player ��ġ�� �����ϰ�
        Vector3 direct = cameraArm.position - transform.position;
        direct = (direct + transform.rotation.eulerAngles).normalized;   // �������� = Ÿ�ٹ��� + �þ� ������

        camera.rotation = Quaternion.Slerp(camera.rotation, Quaternion.LookRotation(direct), Time.deltaTime * turnSpeed); //ī�޶� ȸ��

        //cameraCenterPos.transform.rotation = Quaternion.LookRotation(cameraArm.position - cameraCenterPos.transform.position); //ī�޶� ȸ��
        //cameraLeftPos.transform.rotation = Quaternion.LookRotation(cameraArm.position - cameraLeftPos.transform.position); //ī�޶� ȸ��
        //cameraRightPos.transform.rotation = Quaternion.LookRotation(cameraArm.position - cameraRightPos.transform.position); //ī�޶� ȸ��
    }
    //ī�޶�(Arm) ȸ��
    void CameraMouseAngle()
    {
        //Input  ���콺 �� Ŭ���� ���콺 �̵� �Է� �ޱ�
        if (Input.GetKey(KeyCode.Mouse2))
        {
            Vector2 mouseAngle = inputManager.GetMouseAngle();
            mouseAngle = new Vector2(mouseAngle.x * sensitiveX, mouseAngle.y * sensitiveY);  //���콺�κ��� ȸ�� ���� ����

            if (!fightCamera.enabled)
            {
                Vector3 cameraAngle = cameraArm.rotation.eulerAngles; // ���� CamArm �ޱ� ����

                float x = cameraAngle.x - mouseAngle.y; //����  ���簪 ���콺 �Է°� ��ġ��  
                float y = cameraAngle.y + mouseAngle.x; //�¿� ���簪 ���콺 �Է°� ��ġ��

                x = Mathf.Clamp(x, 270f, 350f); // ���� 365f ~ 270f ����  (350f �Ѵ� ���� ���ö��� ���� ������ �����)

                Quaternion euler = Quaternion.Euler(x, y, 0f); //ȸ�� ��

                cameraArm.rotation = Quaternion.Lerp(cameraArm.rotation, euler, Time.deltaTime * smooth); //ȸ��
            }
            else
            {
                
                float x = fightCamera.transform.eulerAngles.x - mouseAngle.y; //����  ���簪 ���콺 �Է°� ��ġ��  
                float y = player.eulerAngles.y + mouseAngle.x; //�¿� ���簪 ���콺 �Է°� ��ġ��
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
        //ĳ���� ������Ʈ
        RaycastHit hit;
        RaycastHit hit2;

        //ī�޶� -> ī�޶� ��ġ �̵��� ( Object�� �ɸ� ��)   ex) �ٱ� ->����
        if (Physics.Linecast(targetCamPos.position, camera.transform.position, out hit, layerMask)) 
        {
            //Player ī�޶���ġ ���̿�,  ī�޶�  ī�޶���ġ ���̿� ��ֹ��� ���� ���
            if (Physics.Linecast(cameraArm.position + Vector3.up, cameraArm.TransformPoint(cameraCenterPos.transform.localPosition), out hit, layerMask))
            {
                transform.position = Vector3.Slerp(transform.position, hit.collider.bounds.ClosestPoint(hit.point) + (Vector3.up * 1.2f), Time.deltaTime * moveSpeed); // 2�� ������ 
            }
            else  //��ó ������ ���ǿ� ���� ��
            {
                 transform.position = Vector3.Slerp(transform.position, targetCamPos.position + Vector3.up *1.2f, Time.deltaTime * moveSpeed); //  2�� ������ (ī�޶� ������ 1�� ����������..)
            }
        }
        else
        {
            //ī�޶���ġ �� ī�޶� �̹� Object ������ ��   ex) ����
            if (Physics.Linecast(cameraArm.position + Vector3.up, cameraArm.TransformPoint(cameraCenterPos.transform.localPosition), out hit2, layerMask))
            {
                Debug.DrawLine(cameraArm.position, hit2.point, Color.red, 1f);
                //�Ÿ� ����
                Vector3 camDistance = cameraArm.position - hit2.collider.bounds.ClosestPoint(hit2.point);
                float sqrDistance = camDistance.sqrMagnitude;

                if (sqrDistance < 3f)
                {  //(�ʹ� ����� ���)
                    transform.position = Vector3.Slerp(transform.position, hit2.collider.bounds.ClosestPoint(hit2.point) + (Vector3.up * 3f), Time.deltaTime * moveSpeed); //3�� ������ 
                }
                else
                { //(�Ÿ��� ������ ���)
                    transform.position = Vector3.Slerp(transform.position, hit2.collider.bounds.ClosestPoint(hit2.point) + (Vector3.up * 1.2f), Time.deltaTime * moveSpeed); //2�� ������ 
                }
            }
            else  //�׳� ����� ��
            {
                transform.position = Vector3.Slerp(transform.position, targetCamPos.position , Time.deltaTime * moveSpeed); // 1�� ������ Default
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
