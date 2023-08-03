using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  PlayerPrefs 관련 담당 클래스
///  데이터 저장, 불러오기, 설정 메소드
///  
///  Data { NAME, MONEY, BESTSCORE, PLANE, BUSTER, STARBUSTER, ZBUSTER, HBUSTER, BGM, SFX, JOYSTICK} 
///  데이터 : 1.이름 2.돈 3.최고기록 4.비행기 5.부스터 6. 스타부스터 7.ㅋ부스터 8.ㅎ부스터  9. BGM 10. SFX 11.조이스특(오른손,왼손잡이)
/// </summary>
public class DataManager : Singleton<DataManager>
{
    #region
    public void GameModeSave(int num)
    {
        PlayerPrefs.SetInt("GameMode",num);
    }

    public int GetGameMode()
    {
        return PlayerPrefs.GetInt("GameMode");
    }
    #endregion

    #region 비행기 선택 & 반환
    public void PlaneSelectSave(int num)
    {
        PlayerPrefs.SetInt("PLANE", num);
        GameManager.planeNumber = num;
    }

    public int GetPlaneSelected()
    {
        return PlayerPrefs.GetInt("PLANE");
    }
    #endregion

    #region 부스터 선택 & 반환
    public void BusterSelectSave(int num)
    {
        PlayerPrefs.SetInt("BUSTER", num);
        GameManager.busterNumber = num;
    }

    public int GetBusterSelected()
    {
        return PlayerPrefs.GetInt("BUSTER");
    }
    #endregion

    #region PlayerName 설정
    //플레이어 이름 불러오기
    public string GetPlayerName()
    {
        return PlayerPrefs.GetString("NAME");
    }
    //플레이어 이름 저장하기
    public void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString("NAME", playerName);
    }
    #endregion

    #region Player Money 설정
    public int GetPlayerMoney()
    {
        return PlayerPrefs.GetInt("MONEY");
    }
    //플레이어 돈 더하기
    public void AddPlayerMoney(int addMoney)
    {
        int currentMoney = PlayerPrefs.GetInt("MONEY");
        currentMoney += addMoney;
        SetPlayerMoney(currentMoney);
    }
    //플레이어 돈 빼기
    public void MinusPlayerMoney(int minusMoney)
    {
        int currentMoney = PlayerPrefs.GetInt("MONEY");
        currentMoney -= minusMoney;
        SetPlayerMoney(currentMoney);
    }
    //플레이어 돈 저장
    private void SetPlayerMoney(int playerMoney)
    {
        PlayerPrefs.SetInt("MONEY",playerMoney);
    }
    #endregion

    #region 최고기록
    //최고기록 가져와 비교하기
    public int CompareBestScore(int score)
    {
        int bestScore = PlayerPrefs.GetInt("BESTSCORE");
        if (score > bestScore)
        {
            bestScore = score;
            SetBestScore(score);
        }
        return bestScore;
    }

    //최고기록 저장하기
    private void SetBestScore(int score)
    {
        PlayerPrefs.SetInt("BESTSCORE",score);
    }
    #endregion

    #region 부스터 구매 확인

    // 별 부스터 구매 확인
    public bool CheckStarBuster()
    {
        int num = PlayerPrefs.GetInt("STARBUSTER");
        if (num != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // ㅋ 부스터 구매 확인
    public bool CheckZBuster()
    {
        int num = PlayerPrefs.GetInt("ZBUSTER");
        if (num != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // ㅎ 부스터 구매 확인
    public bool CheckHBuster()
    {
        int num = PlayerPrefs.GetInt("HBUSTER");
        if (num != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region 부스터 구매
    public void BuyItem(int itemID)
    {
        switch (itemID)
        {
            case 1:
                BuyStarBuster();
                break;
            case 2:
                BuyZBuster();
                break;
            case 3:
                BuyHBuster();
                break;
            default:
                break;
        }
    }

    //별 부스터 구매
    private void BuyStarBuster()
    {
        PlayerPrefs.SetInt("STARBUSTER", 1);
    }

    //ㅋ 부스터 구매
    private void BuyZBuster()
    {
        PlayerPrefs.SetInt("ZBUSTER", 1);
    }

    //ㅎ 부스터 구매
    private void BuyHBuster()
    {
        PlayerPrefs.SetInt("HBUSTER", 1);
    }
    #endregion

    #region SFX 설정
    public void SetSFXSound(int result)
    {
        //0 : ON  , 1: OFF   (기본 0으로 저장돼서)
        PlayerPrefs.SetInt("SFX",result);
    }

    public bool GetSFSXSound()
    {
        int num = PlayerPrefs.GetInt("SFX");
        if (num == 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region BGM 설정
    public void SetBGMSound(int result)
    {
        //0 : ON  , 1: OFF (기본 0으로 저장돼서)
        PlayerPrefs.SetInt("BGM", result);
    }

    public bool GetBGMXSound()
    {
        int num = PlayerPrefs.GetInt("BGM");
        if (num == 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region 조이스틱 설정
    //조이스틱 오른손잡이 왼손잡이 설정
    public void SetJoystic(int hand)
    {
        if (hand==0)
        {
            PlayerPrefs.SetInt("JOYSTICK", 0);
        }
        else
        {
            PlayerPrefs.SetInt("JOYSTICK", 1);
        }
    }
    //조이스틱 오른손잡이 왼손잡이 리턴
    public bool GetJoystick()
    {
        int hand = PlayerPrefs.GetInt("JOYSTICK"); // 0 -> 오른손  1-> 왼손
        if (hand == 0)
        {
            return true;
        }
        else {
            return false;
        }
    }
    #endregion
}
