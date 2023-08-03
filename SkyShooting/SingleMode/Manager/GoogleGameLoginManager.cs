using Firebase;
using Firebase.Auth;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class GoogleGameLoginManager : Singleton<GoogleGameLoginManager>
{
    //Manager
    private DataManager DM;

    //Firebase
    public FirebaseApp firebaseApp; // firebas Application을 관리
    public FirebaseAuth firebaseAuth; // firebas Application을 중 Auth를 관리
    public FirebaseUser User;

    //UI Component
    public Button playButton;
    public Text Firebase_Text;

    //
    public bool IsFirebaseReady { get; private set; }     //현재 Firebase를 구동시킬 수 있는지
    public bool IsLogin; //로그인 완료
    private string authCode;

#if UNITY_ANDROID
     //PC일때는 주석 해줘야 함
    void Start()
    {
        DM = DataManager.Instance;

        IsLogin = false;
        IsFirebaseReady = false;
        playButton.interactable = IsLogin; 

         PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                                                   .RequestServerAuthCode(false) //  Don't force refresh 
                                                   .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        IsFirebaseReady = true;

        Firebase_Text.text = "Firebase Ready";

        OnLogin();

    }

    private void Update()
    {
        //로그인 성공시 or 로그인 실패시 메인 로비 화면에서 계정연결 할 수 있도록 해줌
        if (IsLogin || IsFirebaseReady)
        {
            playButton.interactable = true;
        }
    }

    //GoogleGame 계정 로그인
    public void OnLogin()
    {
        if (IsFirebaseReady)
        {
            Social.localUser.Authenticate((bool success) => {
                if (success)
                {
                    authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                    Firebase_Text.text = Social.localUser.userName;

                    //저장된 PlayerName 불러오기 
                    string savePlayerName = DM.GetPlayerName();
                    //저장된 PlayerName이 공백이면
                    if (string.IsNullOrEmpty(savePlayerName))
                    {
                        DM.SetPlayerName(Social.localUser.userName); //PlayerName 저장하기
                    }
                }
                else
                {
                    Firebase_Text.text = "Fail";
                }
            });
            firebaseAuth = FirebaseAuth.DefaultInstance;
            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
            firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    Firebase_Text.text = "Canceled";
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    Firebase_Text.text = "Faulted";
                    return;
                }

                FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                Firebase_Text.text = "new" + newUser.DisplayName;
            });
            User = firebaseAuth.CurrentUser;
            if (User != null)
            {
                string playerName = User.DisplayName;

                // The user's Id, unique to the Firebase project.
                // Do NOT use this value to authenticate with your backend server, if you
                // have one; use User.TokenAsync() instead.
                string uid = User.UserId;

                Firebase_Text.text = playerName + uid;
            }

            IsLogin = true;
        }
        else
        {
            IsLogin = false;
            Firebase_Text.text = "not Ready";
        }
     }

    //Google Game 계정 로그아웃
    public void OnLogout()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        firebaseAuth.SignOut();
        Firebase_Text.text = "Logout";
    }
#endif
}