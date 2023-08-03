using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class TurnModeSceneManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public enum GameMode { READY =1, COMMAND=2, WAIT=3, ACTION=4, RESULT=5, END=6 } //준비, 명령, 통신대기, 명령실행, 결과, 종료
    [SerializeField]
    private GameMode gameMode;

    [Header("Channel / SO")]
    [SerializeField]
    private CommandChannel commandChannel;
    [SerializeField]
    private GameModeChannel gameModeChannel;

    [Header("UI")]
    [SerializeField]
    Text aniPlayer1Name;
    [SerializeField]
    Text aniPlayer2Name;
    [SerializeField]
    Text gamePlayer1Name;
    [SerializeField]
    Text gamePlayer2Name;

    [Header("UI - End")]
    [SerializeField]
    Canvas gameCanvas;
    [SerializeField]
    GameObject winBoard;
    [SerializeField]
    GameObject drawBoard;
    [SerializeField]
    GameObject loseBoard;

    [Header("Component")]
    [SerializeField]
    CameraDirector cameraDirector;

    [Header("Game Setting")]
    [SerializeField]
    private Transform[] playerStartPos;
    [SerializeField]
    private Transform[] playerGamePos;

    [Header("Wind Speed Line Fx")]
    [SerializeField]
    private GameObject[] windFx;

    [Header("Players Command")]
    [SerializeField]
    private int player1CommnadType;
    [SerializeField]
    private int player2CommnadType;

    [Header("Players HP")]
    [SerializeField]
    private int player1HP = 3;
    [SerializeField]
    private int player2HP = 3;

    private bool action1;
    private bool action2;

    private readonly float actionWaitTime = 1.45f;
    private readonly float actionEndTime = 3f;

    #region MonoBehaviour
    private void Awake()
    {
        player1CommnadType = -1;
        player2CommnadType = -1;
    }

    private void OnEnable()
    {
        base.OnEnable();
        gameModeChannel.startRequested += GameStart;
    }

    private void OnDisable()
    {
        base.OnDisable();
        gameModeChannel.startRequested -= GameStart;
    }

    void Start()
    {
        if(PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            //Player 배치
            PlayerCreation(0);
            aniPlayer1Name.text = PhotonNetwork.PlayerList[0].NickName;
            aniPlayer2Name.text = PhotonNetwork.PlayerList[1].NickName;

            gamePlayer1Name.text = PhotonNetwork.PlayerList[0].NickName;
            gamePlayer2Name.text = PhotonNetwork.PlayerList[1].NickName;
        }
        else
        {
            PlayerCreation(1);
            aniPlayer1Name.text = PhotonNetwork.PlayerList[0].NickName;
            aniPlayer2Name.text = PhotonNetwork.PlayerList[1].NickName;

            gamePlayer1Name.text = PhotonNetwork.PlayerList[1].NickName;
            gamePlayer2Name.text = PhotonNetwork.PlayerList[0].NickName;
        }

        ChangeGameMode(GameMode.READY);
    }

    private void Update()
    {
        Debug.Log(gameMode);
        switch (gameMode)
        {
            case GameMode.WAIT:
                WaitCheck();
                break;
            case GameMode.RESULT:
                PlayerResult();

                break;
            case GameMode.END:
                GameEnd();
                gameMode = GameMode.READY;
                break;
            default:
                break;
        }
    }
    #endregion

    private void PlayerCreation(int playerNumber)
    {
        GameObject player = PhotonNetwork.Instantiate("TurnPlayer", playerStartPos[playerNumber].position, playerStartPos[playerNumber].rotation);
        player.GetComponent<ActionControl>().EventRegister(playerNumber);
        player.GetComponent<PhotonMoveTowardsMove>().SetTarget(playerGamePos[playerNumber].position);
        cameraDirector.SetPlayer(playerNumber, player);  //카메라 셋팅
    }


    #region Change Mode
    private void GameStart()
    {
        gameModeChannel.ModeEvent(GameMode.COMMAND);
    }

    private void ChangeGameMode(GameMode _gameMode)
    {
        gameMode = _gameMode;

        if (photonView.IsMine && PhotonNetwork.IsMasterClient)
        { 
            photonView.RPC(nameof(RPC_GameMode), RpcTarget.All,GetMode());
        }
    }

    private int GetMode()
    {
        int result = 0;

        switch (gameMode)
        {
            case GameMode.READY:
                result = 1;
                break;
            case GameMode.COMMAND:
                result = 2;
                break;
            case GameMode.WAIT:
                result = 3;
                break;
            case GameMode.ACTION:
                result = 4;
                break;
            case GameMode.RESULT:
                result = 5;
                break;
            case GameMode.END:
                result = 6;
                break;
        }
        return result;
    }
    #endregion

    #region Action
    public void InputCommand(int commandNum)
    {
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            //RPC 1
            photonView.RPC(nameof(RPC_Player1Command), RpcTarget.All, commandNum);
        }
        else
        {
            //RPC 2
            photonView.RPC(nameof(RPC_Player2Command), RpcTarget.All, commandNum);
        }
    }

    private void Player1Action()
    {
        switch (player1CommnadType)
        {
            case 0:
                windFx[0].gameObject.SetActive(true);
                windFx[1].gameObject.SetActive(false);
                commandChannel.Player1ActionEvent("Attack");
                cameraDirector.ActionCamOn(1); //공격 카메라 키기
                cameraDirector.ActionCamOff(2);
                break;
            case 1:
                commandChannel.Player1ActionEvent("Reload");
                cameraDirector.ActionCamOn(1); //카메라 키기
                cameraDirector.ActionCamOff(2);
                //장전 효과
                break;
            case 2:
                commandChannel.Player1ActionEvent("Avoid");
                cameraDirector.ActionCamOn(1); //카메라 키기
                cameraDirector.ActionCamOff(2);
                break;
        }
    }

    private void Player2Action()
    {
        switch (player2CommnadType)
        {
            case 0:
                windFx[0].gameObject.SetActive(false);
                windFx[1].gameObject.SetActive(true);
                commandChannel.Player2ActionEvent("Attack");
                cameraDirector.ActionCamOn(2); //공격 카메라 키기
                cameraDirector.ActionCamOff(1);
                break;
            case 1:
                commandChannel.Player2ActionEvent("Reload");
                cameraDirector.ActionCamOn(2); //카메라 키기
                cameraDirector.ActionCamOff(1);
                //장전 효과
                break;
            case 2:
                commandChannel.Player2ActionEvent("Avoid");
                cameraDirector.ActionCamOn(2); //카메라 키기
                cameraDirector.ActionCamOff(1);
                break;
        }
    }

    IEnumerator ActionTime()
    {
        yield return new WaitUntil(() => action1 && action2);
        yield return new WaitForSeconds(actionEndTime);
        photonView.RPC(nameof(RPC_ResetCommand), RpcTarget.All);
        cameraDirector.ActionCamOff();
        windFx[0].gameObject.SetActive(false);
        windFx[1].gameObject.SetActive(false);
        action1 = false;
        action2 = false;

        ChangeGameMode(GameMode.RESULT);
    }

    #endregion

    #region Wait
    private void WaitCheck() {

        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if (gameMode == GameMode.WAIT)
            {
                if (player1CommnadType != -1 && player2CommnadType != -1)
                {
                    // Wait -> Action -> Result -> Command
                    ChangeGameMode(GameMode.ACTION);
                    photonView.RPC(nameof(RPC_ActionTime), RpcTarget.All);
                }
            }
        }
    }

    public void WaitOtherPlyerCommand()
    {
        gameMode = GameMode.WAIT;
        gameModeChannel.ModeEvent(gameMode);
    }
    #endregion

    #region Result
    private void PlayerResult()
    {
        if (player1HP <= 0 || player2HP <= 0)  //게임 종료
        {
            // 패배 Player 비행기 폭발 후  End로

            ChangeGameMode(GameMode.END);
        }
        else                                 //게임 계속
        {
            ChangeGameMode(GameMode.COMMAND);
        }
    }
    #endregion

    #region End
    private void GameEnd()
    {
        if (player1HP <= 0 && player2HP <= 0) //Draw
        {
            drawBoard.SetActive(true);
        }
        else if (player1HP <= 0) // P1 Lose : P2 Win
        {
            if (photonView.IsMine)
            {
                loseBoard.SetActive(true);
            }
            else
            {
                winBoard.SetActive(true);
            }
        }
        else if (player2HP <= 0)// P1 Win : P2 Lose
        {
            if (photonView.IsMine)
            {
                winBoard.SetActive(true);
            }
            else
            {
                loseBoard.SetActive(true);
            }
        }
        gameCanvas.gameObject.SetActive(false);
    }
    #endregion

    #region Button Click
    public void GameEndClick()
    {
        PhotonNetwork.LoadLevel("Photon_Lobby");
    }
    #endregion

    #region RPC
    [PunRPC]
    private void RPC_ActionTime()
    {
        if (gameMode == GameMode.ACTION)
        {
            //공격
            if (player1CommnadType == 0)
            {
                action1 = true;
                Player1Action();

                //피할 경우 줌 아웃+ 총알 사라지기
                if (player2CommnadType == 2)
                {
                    action2 = true;
                    commandChannel.Player2ActionEvent("Avoid2");

                    StartCoroutine(ActionTime()); //Action 종료
                    return;
                }
                else
                {
                    player2HP--;
                }
            }

            if (player2CommnadType == 0)
            {
                action2 = true;
                if (action1)
                {
                    Invoke(nameof(Player2Action), actionWaitTime);
                }
                else
                {
                    Player2Action();
                }

                //피할 경우 줌 아웃+ 총알 사라지기
                if (player1CommnadType == 2)
                {
                    action1 = true;
                    commandChannel.Player1ActionEvent("Avoid2");

                    StartCoroutine(ActionTime()); //Action 종료 
                    return;
                }
                else
                {
                    player1HP--;
                }
            }

            //피하기 
            if (player1CommnadType == 2)
            {
                action1 = true;
                if (action2)
                {
                    Invoke(nameof(Player1Action), actionWaitTime);
                }
                else
                {
                    Player1Action();
                }
            }
            if (player2CommnadType == 2)
            {
                action2 = true;
                if (action1)
                {
                    Invoke(nameof(Player2Action), actionWaitTime);
                }
                else
                {
                    Player2Action();
                }
            }

            //장전
            if (player1CommnadType == 1)
            {
                action1 = true;
                if (action2)
                {
                    Invoke(nameof(Player1Action), actionWaitTime);
                }
                else
                {
                    Player1Action();
                }
            }
            if (player2CommnadType == 1)
            {
                action2 = true;
                if (action1)
                {
                    Invoke(nameof(Player2Action), actionWaitTime);
                }
                else
                {
                    Player2Action();
                }
            }

            StartCoroutine(ActionTime()); //Action 종료
        }
    }

    [PunRPC]
    private void RPC_GameMode(int mode)
    {
        gameMode = (GameMode)Enum.ToObject(typeof(GameMode), mode);
        gameModeChannel.ModeEvent(gameMode);
    }

    [PunRPC]
    private void RPC_Player1Command(int commandNum)
    {
        player1CommnadType = commandNum;
    }

    [PunRPC]
    private void RPC_Player2Command(int commandNum)
    {
        player2CommnadType = commandNum;
    }

    [PunRPC]
    private void RPC_ResetCommand()
    {
        player1CommnadType = -1;
        player2CommnadType = -1;

    }
    #endregion
    #region Photon Callback
    //방 나갈 경우
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }

    //상대 Player가 방 들어왔을 경우
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("들옴");
    }

    //상대 Player가 방 나갈 경우
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.LoadLevel("Photon_Lobby");
    }
    #endregion
}
