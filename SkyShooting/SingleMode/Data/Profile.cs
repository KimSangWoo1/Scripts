using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile 
{
    //Player 전용
    public string name; //이름
    [HideInInspector]
    public int dollar = 0; //돈

    //공용
    public int score = 0; // 점수
    [HideInInspector]
    public int skinType = 0; // 비행기 색
    [HideInInspector]
    public int busterType = 0; // 비행기 부스터 타입


    //네트워크용
    public int photonViewId;
    public Profile(int _photonViewId, string _name, int _score)
    {
        photonViewId = _photonViewId;
        name = _name;
        score = _score;
    }
    public void UpdateProfile(string _name, int _score, int _dollar)
    {
        name = _name;
        score = _score;
        dollar = _dollar;
    }
    public void UpdateName(string _name)
    {
        name = _name;
    }
    public void UpdateScore(int _score)
    {
        score = _score;
    }

    public void UpdateDollar(int _dollar)
    {
        dollar += _dollar;
    }
    public void UpdatePlaneType(int _skinType, int _busterType)
    {
        skinType = _skinType;
        busterType = _busterType;
    }
    public void UpdatePhotonViewId(int _photonViewId)
    {
        photonViewId = _photonViewId;
    }

    public int GetPotonViewId()
    {
        return photonViewId;
    }

    public string GetPlayerName()
    {
        return name;
    }

    public int GetScore()
    {
        return score;
    }
}
