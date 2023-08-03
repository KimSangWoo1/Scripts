using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ResultBoardControl : MonoBehaviour
{
    //Manager
    DataManager DM;

    //AD
    public GoogleMobileAD googleMobileAD;

    //UI Component
    public Image board;
    public Text playerNameText;
    public Text scoreText;
    public Text dollarText;
    public Text bestScoreText;

    public Button homeBtn;
    public Button rePlayBtn;

    private void Start()
    {
        DM = DataManager.Instance;
    }

    //결과 보드 보여주기
    public void SetResultBoard(Profile profile)
    {
        //결과 Board Object 켜기
        board.gameObject.SetActive(true);

        //Player에게 점수, 돈 보여주기
        playerNameText.text = profile.name;
        scoreText.text = profile.score.ToString();
        dollarText.text = profile.dollar.ToString();
        bestScoreText.text = DM.CompareBestScore(profile.score).ToString(); // 베스트 점수 비교 후 저장하여 가져오기
        //데이터 저장
        DM.AddPlayerMoney(profile.dollar);

        //버튼 활성화 중지
        homeBtn.interactable = false;
        rePlayBtn.interactable = false;

        //대기 시간 후 광고 재개
        StartCoroutine(nameof(WaitAd));
    }

    //LoadLobbyScene 전환
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    //DeathMatchScene 전환
    public void LoadDeathMatchScene()
    {
        SceneManager.LoadScene("DeathMatchScene");
    }

    private IEnumerator WaitAd()
    {
        print("Wait");
        yield return new WaitForSeconds(1.5f);
        print("End");
        googleMobileAD.GameOver();
        homeBtn.interactable = true;
        rePlayBtn.interactable = true;

    }
}
