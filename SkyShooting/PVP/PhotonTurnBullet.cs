using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonTurnBullet: MonoBehaviourPunCallbacks
{
    [SerializeField]
    TurnBulletChannel turnBulletChannel;
    
    private MeshRenderer meshRenderer; 
    private TrailRenderer trailRenderer;

    private GameObject myPlayer;
    private Vector3 startPos;

    [SerializeField]
    private float bulletSpeed; //총알 속도
    [SerializeField]
    private float lifeTime; //생명 시간
    [SerializeField]
    private float deadTime; //죽는 시간

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        startPos = transform.position;
    }

    private void OnEnable()
    {
        base.OnEnable();

        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_BulletInit), RpcTarget.All, true);
        }
    }

    void Update()
    {
        if(lifeTime <= deadTime)
        {
            lifeTime += Time.deltaTime;
        }
        else
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_BulletInit), RpcTarget.All, false);
            }
            lifeTime = 0;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed, Space.Self); //총알 이동
    }

    public void SetMyPlayer(GameObject parent)
    {
        myPlayer = parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != myPlayer)
        {
            ActionCam.target = other.gameObject;
            ActionCam.shake = true;

            gameObject.SetActive(false);
            transform.position = startPos;

            if (photonView.IsMine)
            {
                turnBulletChannel.OtherPlayerHitEvent();
            }
            else
            {
                turnBulletChannel.PlayerHitEvent();
            }
        }
    }

    #region RPC
    [PunRPC]
    private void RPC_BulletInit(bool active)
    {
        gameObject.SetActive(active);
        transform.position = startPos;
        lifeTime = 0f;
    }
    #endregion
}


// OnPhotonSerializeView는 RPC 전에만 작동 된다.
/*
//OnEable때 리모트 오브젝트 초기화
[PunRPC]
private void RPCBulletLife()
{
    lifeTime = 0f;
    active = true;
    gameObject.SetActive(true);
}

[PunRPC]
private void RPCBulletReady(Vector3 firePosition, Quaternion fireRotation)
{
    transform.position = firePosition;
    transform.rotation = fireRotation;
}

 */