using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MatchManager : MonoBehaviourPunCallbacks
{
    //Manager
    LoadingManager loadingManager;
    DataManager dataManager;

    [Header("UI")]
    [SerializeField]
    private Canvas matchingCanvas;
    [SerializeField]
    private Text connectionInfoText;

    [Header("Photon Setting")]
    [SerializeField]
    private byte turnMaxPlayers;
    [SerializeField]
    private byte battlRoyaleMaxPlayers;

    // Photon Properties
    private RoomOptions roomOptions;
    private ExitGames.Client.Photon.Hashtable hashTable;
    // variable
    private bool matchCancel;

    // constant
    private const string gameVersion = "1";

    private void Awake()
    {
        //Manager - Singleton
        loadingManager = LoadingManager.Instance;
        dataManager = DataManager.Instance;

        PhotonNetwork.GameVersion = gameVersion;
        //마스터 클라이언트와 같은 룸의 모든 클라이언트의 LoadLevel()이 자동으로 수준 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        roomOptions = new RoomOptions();
    }
    private void OnEnable()
    {
        base.OnEnable();
        matchingCanvas.gameObject.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        connectionInfoText.text = "Connecting To Master Server...";
    }

    private void OnDisable()
    {
        base.OnDisable();
        matchCancel = false;
    }

    public void MacthingCancel()
    {
        matchCancel = true;
        StartCoroutine(Disconnect());
    }

   IEnumerator Disconnect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        gameObject.SetActive(false);
        matchingCanvas.gameObject.SetActive(false);
    }

    #region Photon Callback
    //마스터 서버 접속 성공시
    public override void OnConnectedToMaster()
    {
        connectionInfoText.text = "Online : Connected to Master Server";
        PhotonNetwork.JoinLobby();

    }

    //접속 실패, 접속 끊긴 경우
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectionInfoText.text = $"Offline : Connected Disable {cause.ToString()} - Try reconnecting..";
        Debug.Log("연결 끊김");
        if (!matchCancel)
        {
            PhotonNetwork.ConnectUsingSettings();// 재접속 시도
            Debug.Log("연결 재시도");
        }
    }

    //방 접속
    public void Connect()
    {
        //안전장치
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "Connecting to Random Room...";
            PhotonNetwork.JoinRandomRoom(); //랜덤 방으로 접속
        }
        else
        {
            //혹시 모를 끊길 경우 재접속
            connectionInfoText.text = "Offline : Connected Disable - Try reconnecting..";
            PhotonNetwork.ConnectUsingSettings();// 재접속 시도
        }
    }

    public override void OnJoinedLobby()
    {
#if UNITY_ANDROID
        PhotonNetwork.LocalPlayer.NickName = DataManager.Instance.GetPlayerName();
#elif UNITY_IOS
        PhotonNetwork.LocalPlayer.NickName = "IOS Player" + PhotonNetwork.CountOfPlayers;
#else
        PhotonNetwork.LocalPlayer.NickName = "Win Player" + PhotonNetwork.CountOfPlayers;
#endif

        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        if (dataManager.GetGameMode() == 1)
        {
            hashTable = new ExitGames.Client.Photon.Hashtable() { { "T", 1 } };

            roomOptions.CustomRoomProperties = hashTable;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "T" };
            roomOptions.MaxPlayers = turnMaxPlayers;

            PhotonNetwork.JoinRandomRoom(hashTable, turnMaxPlayers);
        }
        else if (dataManager.GetGameMode() == 2)
        {
            hashTable = new ExitGames.Client.Photon.Hashtable() { { "B", 1 } };

            roomOptions.CustomRoomProperties = hashTable;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "B" };
            roomOptions.MaxPlayers = battlRoyaleMaxPlayers;

            PhotonNetwork.JoinRandomRoom(hashTable, battlRoyaleMaxPlayers);
        }
    }

    //대부분의 경우 빈방이 없을 경우 방을 접속하려 할때 Failed
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "There is no empty room, Creatin new Room.";
        Debug.Log("랜덤 룸 입장 실패");
        //새로운 방 생성 : (생성 방 이름, 방 제약사항) 후 방 입장
        PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "Create Room Failed, Trying JoinRandomRoom";
        Debug.Log("방생성 실패");
        PhotonNetwork.JoinRandomRoom(hashTable, battlRoyaleMaxPlayers);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "Join room Failed, Trying JoinRandomRoom";
        Debug.Log("방 입장 실패");
        PhotonNetwork.JoinRandomRoom(hashTable, battlRoyaleMaxPlayers);
    }

    public override void OnLeftLobby()
    {
        connectionInfoText.text = "Leave Lobby";
    }

    //방 접속 성공 한 경우
    public override void OnJoinedRoom()
    {
        loadingManager.StartLoading();

        connectionInfoText.text = "Connected with Room";
        int gameMode = dataManager.GetGameMode();

        switch (gameMode)
        {
            case 1:
                PhotonNetwork.LoadLevel("Turn");
                break;
            case 2:
                PhotonNetwork.LoadLevel("BattleRoyale");
                break;
        }

        loadingManager.EndLoading();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("누군가 들어왔다");
        if (dataManager.GetGameMode() == 1)
        {
            PhotonNetwork.LoadLevel("Turn");
        }
    }
    #endregion
}
