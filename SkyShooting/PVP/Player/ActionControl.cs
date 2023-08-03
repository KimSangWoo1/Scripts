using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ActionControl : MonoBehaviourPun, IPunObservable
{
    [Header("SO")]
    [SerializeField]
    CommandChannel commandChannel;
    [SerializeField]
    GameModeChannel gameModeChannel;
    [SerializeField]
    TurnBulletChannel turnBulletChannel;

    [Header("Component")]
    [SerializeField]
    Circle_Move circle_Move;
    [SerializeField]
    PhotonMoveTowardsMove photonMoveTowardsMove;
    [SerializeField]
    ActionFx actionFx;
    [SerializeField]
    PhotonTurnBullet myBullet;

    Animator animator;
    BoxCollider boxCollider;
    
    [Header("Settings")]
    [SerializeField]
    private float attackWaitTime;
    [SerializeField]
    private float avoidTime;

    [SerializeField]
    int hp =3 ;

    int myPlayerNumber = -1;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();

        actionFx.SetMyNumber(GameManager.planeNumber);
        myBullet.SetMyPlayer(this.gameObject);
    }

    private void OnEnable()
    {
        gameModeChannel.modeRequested += ChangeMode;
        turnBulletChannel.Requested += BulletInit;
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
        {
            if (myPlayerNumber == 0)
            {
                commandChannel.player1ActionReuqested -= AniControl;
            }
            else if (myPlayerNumber == 1)
            {
                commandChannel.player2ActionReuqested -= AniControl;
            }
        }

        gameModeChannel.modeRequested -= ChangeMode;
        turnBulletChannel.Requested -= BulletInit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != myBullet.gameObject)
        {
            actionFx.Damage();
            hp--;
        }
    }

    private void ChangeMode(TurnModeSceneManager.GameMode gameMode)
    {
        switch (gameMode)
        {
            case TurnModeSceneManager.GameMode.READY:
                break;
            case TurnModeSceneManager.GameMode.COMMAND:
                photonMoveTowardsMove.enabled = false;
                circle_Move.enabled = true;

                boxCollider.enabled = true;
                animator.enabled = false;
                break;
            case TurnModeSceneManager.GameMode.WAIT:
                break;
            case TurnModeSceneManager.GameMode.ACTION:
                circle_Move.enabled = false;
                animator.enabled = true;

                break;
            case TurnModeSceneManager.GameMode.RESULT:
                if (hp <= 0)
                {
                    actionFx.Dead();
                    this.gameObject.SetActive(false);
                }
                break;
            case TurnModeSceneManager.GameMode.END:
                break;
        }
    }

    private void BulletInit()
    {
        myBullet.transform.parent = null;
    }

    public void EventRegister(int playerNumber)
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_SetPlayerNumber), RpcTarget.All, playerNumber);
        }

        if (myPlayerNumber == 0)
        {
            commandChannel.player1ActionReuqested += AniControl;
        }
        else if (myPlayerNumber == 1)
        {
            commandChannel.player2ActionReuqested += AniControl;
        }
    }

    private void Fire()
    {
        myBullet.gameObject.SetActive(true);
    }

    #region Ani
    private void AniControl(string action)
    {
        if(action.Equals("Avoid2"))
        {
            Invoke(nameof(Avoid), avoidTime);
            photonView.RPC(nameof(RPC_SetCollider), RpcTarget.All, false);
        }
        else
        {
            if (action.Equals("Attack"))
            {
                Invoke(nameof(Fire), attackWaitTime);
            }
            animator.SetTrigger(action);
        }
    }

    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    private void Reload()
    {
        animator.SetTrigger("Reload");
    }

    private void Avoid()
    {
        animator.SetTrigger("Avoid");
    }
    #endregion

    #region RPC
    [PunRPC]
    private void RPC_SetCollider(bool able)
    {
        boxCollider.enabled = able;
    }

    [PunRPC]
    private void RPC_SetPlayerNumber(int playerNumber)
    {
        myPlayerNumber = playerNumber;
    }

    [PunRPC]
    private void RPC_AniControl(string action)
    {
        animator.SetTrigger(action);
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }
}
